// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.User.Core;

internal sealed class MyClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly IBiliTokenResolver _tokenResolver;
    private readonly BasicAuthenticator _authenticator;

    public MyClient(
        BiliHttpClient httpClient,
        IAuthenticationService authenticationService,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator basicAuthenticator)
    {
        _httpClient = httpClient;
        _authenticationService = authenticationService;
        _tokenResolver = tokenResolver;
        _authenticator = basicAuthenticator;
    }

    public async Task<UserDetailProfile> GetMyInformationAsync(CancellationToken cancellationToken)
    {
        var responseObj = await GetAsync<BiliDataResponse<MyInfo>>(BiliApis.Account.MyInfo, cancellationToken: cancellationToken).ConfigureAwait(false);
        var info = responseObj.Data;
        return info == null || string.IsNullOrEmpty(info.Name)
            ? throw new KernelException("返回的用户数据为空")
            : info.ToUserDetailProfile(48d);
    }

    public async Task<UserCommunityInformation> GetMyCommunityInformationAsync(CancellationToken cancellationToken)
    {
        var responseObj = await GetAsync<BiliDataResponse<Mine>>(BiliApis.Account.Mine, cancellationToken: cancellationToken).ConfigureAwait(false);
        var mine = responseObj.Data;
        return mine == null || string.IsNullOrEmpty(mine.Name)
            ? throw new KernelException("无法获取用户社区数据")
            : mine.ToUserCommunityInformation();
    }

    public async Task<IReadOnlyList<UserGroup>> GetMyFollowUserGroupsAsync(CancellationToken cancellationToken)
    {
        var responseObj = await GetAsync<BiliDataResponse<List<RelatedTag>>>(BiliApis.Account.MyFollowingTags, cancellationToken: cancellationToken).ConfigureAwait(false);
        return responseObj.Data?.Select(p => p.ToUserGroup()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取用户关注的分组数据");
    }

    public async Task<IReadOnlyList<UserCard>> GetMyFollowUserGroupDetailAsync(string groupId, int page = 1, CancellationToken cancellationToken = default)
    {
        var localToken = _tokenResolver.GetToken();
        var parameters = new Dictionary<string, string>
        {
            { "tagid", groupId },
            { "pn", page.ToString() },
            { "mid", localToken?.UserId.ToString() ?? string.Empty },
        };

        var responseObj = await GetAsync<BiliDataResponse<List<RelatedUser>>>(BiliApis.Account.MyFollowingTagDetail, parameters, cancellationToken).ConfigureAwait(false);
        var users = responseObj.Data?.Select(p => p.ToUserCard()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取用户关注的分组详情数据");
        foreach (var user in users)
        {
            user.Community.Relation = UserRelationStatus.Following;
        }

        return users;
    }

    public async Task<(IReadOnlyList<UserCard> Users, int Count)> GetMyFansAsync(int page = 1, CancellationToken cancellationToken = default)
    {
        var localToken = _tokenResolver.GetToken();
        var parameters = new Dictionary<string, string>
        {
            { "pn", page.ToString() },
            { "vmid", localToken.UserId.ToString() },
        };

        var responseObj = await GetAsync<BiliDataResponse<RelatedUserResponse>>(BiliApis.Account.Fans, parameters, cancellationToken).ConfigureAwait(false);
        var users = responseObj.Data?.UserList?.Select(p => p.ToUserCard()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取用户粉丝数据");
        return (users, responseObj.Data.TotalCount);
    }

    public async Task<(IReadOnlyList<VideoInformation> Videos, int Count)> GetMyViewLaterAsync(int page = 0, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "pn", page.ToString() },
            { "ps", "40" },
        };

        var responseObj = await GetAsync<BiliDataResponse<ViewLaterResponse>>(BiliApis.Account.ViewLaterList, parameters, cancellationToken).ConfigureAwait(false);
        var videos = responseObj.Data?.List?.Select(p => p.ToVideoInformation()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取稍后再看视频数据");
        return (videos, responseObj.Data.Count);
    }

    private async Task<T> GetAsync<T>(string url, Dictionary<string, string>? paramters = default, CancellationToken cancellationToken = default)
    {
        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(url));
        _authenticator.AuthroizeRestRequest(request, paramters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await BiliHttpClient.ParseAsync<T>(response).ConfigureAwait(false);
    }
}
