// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Data.Pgc;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 收藏夹相关服务提供工具.
/// </summary>
public sealed partial class FavoriteProvider
{
    private int _videoFolderDetailPageNumber;
    private int _videoCollectFolderPageNumber;
    private int _videoCreatedFolderPageNumber;
    private int _animeFolderPageNumber;
    private int _cinemaFolderPageNumber;
    private int _articleFolderPageNumber;

    /// <summary>
    /// 实例.
    /// </summary>
    public static FavoriteProvider Instance { get; } = new FavoriteProvider();

    private static async Task<SeasonSet> GetPgcFavoriteListInternalAsync(string requestUrl, int pageNumber, int status)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.PageNumber, pageNumber.ToString() },
            { Query.PageSizeSlim, "20" },
            { Query.Status, status.ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, requestUrl, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse2<PgcFavoriteListResponse>>(response);
        return PgcAdapter.ConvertToSeasonSet(result.Result);
    }
}
