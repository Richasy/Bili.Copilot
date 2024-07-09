using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel;
using Spectre.Console;

namespace Bili.Console;

internal sealed class AnimeModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IEntertainmentDiscoveryService _animeService;

    public AnimeModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _animeService = kernel.GetRequiredService<IEntertainmentDiscoveryService>();
    }

    public async Task RunAsync()
    {
        var animeType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("请选择动漫类型")
                .PageSize(10)
                .AddChoices(["番剧", "国创"]));

        AnsiConsole.Clear();

#if TIMELINE
        AnsiConsole.MarkupLine($"正在获取 {animeType} 的时间线");

        var (title, desc, items) = animeType == "番剧"
            ? await _animeService.GetBangumiTimelineAsync(_cancellationToken).ConfigureAwait(false)
            : await _animeService.GetDomesticTimelineAsync(_cancellationToken).ConfigureAwait(false);

        AnsiConsole.MarkupLine(title);
        AnsiConsole.MarkupLine(desc);
        var table = new Table();
        table.AddColumn("日期");
        table.AddColumn("列表");

        foreach (var item in items)
        {
            var itemTable = new Table();
            itemTable.AddColumn("标题");
            itemTable.AddColumn("状态");

            if (item.Seasons != null)
            {
                foreach (var subItem in item.Seasons)
                {
                    var status = subItem.GetExtensionIfNotNull<string>(SeasonExtensionDataId.Subtitle);
                    itemTable.AddRow(subItem.Identifier.Title, status);
                }
            }

            table.AddRow(new Markup(item.Date), itemTable);
        }

        AnsiConsole.Write(table);

#endif
        AnsiConsole.MarkupLine($"正在获取 {animeType} 的筛选条件");
        var filters = await _animeService.GetIndexFiltersAsync(Richasy.BiliKernel.Models.EntertainmentType.Anime, _cancellationToken).ConfigureAwait(false);
        var filterTable = new Table();
        filterTable.AddColumn("名称");
        filterTable.AddColumn("值列表");

        foreach (var filter in filters)
        {
            var valueList = string.Join(", ", filter.Conditions.Select(p => p.Name));
            filterTable.AddRow(filter.Name, valueList);
        }

        AnsiConsole.Write(filterTable);

        AnsiConsole.WriteLine($"正在获取 {animeType} 的索引数据");
        var (seasons, hasNext) = await _animeService.GetIndexSeasonsWithFiltersAsync(Richasy.BiliKernel.Models.EntertainmentType.Anime, null, 0, _cancellationToken).ConfigureAwait(false);
        var seasonTable = new Table();
        seasonTable.AddColumn("ID");
        seasonTable.AddColumn("标题");

        foreach (var item in seasons)
        {
            seasonTable.AddRow(item.Identifier.Id, item.Identifier.Title);
        }

        AnsiConsole.Write(seasonTable);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }

    public void Exit()
    {
    }
}
