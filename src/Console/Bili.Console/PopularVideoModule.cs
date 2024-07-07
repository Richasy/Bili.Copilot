using Bili.Console.Models;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Bili.Media;
using Spectre.Console;

namespace Bili.Console;

internal sealed class PopularVideoModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IPopularVideoService _popularVideoService;

    public PopularVideoModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _popularVideoService = kernel.GetRequiredService<IPopularVideoService>();
    }

    public async Task RunAsync()
    {
        var command = AnsiConsole.Prompt(
            new SelectionPrompt<PopularVideoCommand>()
                .Title("请选择流行视频类型")
                .PageSize(10)
                .AddChoices(Enum.GetValues<PopularVideoCommand>())
                .UseConverter(GetCommandName));

        if (command == PopularVideoCommand.Back)
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
        else if (command == PopularVideoCommand.Curated)
        {
            await DisplayCuratedListAsync().ConfigureAwait(false);
        }
        else if (command == PopularVideoCommand.Recommend)
        {
            await DisplayRecommendVideoListAsync().ConfigureAwait(false);
        }
        else if (command == PopularVideoCommand.Hot)
        {
            await DisplayHotVideoListAsync().ConfigureAwait(false);
        }
        else if (command == PopularVideoCommand.Ranking)
        {
            await DisplayRankingListAsync().ConfigureAwait(false);
        }

        static string GetCommandName(PopularVideoCommand command)
        {
            return command switch
            {
                PopularVideoCommand.Curated => "精选视频",
                PopularVideoCommand.Recommend => "推荐视频",
                PopularVideoCommand.Hot => "热门视频",
                PopularVideoCommand.Ranking => "全站排行榜",
                PopularVideoCommand.Back => "返回",
                _ => string.Empty,
            };
        }
    }

    public void Exit()
    {
    }

    public async Task DisplayCuratedListAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("正在获取精选视频列表...");

        var list = await _popularVideoService.GetCuratedPlaylistAsync(_cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("精选视频列表：");

        await RenderVideosAsync(list.ToDictionary(p => p.Identifier.Id, p => p.Identifier.Title!)).ConfigureAwait(false);
    }

    public async Task DisplayRecommendVideoListAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("正在获取推荐视频列表...");

        var (videos, offset) = await _popularVideoService.GetRecommendVideoListAsync(0, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("推荐视频列表：");

        await RenderVideosAsync(videos.ToDictionary(p => p.Identifier.Id, p => p.Identifier.Title!)).ConfigureAwait(false);
    }

    public async Task DisplayHotVideoListAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("正在获取热门视频列表...");

        var (videos, offset) = await _popularVideoService.GetHotVideoListAsync(0, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("热门视频列表：");

        await RenderVideosAsync(videos.ToDictionary(p => p.Identifier.Id, p => p.Identifier.Title!)).ConfigureAwait(false);
    }

    public async Task DisplayRankingListAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("正在获取全站排行榜...");

        var list = await _popularVideoService.GetGlobalRankingListAsync(_cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("全站排行榜：");

        await RenderVideosAsync(list.ToDictionary(p => p.Identifier.Id, p => p.Identifier.Title!)).ConfigureAwait(false);
    }

    private async Task RenderVideosAsync(Dictionary<string, string> videos)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.LeftAligned();
        table.AddColumn("ID");
        table.AddColumn("标题");

        foreach (var item in videos)
        {
            table.AddRow(item.Key, item.Value);
        }

        AnsiConsole.Write(table);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }
}
