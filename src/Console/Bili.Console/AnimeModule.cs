
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel;
using Spectre.Console;
using Richasy.BiliKernel.Models.Media;

namespace Bili.Console;

internal sealed class AnimeModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IAnimeService _animeService;

    public AnimeModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _animeService = kernel.GetRequiredService<IAnimeService>();
    }

    public async Task RunAsync()
    {
        var animeType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("请选择动漫类型")
                .PageSize(10)
                .AddChoices(["番剧", "国创"]));

        AnsiConsole.Clear();
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

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }

    public void Exit()
    {
    }
}
