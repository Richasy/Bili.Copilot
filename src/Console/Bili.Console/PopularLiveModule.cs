
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel;
using Spectre.Console;

namespace Bili.Console;

internal sealed class PopularLiveModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IPopularLiveService _popularLiveService;

    public PopularLiveModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _popularLiveService = kernel.GetRequiredService<IPopularLiveService>();
    }

    public void Exit()
    {
    }

    public async Task RunAsync()
    {
        AnsiConsole.MarkupLine("正在获取直播信息流...");
        var (follows, recommends, nextPageNumber) = await _popularLiveService.GetFeedAsync(0, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("我关注的");
        var followTable = new Table();
        followTable.AddColumn("ID");
        followTable.AddColumn("标题");
        followTable.AddColumn("主播");

        foreach (var item in follows)
        {
            followTable.AddRow(item.Identifier.Id, item.Identifier.Title, item.User?.Name ?? "N/A");
        }

        AnsiConsole.Write(followTable);

        AnsiConsole.MarkupLine("推荐的");
        var recommendTable = new Table();
        recommendTable.AddColumn("ID");
        recommendTable.AddColumn("标题");
        recommendTable.AddColumn("主播");

        foreach (var item in recommends)
        {
            recommendTable.AddRow(item.Identifier.Id, item.Identifier.Title, item.User?.Name ?? "N/A");
        }

        AnsiConsole.Write(recommendTable);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }
}
