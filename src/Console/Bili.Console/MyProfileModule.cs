
using System.Diagnostics;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using Spectre.Console;

namespace Bili.Console;

internal sealed class MyProfileModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;

    private UserDetailProfile? _myInfo;
    private UserCommunityInformation? _myCommunityInfo;

    public MyProfileModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
    }

    public async Task RunAsync()
    {
        var profileService = _kernel.GetRequiredService<IMyProfileService>();

        try
        {
            await DisplayUserInformationAsync().ConfigureAwait(false);

            var command = AnsiConsole.Prompt(
                new SelectionPrompt<MyProfileCommand>()
                    .Title("请选择操作")
                    .PageSize(10)
                    .AddChoices(MyProfileCommand.MyFollow, MyProfileCommand.MyFans, MyProfileCommand.ViewLater, MyProfileCommand.Back)
                    .UseConverter(GetCommandName));

            if (command == MyProfileCommand.Back)
            {
                await _backFunc(string.Empty).ConfigureAwait(false);
            }
            else if (command == MyProfileCommand.MyFollow)
            {
                await DisplayFollowGroupAsync().ConfigureAwait(false);
            }
            else if (command == MyProfileCommand.MyFans)
            {
                await DisplayFansAsync().ConfigureAwait(false);
            }
            else if (command == MyProfileCommand.ViewLater)
            {
                await DisplayViewLaterAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await _backFunc("[bold red]用户数据模块出现异常[/]").ConfigureAwait(false);
        }

        static string GetCommandName(MyProfileCommand command)
        {
            return command switch
            {
                MyProfileCommand.MyFollow => "我的关注",
                MyProfileCommand.MyFans => "我的粉丝",
                MyProfileCommand.ViewLater => "稍后再看",
                MyProfileCommand.Back => "返回",
                _ => string.Empty,
            };
        }
    }

    public void Exit()
    {
        // Do nothing.
    }

    private async Task DisplayUserInformationAsync()
    {
        var profileService = _kernel.GetRequiredService<IMyProfileService>();
        _myInfo ??= await profileService.GetMyProfileAsync(_cancellationToken).ConfigureAwait(false);
        _myCommunityInfo ??= await profileService.GetMyCommunityInformationAsync(_cancellationToken).ConfigureAwait(false);

        AnsiConsole.MarkupLine($"你好，[bold yellow]{_myInfo.User.Name}[/](Lv.{_myInfo.Level})");
        AnsiConsole.MarkupLine($"UID: [bold green]{_myInfo.User.Id}[/]");

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.LeftAligned();
        table.AddColumn("关注数");
        table.AddColumn("粉丝数");
        table.AddColumn("动态数");
        table.AddColumn("硬币数");

        table.AddRow(
            _myCommunityInfo.FollowCount.ToString(),
            _myCommunityInfo.FansCount.ToString(),
            _myCommunityInfo.DynamicCount.ToString(),
            _myCommunityInfo.CoinCount.ToString());

        AnsiConsole.Write(table);
    }

    private async Task DisplayFollowGroupAsync()
    {
        var profileService = _kernel.GetRequiredService<IRelationshipService>();
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold]正在获取关注分组信息...[/]");
        var groups = await profileService.GetMyFollowUserGroupsAsync(_cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold]关注分组:[/]");
        var group = AnsiConsole.Prompt(
            new SelectionPrompt<UserGroup>()
                .Title("请选择分组")
                .PageSize(10)
                .AddChoices(groups)
                .UseConverter(p => p.Name));
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"正在获取分组[green] {group.Name} [/]的详细信息...");
        var users = await profileService.GetMyFollowUserGroupDetailAsync(group.Id, 1, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold]分组[green] {group.Name} [/]的详细信息:[/]");
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.LeftAligned();
        table.AddColumn("UID");
        table.AddColumn("昵称");

        foreach (var user in users)
        {
            table.AddRow(user.Profile.User.Id.ToString(), user.Profile.User.Name);
        }

        AnsiConsole.Write(table);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }

    private async Task DisplayFansAsync()
    {
        var profileService = _kernel.GetRequiredService<IRelationshipService>();
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold]正在获取粉丝列表...[/]");

        var (users, count) = await profileService.GetMyFansAsync(1, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold]共有[green] {count} [/]位粉丝[/]");
        AnsiConsole.MarkupLine($"[bold]粉丝列表 ({users.Count}):[/]");

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.LeftAligned();
        table.AddColumn("UID");
        table.AddColumn("昵称");
        foreach (var user in users)
        {
            table.AddRow(user.Profile.User.Id.ToString(), user.Profile.User.Name);
        }
        AnsiConsole.Write(table);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }

    private async Task DisplayViewLaterAsync()
    {
        var profileService = _kernel.GetRequiredService<IViewLaterService>();
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold]正在获取稍后再看列表...[/]");

        var (videos, count) = await profileService.GetViewLaterSetAsync(1, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold]共有[green] {count} [/]个稍后再看视频[/]");
        AnsiConsole.MarkupLine($"[bold]稍后再看列表 ({videos.Count}):[/]");

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.LeftAligned();
        table.AddColumn("AV号");
        table.AddColumn("标题");
        foreach (var video in videos)
        {
            table.AddRow(video.Identifier.Id, video.Identifier.Title);
        }
        AnsiConsole.Write(table);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }
}

internal enum MyProfileCommand
{
    MyFollow,
    MyFans,
    ViewLater,
    Back,
}
