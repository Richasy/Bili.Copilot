// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl.Http;
using Richasy.BiliKernel.Content;

namespace Richasy.BiliKernel.Http;

public sealed partial class BiliHttpClient
{
    private readonly IFlurlClient _client;

    /// <summary>
    /// 从响应中获取 Cookie.
    /// </summary>
    /// <param name="response">响应结果.</param>
    /// <returns>Cookie 列表.</returns>
    public static Dictionary<string, string> GetCookiesFromResponse(IFlurlResponse response)
    {
        Verify.NotNull(response, nameof(response));
        return response.Cookies.ToDictionary(p => p.Name, p => p.Value);
    }

    private static async Task ThrowIfResponseInvalidAsync(IFlurlResponse response)
    {
        response.ResponseMessage.EnsureSuccessStatusCode();
        response.Headers.TryGetFirst("Content-Type", out var contentType);
        contentType ??= string.Empty;
        if (contentType.Contains("image"))
        {
            return;
        }
        else if (contentType.Contains("grpc"))
        {
            var bytes = await response.ResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            if (bytes.Length < 5)
            {
                throw new HttpOperationException("哔哩哔哩返回了一个空的响应");
            }

            return;
        }

        var responseContent = await response.GetJsonAsync<BiliResponse>().ConfigureAwait(false)
            ?? throw new HttpOperationException("哔哩哔哩返回了一个空的响应");
        if (!responseContent.IsSuccess())
        {
            throw new HttpOperationException($"哔哩哔哩返回了一个异常响应: {responseContent.Message ?? "N/A"}", new System.Exception(JsonSerializer.Serialize(responseContent)));
        }
    }
}
