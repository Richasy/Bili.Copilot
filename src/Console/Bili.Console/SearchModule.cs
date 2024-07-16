using Richasy.BiliKernel;
using Richasy.BiliKernel.Bili.Search;
using Spectre.Console;

namespace Bili.Console;

internal sealed class SearchModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly ISearchService _searchService;

    public SearchModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _searchService = kernel.GetRequiredService<ISearchService>();
    }

    public async Task RunAsync()
    {
        AnsiConsole.MarkupLine("正在获取热搜信息");
        var hotSearchItems = await _searchService.GetTotalHotSearchAsync(_cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        var hotSearchTable = new Table();
        hotSearchTable.AddColumn("排名");
        hotSearchTable.AddColumn("标题");

        foreach (var item in hotSearchItems)
        {
            hotSearchTable.AddRow(item.Index.ToString(), item.Text);
        }

        AnsiConsole.Write(hotSearchTable);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }
}
