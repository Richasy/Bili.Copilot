using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Models;
using Spectre.Console;

namespace Bili.Console;

internal sealed class LivePartitionModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly ILiveDiscoveryService _livePartitionService;

    private IList<Partition> _partitions;

    public LivePartitionModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _livePartitionService = kernel.GetRequiredService<ILiveDiscoveryService>();
    }

    public async Task RunAsync()
    {
        if (_partitions == null)
        {
            var partitions = await _livePartitionService.GetLivePartitionsAsync(_cancellationToken).ConfigureAwait(false);
            _partitions = partitions.ToList();
        }

        var partition = AnsiConsole.Prompt(
            new SelectionPrompt<Partition>()
                .Title("请选择直播分区")
                .PageSize(10)
                .AddChoices(_partitions)
                .UseConverter(p => p.Name));

        var subPartition = AnsiConsole.Prompt(
            new SelectionPrompt<Partition>()
                .Title("请选择子分区")
                .PageSize(10)
                .AddChoices(partition.Children)
                .UseConverter(p => p.Name));

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"正在获取 [green]{subPartition.Name}[/] 直播列表...");
        var (lives, tags, _) = await _livePartitionService.GetPartitionLiveListAsync(subPartition, cancellationToken: _cancellationToken).ConfigureAwait(false);

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"{subPartition.Name} 标签列表：");
        var tagTable = new Table();
        tagTable.AddColumn("ID");
        tagTable.AddColumn("名称");
        tagTable.AddColumn("排序方式");
        foreach (var tag in tags)
        {
            tagTable.AddRow(tag.Id, tag.Name, tag.SortType);
        }

        AnsiConsole.Write(tagTable);

        AnsiConsole.MarkupLine($"{subPartition.Name} 直播列表：");
        var liveTable = new Table();
        liveTable.AddColumn("ID");
        liveTable.AddColumn("标题");
        liveTable.AddColumn("UP主");

        foreach (var live in lives)
        {
            liveTable.AddRow(live.Identifier.Id, live.Identifier.Title, live.User.Name);
        }

        AnsiConsole.Write(liveTable);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(default).ConfigureAwait(false);
        }
    }
}
