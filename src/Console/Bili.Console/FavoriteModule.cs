using Richasy.BiliKernel;
using Richasy.BiliKernel.Bili.User;
using Spectre.Console;

namespace Bili.Console;

internal sealed class FavoriteModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IFavoriteService _favoriteService;

    public FavoriteModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _favoriteService = kernel.GetRequiredService<IFavoriteService>();
    }

    public async Task RunAsync()
    {
        AnsiConsole.MarkupLine("正在获取收藏夹信息");
        var (groups, @default) = await _favoriteService.GetVideoFavoriteGroupsAsync(_cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        var groupTable = new Table();
        groupTable.AddColumn("ID");
        groupTable.AddColumn("标题");
        groupTable.AddColumn("文件夹名称");

        foreach (var group in groups)
        {
            groupTable.AddRow(group.Id.ToString(), group.Title, string.Join('\n', group.Folders.Select(p => p.Title)));
        }

        AnsiConsole.Write(groupTable);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }
        
}
