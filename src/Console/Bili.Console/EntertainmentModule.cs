
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel;
using Spectre.Console;
using Richasy.BiliKernel.Models;

namespace Bili.Console;

internal sealed class EntertainmentModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IEntertainmentDiscoveryService _entertainmentService;

    public EntertainmentModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _entertainmentService = kernel.GetRequiredService<IEntertainmentDiscoveryService>();
    }

    public async Task RunAsync()
    {
        var entertainmentType = AnsiConsole.Prompt(
            new SelectionPrompt<EntertainmentType>()
                .Title("请选择类型")
                .PageSize(10)
                .AddChoices(Enum.GetValues<EntertainmentType>().Where(p => p is not EntertainmentType.Anime))
                .UseConverter(GetTypeName));

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"正在获取 {entertainmentType} 的筛选条件");
        var filters = await _entertainmentService.GetIndexFiltersAsync(entertainmentType, _cancellationToken).ConfigureAwait(false);
        var filterTable = new Table();
        filterTable.AddColumn("名称");
        filterTable.AddColumn("值列表");

        foreach (var filter in filters)
        {
            var valueList = string.Join(", ", filter.Conditions.Select(p => p.Name));
            filterTable.AddRow(filter.Name, valueList);
        }

        AnsiConsole.Write(filterTable);

        AnsiConsole.WriteLine($"正在获取 {entertainmentType} 的索引数据");
        var (seasons, hasNext) = await _entertainmentService.GetIndexSeasonsWithFiltersAsync(entertainmentType, null, 0, _cancellationToken).ConfigureAwait(false);
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


        static string GetTypeName(EntertainmentType type)
        {
            return type switch
            {
                EntertainmentType.Movie => "电影",
                EntertainmentType.TV => "电视剧",
                EntertainmentType.Documentary => "纪录片",
                _ => "未知",
            };
        }
    }
}
