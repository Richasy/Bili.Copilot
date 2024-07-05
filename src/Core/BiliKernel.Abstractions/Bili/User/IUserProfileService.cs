using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Bili.User;

/// <summary>
/// 用户资料服务.
/// </summary>
public interface IUserProfileService
{
    /// <summary>
    /// 获取已登录用户的资料.
    /// </summary>
    Task<UserDetailProfile> GetMyProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已登录用户的社区信息.
    /// </summary>
    Task<UserCommunityInformation> GetMyCommunityInformationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已登录用户关注的分组.
    /// </summary>
    Task<IReadOnlyList<UserGroup>> GetMyFollowUserGroupsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已登录用户关注的分组的详细信息.
    /// </summary>
    Task<IReadOnlyList<UserCard>> GetMyFollowUserGroupDetailAsync(string groupId, int pageNumber = 0, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取已登录用户的关注者.
    /// </summary>
    Task<(IReadOnlyList<UserCard> Users, int Count)> GetMyFansAsync(int pageNumber = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取稍后再看视频.
    /// </summary>
    Task<(IReadOnlyList<VideoInformation> Videos, int Count)> GetMyViewLaterAsync(int pageNumber = 0, CancellationToken cancellationToken = default);
}
