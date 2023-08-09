// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 搜索工具.
/// </summary>
public partial class SearchProvider
{
    private int _comprehensivePageNumber;
    private int _animePageNumber;
    private int _moviePageNumber;
    private int _userPageNumber;
    private int _articlePageNumber;
    private int _livePageNumber;

    /// <summary>
    /// 实例.
    /// </summary>
    public static SearchProvider Instance { get; } = new SearchProvider();

    private static Dictionary<string, string> GetSearchBasicQueryParameters(string keyword, string orderType, int pageNumber)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Keyword, Uri.EscapeDataString(keyword) },
            { Query.Order, orderType },
            { Query.PageNumber, pageNumber.ToString() },
            { Query.PageSizeSlim, "20" },
        };

        return queryParameters;
    }

    private static async Task<SubModuleSearchResultResponse<T>> GetSubModuleResultAsync<T>(int typeId, string keyword, string orderType, int pageNumber, Dictionary<string, string> additionalParameters = null)
    {
        var proxy = string.Empty;
        var area = string.Empty;
        var isOpenRoaming = SettingsToolkit.ReadLocalSetting(SettingNames.IsOpenRoaming, false);
        var localProxy = SettingsToolkit.ReadLocalSetting(SettingNames.RoamingSearchAddress, string.Empty);
        if (isOpenRoaming && !string.IsNullOrEmpty(localProxy))
        {
            proxy = localProxy;
            area = "area=hk";
        }

        var queryParameters = GetSearchBasicQueryParameters(keyword, orderType, pageNumber);
        queryParameters.Add(Query.Type, typeId.ToString());
        if (additionalParameters != null && additionalParameters.Count > 0)
        {
            foreach (var item in additionalParameters)
            {
                queryParameters.Add(item.Key, item.Value);
            }
        }

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Models.App.Constants.ApiConstants.Search.SubModuleSearch(proxy), queryParameters, RequestClientType.IOS, additionalQuery: area);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<SubModuleSearchResultResponse<T>>>(response);
        return result.Data;
    }
}
