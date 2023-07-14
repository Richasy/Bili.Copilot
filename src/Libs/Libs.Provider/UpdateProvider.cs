// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 应用更新相关.
/// </summary>
public static class UpdateProvider
{
    private const string LatestReleaseUrl = "https://api.github.com/repos/Richasy/Bili.Copilot/releases/latest";

    /// <summary>
    /// 获取Github最新的发布版本.
    /// </summary>
    /// <returns>最新发布版本.</returns>
    public static async Task<GithubReleaseResponse> GetGithubLatestReleaseAsync()
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add(ServiceConstants.Headers.UserAgent, ServiceConstants.DefaultUserAgentString);
        var request = new HttpRequestMessage(HttpMethod.Get, LatestReleaseUrl);
        var response = await httpClient.SendAsync(request, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
        _ = response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GithubReleaseResponse>(content);
    }
}
