// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 获取专业内容创作数据的工具.
/// </summary>
public partial class PgcProvider
{
    private readonly Dictionary<PgcType, string> _pgcOffsetCache;
    private readonly Dictionary<PgcType, int> _pgcIndexCache;

    /// <summary>
    /// 实例.
    /// </summary>
    public static PgcProvider Instance { get; } = new PgcProvider();

    private static Dictionary<string, string> GetTabQueryParameters(PgcType type)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Device, "phone" },
            { Query.IsHideRecommendTab, "0" },
        };
        var parentTab = string.Empty;
        switch (type)
        {
            case PgcType.Bangumi:
                parentTab = BangumiOperation;
                break;
            case PgcType.Domestic:
                parentTab = DomesticOperation;
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(parentTab))
        {
            queryParameters.Add(Query.ParentTab, parentTab);
        }

        return queryParameters;
    }

    private static Dictionary<string, string> GetPageDetailQueryParameters(string tabId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Device, "phone" },
            { Query.Fnval, "976" },
            { Query.Fnver, "0" },
            { Query.Fourk, "1" },
            { Query.Qn, "112" },
            { Query.TabId, tabId },
            { Query.TeenagersMode, "0" },
        };

        return queryParameters;
    }

    private static Dictionary<string, string> GetPageDetailQueryParameters(PgcType type, string cursor)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Device, "phone" },
            { Query.Fnval, "976" },
            { Query.Fnver, "0" },
            { Query.Fourk, "1" },
            { Query.Qn, "112" },
        };

        switch (type)
        {
            case PgcType.Movie:
                queryParameters.Add(Query.Name, MovieOperation);
                break;
            case PgcType.Documentary:
                queryParameters.Add(Query.Name, DocumentaryOperation);
                break;
            case PgcType.TV:
                queryParameters.Add(Query.Name, TvOperation);
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(cursor))
        {
            queryParameters.Add(Query.Cursor, cursor);
        }

        return queryParameters;
    }

    private static Dictionary<string, string> GetFollowQueryParameters(string seasonId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.SeasonId, seasonId },
        };

        return queryParameters;
    }

    private static Dictionary<string, string> GetPgcIndexBaseQueryParameters(PgcType type)
    {
        var queryParameters = new Dictionary<string, string>();

        if (type is PgcType.Bangumi or PgcType.Domestic)
        {
            queryParameters.Add(Query.SeasonType, "1");
        }
        else
        {
            var indexType = string.Empty;
            switch (type)
            {
                case PgcType.Movie:
                    indexType = "2";
                    break;
                case PgcType.Documentary:
                    indexType = "3";
                    break;
                case PgcType.TV:
                    indexType = "5";
                    break;
                default:
                    break;
            }

            queryParameters.Add(Query.IndexType, indexType);
        }

        queryParameters.Add(Query.Type, "0");
        return queryParameters;
    }

    private static Dictionary<string, string> GetPgcIndexResultQueryParameters(PgcType type, int page, Dictionary<string, string> additionalParameters)
    {
        var queryParameters = GetPgcIndexBaseQueryParameters(type);
        queryParameters.Add(Query.PageSizeFull, "21");
        queryParameters.Add(Query.Page, page.ToString());

        if (additionalParameters != null)
        {
            foreach (var item in additionalParameters)
            {
                queryParameters.Add(item.Key, item.Value);
            }
        }

        return queryParameters;
    }

    private static Dictionary<string, string> GetPgcTimeLineQueryParameters(PgcType type)
    {
        var typeStr = string.Empty;
        switch (type)
        {
            case PgcType.Bangumi:
                typeStr = "2";
                break;
            case PgcType.Domestic:
                typeStr = "3";
                break;
            case PgcType.Movie:
            case PgcType.Documentary:
            case PgcType.TV:
            default:
                break;
        }

        var queryParameters = new Dictionary<string, string>
        {
            { Query.Type, typeStr },
            { Query.FilterType, "0" },
        };
        return queryParameters;
    }

    private static Dictionary<string, string> GetPgcPlayListQueryParameters(string id)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Id, id },
        };

        return queryParameters;
    }

    private static async Task<PgcResponse> GetPgcResponseInternalAsync(Dictionary<string, string> queryParameters)
    {
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, ApiConstants.Pgc.PageDetail, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse2<PgcResponse>>(response);
        return data.Result;
    }
}
