// Copyright (c) Richasy. All rights reserved.

using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Flurl.Http;
using Polly;
using System;
using System.Linq;
using Google.Protobuf;
using System.Text.Json;

namespace Richasy.BiliKernel.Http;

/// <summary>
/// BiliBili 网络请求客户端.
/// </summary>
public sealed partial class BiliHttpClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BiliHttpClient"/> class.
    /// </summary>
    public BiliHttpClient()
    {
        var policy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .RetryAsync(3);
        var cache = FlurlHttp.Clients.WithDefaults(builder => builder.AddMiddleware(() => new PollyHandler(policy)));
        _client = cache.GetOrAdd("Bili");
        _client
            .WithHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69")
            .WithTimeout(30);
    }

    /// <summary>
    /// 创建一个请求.
    /// </summary>
    /// <param name="method">请求方法.</param>
    /// <param name="uri">请求地址.</param>
    /// <returns><see cref="IFlurlRequest"/>.</returns>
    public static IFlurlRequest CreateRequest(HttpMethod method, Uri uri)
    {
        var request = new FlurlRequest(uri)
            .WithAutoRedirect(false);
        request.Verb = method;
        return request;
    }

    /// <summary>
    /// 创建一个 GRPC 请求.
    /// </summary>
    /// <param name="uri">请求地址.</param>
    /// <param name="grpcMessage">消息结构体.</param>
    /// <returns>请求信息.</returns>
    public static IFlurlRequest CreateRequest(Uri uri, IMessage grpcMessage)
    {
        var request = new FlurlRequest(uri);
        request.Verb = HttpMethod.Post;
        var messageBytes = grpcMessage.ToByteArray();
        var stateBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, Convert.ToByte(messageBytes.Length) };
        var bytes = stateBytes.Concat(messageBytes).ToArray();
        var byteArrayContent = new ByteArrayContent(bytes);
        byteArrayContent.Headers.ContentType = new("application/grpc");
        byteArrayContent.Headers.ContentLength = bytes.Length;
        request.Content = byteArrayContent;
        return request;
    }

    /// <summary>
    /// 发送请求.
    /// </summary>
    /// <param name="request">请求信息.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>响应结果.</returns>
    /// <exception cref="HttpOperationException"></exception>
    /// <exception cref="KernelException"></exception>
    public async Task<IFlurlResponse> SendAsync(IFlurlRequest request, CancellationToken cancellationToken = default)
    {
        Verify.NotNull(request, nameof(request));
        try
        {
            var response = await _client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            await ThrowIfResponseInvalidAsync(response).ConfigureAwait(false);
            return response;
        }
        catch (HttpOperationException)
        {
            throw;
        }
        catch (HttpRequestException httpEx)
        {
            throw new HttpOperationException(httpEx.Message, httpEx);
        }
        catch (Exception ex)
        {
            throw new KernelException($"未知错误：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 直接获取字符串.
    /// </summary>
    public async Task<string> GetStringAsync(IFlurlRequest request, CancellationToken cancellationToken = default)
    {
        Verify.NotNull(request, nameof(request));
        var response = await _client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        return await response.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// 从响应中获取数据.
    /// </summary>
    /// <typeparam name="T">数据类型.</typeparam>
    /// <param name="response">响应.</param>
    /// <returns><see cref="Task{T}"/></returns>
    public static async Task<T> ParseAsync<T>(IFlurlResponse response)
    {
        Verify.NotNull(response, nameof(response));
        var contentText = await response.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(contentText);
    }

    /// <summary>
    /// 从 Protobuf 响应中获取数据.
    /// </summary>
    /// <typeparam name="T">数据类型.</typeparam>
    /// <param name="response">响应.</param>
    /// <param name="parser">解析器.</param>
    /// <returns><see cref="Task{T}"/></returns>
    public static async Task<T> ParseAsync<T>(IFlurlResponse response, MessageParser<T> parser)
        where T : IMessage<T>
    {
        Verify.NotNull(response, nameof(response));
        Verify.NotNull(parser, nameof(parser));
        var bytes = await response.ResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        return parser.ParseFrom(bytes.Skip(5).ToArray());
    }

    internal class PollyHandler(IAsyncPolicy<HttpResponseMessage> policy) : DelegatingHandler
    {
        private readonly IAsyncPolicy<HttpResponseMessage> _policy = policy;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => _policy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
    }
}
