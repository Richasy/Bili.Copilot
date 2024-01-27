// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Grpc;
using Flurl.Http;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 用于网络请求模块的内部方法.
/// </summary>
public sealed partial class HttpProvider
{
    private static string _tempBuvid = string.Empty;
    private FlurlClient _client;

    /// <summary>
    /// 实例.
    /// </summary>
    public static HttpProvider Instance { get; } = new HttpProvider();

    /// <summary>
    /// 网络客户端.
    /// </summary>
    public HttpClient HttpClient => _client.HttpClient;

    internal async Task<HttpResponseMessage> SendRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await HttpClient.SendAsync(request, cancellationToken);
            await ThrowIfHasExceptionAsync(response);
        }
        catch (TaskCanceledException exception)
        {
            throw new ServiceException(
                    new ServerResponse
                    {
                        Code = 408,
                        Message = ServiceConstants.Messages.RequestTimedOut,
                        IsHttpError = true,
                    },
                    exception);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception exception)
        {
            if (exception.Message.Contains("WSAStartup")
                || (exception.InnerException is not null && exception.InnerException.Message.Contains("WSAStartup")))
            {
                InitHttpClient();
            }

            throw new ServiceException(
                    new ServerResponse
                    {
                        Message = ServiceConstants.Messages.UnexpectedExceptionOnSend,
                        IsHttpError = true,
                    },
                    exception);
        }

        return response;
    }

    internal async Task<IFlurlResponse> SendRequestAsync(
        IFlurlRequest request,
        CancellationToken cancellationToken)
    {
        IFlurlResponse response = default;
        try
        {
            response = await _client.SendAsync(request, cancellationToken: cancellationToken);
            await ThrowIfHasExceptionAsync(response.ResponseMessage);
        }
        catch (TaskCanceledException exception)
        {
            throw new ServiceException(
                    new ServerResponse
                    {
                        Code = 408,
                        Message = ServiceConstants.Messages.RequestTimedOut,
                        IsHttpError = true,
                    },
                    exception);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception exception)
        {
            if (exception.Message.Contains("WSAStartup")
                || (exception.InnerException is not null && exception.InnerException.Message.Contains("WSAStartup")))
            {
                InitHttpClient();
            }

            throw new ServiceException(
                    new ServerResponse
                    {
                        Message = ServiceConstants.Messages.UnexpectedExceptionOnSend,
                        IsHttpError = true,
                    },
                    exception);
        }

        return response;
    }

    private static string GetBuvid()
    {
        if (string.IsNullOrEmpty(_tempBuvid))
        {
            var macAddress = NetworkInterface.GetAllNetworkInterfaces()
                 .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                 .Select(nic => nic.GetPhysicalAddress().ToString())
                 .FirstOrDefault();
            var buvidObj = new Buvid(macAddress);
            _tempBuvid = buvidObj.Generate();
        }

        return _tempBuvid;
    }

    private static async Task ThrowIfHasExceptionAsync(HttpResponseMessage response)
    {
        ServerResponse errorResponse = null;
        try
        {
            if (response.Content.Headers.ContentType?.MediaType.Contains("image") ?? false)
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            else if (response.Content.Headers.ContentType?.MediaType == ServiceConstants.Headers.GRPCContentType)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                if (bytes.Length < 5)
                {
                    errorResponse = new ServerResponse { Message = ServiceConstants.Messages.NoData };
                    throw new ServiceException(errorResponse, response.Headers, response.StatusCode);
                }

                return;
            }
            else if (response.Content.Headers.ContentType?.MediaType != ServiceConstants.Headers.JsonContentType)
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }

            var errorResponseStr = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(errorResponseStr))
            {
                return;
            }

            errorResponse = JsonSerializer.Deserialize<ServerResponse>(errorResponseStr);
            if (errorResponse?.Code == 0 || string.IsNullOrEmpty(errorResponseStr))
            {
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        if (errorResponse == null || !response.IsSuccessStatusCode)
        {
            errorResponse = response != null && response.StatusCode == HttpStatusCode.NotFound
                ? new ServerResponse
                {
                    Message = ServiceConstants.Messages.NotFound,
                    IsHttpError = true,
                }
                : new ServerResponse
                {
                    Message = ServiceConstants.Messages.UnexpectedExceptionResponse,
                    IsHttpError = true,
                };
        }

        if (response.Content?.Headers.ContentType?.MediaType == ServiceConstants.Headers.JsonContentType)
        {
            var rawResponseBody = await response.Content.ReadAsStringAsync();

            throw new ServiceException(
                errorResponse,
                response.Headers,
                response.StatusCode,
                rawResponseBody,
                null);
        }
        else
        {
            // 将响应头和状态代码传递给ServiceException。
            // System.Net.HttpStatusCode不支持RFC 6585，附加HTTP状态代码。
            // 节流状态代码429是在RFC 6586中。状态码429将被传递过去。
            throw new ServiceException(errorResponse, response.Headers, response.StatusCode);
        }
    }

    private void InitHttpClient()
    {
        _client?.Dispose();
        _client = new FlurlClient()
            .WithHeader("user-agent", ServiceConstants.DefaultUserAgentString);
    }
}
