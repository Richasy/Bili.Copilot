using Richasy.BiliKernel;
using Richasy.BiliKernel.Bili.User;
using Spectre.Console;

namespace Bili.Console;

internal sealed class MessageModule : IFeatureModule
{
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IMessageService _messageService;

    public MessageModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _messageService = kernel.GetRequiredService<IMessageService>();
    }

    public async Task RunAsync()
    {
        AnsiConsole.MarkupLine("正在获取未读信息");
        var unreadMessage = await _messageService.GetUnreadInformationAsync(_cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        var unreadMessageTable = new Table();
        unreadMessageTable.AddColumn("@我的");
        unreadMessageTable.AddColumn("回复");
        unreadMessageTable.AddColumn("点赞");
        unreadMessageTable.AddColumn("私信");

        unreadMessageTable.AddRow(
            unreadMessage.AtCount.ToString("N0"),
            unreadMessage.ReplyCount.ToString("N0"),
            unreadMessage.LikeCount.ToString("N0"),
            unreadMessage.ChatCount.ToString("N0"));

        AnsiConsole.Write(unreadMessageTable);

        var (sessions, hasNext) = await _messageService.GetChatSessionsAsync(cancellationToken: _cancellationToken).ConfigureAwait(false);
        var chatSessionTable = new Table();
        chatSessionTable.AddColumn("ID");
        chatSessionTable.AddColumn("用户");
        chatSessionTable.AddColumn("最近消息");

        foreach (var session in sessions)
        {
            chatSessionTable.AddRow(session.User.Id, Markup.Escape(session.User.Name), Markup.Escape(session.LastMessage ?? "N/A"));
        }

        AnsiConsole.Write(chatSessionTable);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }

    public void Exit()
    {
    }
}
