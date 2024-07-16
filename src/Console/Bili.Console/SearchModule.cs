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
#if BASIC
        AnsiConsole.MarkupLine("正在获取热搜信息");
        var hotSearchItems = await _searchService.GetTotalHotSearchAsync(cancellationToken: _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        var hotSearchTable = new Table();
        hotSearchTable.AddColumn("排名");
        hotSearchTable.AddColumn("标题");

        foreach (var item in hotSearchItems)
        {
            hotSearchTable.AddRow(item.Index.ToString(), item.Text);
        }

        AnsiConsole.Write(hotSearchTable);

        AnsiConsole.MarkupLine("正在获取搜索推荐");
        var searchRecommendItems = await _searchService.GetSearchRecommendsAsync(cancellationToken: _cancellationToken).ConfigureAwait(false);
        var searchRecommendTable = new Table();
        searchRecommendTable.AddColumn("索引");
        searchRecommendTable.AddColumn("标题");

        foreach (var item in searchRecommendItems)
        {
            searchRecommendTable.AddRow(item.Index.ToString(), item.Keyword);
        }

        AnsiConsole.Write(searchRecommendTable);
#endif

        var keyword = AnsiConsole.Ask<string>("请输入搜索关键字：");
        var (videos, partitions, nextOffset) = await _searchService.GetComprehensiveSearchResultAsync(keyword, cancellationToken: _cancellationToken).ConfigureAwait(false);
        var partitionTable = new Table();
        partitionTable.AddColumn("名称");
        partitionTable.AddColumn("总个数");

        foreach (var partition in partitions)
        {
            partitionTable.AddRow(partition.Title, partition.TotalItemCount.ToString());
        }

        AnsiConsole.Write(partitionTable);

        AnsiConsole.MarkupLine("视频列表");
        var videoTable = new Table();
        videoTable.AddColumn("ID");
        videoTable.AddColumn("标题");
        videoTable.AddColumn("UP主");

        foreach (var video in videos)
        {
            videoTable.AddRow(video.Identifier.Id, Markup.Escape(video.Identifier.Title), Markup.Escape(video.Publisher.User.Name));
        }

        AnsiConsole.Write(videoTable);

        var firstPartition = partitions.FirstOrDefault(p => p.TotalItemCount > 0);
        if (firstPartition is not null)
        {
            var (subItems, subOffset) = await _searchService.GetPartitionSearchResultAsync(keyword, firstPartition, cancellationToken: _cancellationToken).ConfigureAwait(false);
        }


        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }
}
