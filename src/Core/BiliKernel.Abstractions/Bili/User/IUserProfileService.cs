using System.Threading;
using System.Threading.Tasks;
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
    /// <param name="executionSettings">执行设置.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns><see cref="UserDetailProfile"/>.</returns>
    Task<UserDetailProfile> GetMyProfileAsync(UserExecutionSettings executionSettings, CancellationToken cancellationToken = default);
}
