#define COMPREHENSIVE

using Richasy.BiliKernel;
using Richasy.BiliKernel.Bili.Moment;

#if USER
using Richasy.BiliKernel.Models.User;
#endif
using Spectre.Console;

namespace Bili.Console;

internal sealed class MomentModule : IFeatureModule
{
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IMomentDiscoveryService _momentService;
    private readonly IMomentOperationService _operationService;

    public MomentModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _momentService = kernel.GetRequiredService<IMomentDiscoveryService>();
        _operationService = kernel.GetRequiredService<IMomentOperationService>();
    }

    public async Task RunAsync()
    {
#if USER
        var testUser = new UserProfile("5992670");
        AnsiConsole.MarkupLine("正在获取用户动态");
        var (moments, _, _) = await _momentService.GetUserMomentsAsync(testUser, null, _cancellationToken).ConfigureAwait(false);
        var momentTable = new Table();
        momentTable.AddColumn("ID");
        momentTable.AddColumn("内容");

        foreach (var moment in moments)
        {
            momentTable.AddRow(moment.Id, Markup.Escape(moment.Description?.Text ?? moment.MomentType.ToString()));
        }

        AnsiConsole.Clear();
        AnsiConsole.Write(momentTable);
#endif

#if COMPREHENSIVE
        AnsiConsole.MarkupLine("正在获取综合动态");
        var comprehensiveMoments = await _momentService.GetComprehensiveMomentsAsync(null, null, _cancellationToken).ConfigureAwait(false);
        var comprehensiveMomentTable = new Table();
        comprehensiveMomentTable.AddColumn("ID");
        comprehensiveMomentTable.AddColumn("用户");
        comprehensiveMomentTable.AddColumn("内容");

        foreach (var moment in comprehensiveMoments.Moments)
        {
            comprehensiveMomentTable.AddRow(moment.Id, moment.User.Name, Markup.Escape(moment.Description?.Text ?? moment.MomentType.ToString()));
        }

        AnsiConsole.Write(comprehensiveMomentTable);

        var firstUser = comprehensiveMoments.Users.First(p => p.IsUnread);
        if (AnsiConsole.Confirm($"是否标记 {firstUser.User.Name} 为已读？"))
        {
            await _operationService.MarkUserMomentAsReadAsync(firstUser, comprehensiveMoments.Offset).ConfigureAwait(false);
        }

#endif
#if VIDEO
        AnsiConsole.MarkupLine("正在获取视频动态");
        var videoMoments = await _momentService.GetVideoMomentsAsync(null, null, _cancellationToken).ConfigureAwait(false);
        var videoMomentTable = new Table();
        videoMomentTable.AddColumn("ID");
        videoMomentTable.AddColumn("用户");
        videoMomentTable.AddColumn("内容");

        foreach (var moment in videoMoments.Moments)
        {
            videoMomentTable.AddRow(moment.Id, moment.User.Name, Markup.Escape(moment.Description?.Text ?? moment.MomentType.ToString()));
        }

        AnsiConsole.Write(videoMomentTable);
#endif

            if (AnsiConsole.Confirm("是否返回？"))
            {
                await _backFunc(string.Empty).ConfigureAwait(false);
            }
    }
}
