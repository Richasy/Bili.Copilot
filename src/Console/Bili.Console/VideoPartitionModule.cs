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
    private readonly IVideoPartitionService _videoPartitionService;

    private IList<Partition> _partitions;

    public VideoPartitionModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _videoPartitionService = kernel.GetRequiredService<IVideoPartitionService>();
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

        var rankingList = await _videoPartitionService.GetPartitionRankingListAsync(partition, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"正在获取 [green]{partition.Name}[/] 排行榜...");

        var list = await _videoPartitionService.GetPartitionRankingListAsync(partition, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"{partition.Name}排行榜：");

        await RenderVideosAsync(list.ToDictionary(p => p.Identifier.Id, p => p.Identifier.Title!)).ConfigureAwait(false);
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
