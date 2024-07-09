using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Models;
using Spectre.Console;

namespace Bili.Console;

internal sealed class VideoPartitionModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IVideoDiscoveryService _videoPartitionService;

    private IList<Partition> _partitions;

    public VideoPartitionModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _videoPartitionService = kernel.GetRequiredService<IVideoDiscoveryService>();
    }

    public async Task RunAsync()
    {
        if (_partitions == null)
        {
            var partitions = await _videoPartitionService.GetVideoPartitionsAsync(_cancellationToken).ConfigureAwait(false);
            _partitions = partitions.ToList();
        }

        var partition = AnsiConsole.Prompt(
            new SelectionPrompt<Partition>()
                .Title("请选择视频分区")
                .PageSize(10)
                .AddChoices(_partitions)
                .UseConverter(p => p.Name));

        AnsiConsole.Clear();

#if RANK
        AnsiConsole.MarkupLine($"正在获取 [green]{partition.Name}[/] 排行榜...");
        var rankingList = await _videoPartitionService.GetPartitionRankingListAsync(partition, _cancellationToken).ConfigureAwait(false);
        
        var list = await _videoPartitionService.GetPartitionRankingListAsync(partition, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"{partition.Name}排行榜：");

        await RenderVideosAsync(list.ToDictionary(p => p.Identifier.Id, p => p.Identifier.Title!)).ConfigureAwait(false);
#endif

#if RECOMMEND
        AnsiConsole.MarkupLine($"正在获取 [green]{partition.Name}[/] 推荐视频...");
        var (videos, offset) = await _videoPartitionService.GetPartitionRecommendVideoListAsync(partition, 0, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"{partition.Name}推荐视频：");
        await RenderVideosAsync(videos.ToDictionary(p => p.Identifier.Id, p => p.Identifier.Title!)).ConfigureAwait(false);
#endif

        var firstChildPartition = partition.Children.FirstOrDefault();
        AnsiConsole.MarkupLine($"正在获取 [green]{firstChildPartition.Name}[/] 子分区视频...");
        var (videos, offset, nextPage) = await _videoPartitionService.GetChildPartitionVideoListAsync(firstChildPartition, sortType: PartitionVideoSortType.Play, cancellationToken: _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"{firstChildPartition.Name}子分区视频：");
        await RenderVideosAsync(videos.ToDictionary(p => p.Identifier.Id, p => p.Identifier.Title!)).ConfigureAwait(false);
    }

    public void Exit()
    {
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
