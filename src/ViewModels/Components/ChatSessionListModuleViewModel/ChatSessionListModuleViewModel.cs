// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.ViewModels.Components;

/// <summary>
/// 聊天会话列表模块视图模型.
/// </summary>
public sealed partial class ChatSessionListModuleViewModel : InformationFlowViewModel<ChatSessionItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionListModuleViewModel"/> class.
    /// </summary>
    private ChatSessionListModuleViewModel()
    {
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        _isEnd = false;
        AccountProvider.Instance.ResetChatSessionOffset();
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => errorMsg;

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_isEnd)
        {
            return;
        }

        var view = await AccountProvider.Instance.GetChatSessionsAsync();
        foreach (var item in view.Sessions)
        {
            Items.Add(new ChatSessionItemViewModel(item));
        }

        _isEnd = !view.HasMore;
        IsEmpty = Items.Count == 0;
    }
}
