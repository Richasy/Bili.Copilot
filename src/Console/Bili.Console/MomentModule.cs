using Richasy.BiliKernel;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Models.User;
using Spectre.Console;

namespace Bili.Console;

internal sealed class MomentModule : IFeatureModule
{
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IMomentDiscoveryService _momentService;

    public MomentModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _momentService = kernel.GetRequiredService<IMomentDiscoveryService>();
    }

    public async Task RunAsync()
    {
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

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }

    public void Exit()
    {
    }
}
