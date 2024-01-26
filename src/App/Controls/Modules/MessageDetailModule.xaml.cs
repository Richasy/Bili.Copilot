// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Components;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 消息详情模块.
/// </summary>
public sealed partial class MessageDetailModule : MessageDetailModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDetailModule"/> class.
    /// </summary>
    public MessageDetailModule() => InitializeComponent();

    private void OnIncrementalTriggered(object sender, System.EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnHeaderItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as MessageHeaderViewModel;
        if (data != ViewModel.CurrentType || ViewModel.IsInChatSession)
        {
            ViewModel.SelectTypeCommand.Execute(data);
        }
    }

    private void OnSessionItemClick(object sender, ViewModels.Items.ChatSessionItemViewModel e)
    {
        ViewModel.EnterChatSessionCommand.Execute(default);
        ChatSessionViewModel.Instance.InitializeCommand.Execute(e.User);
    }
}

/// <summary>
/// <see cref="MessageDetailModule"/> 的基类.
/// </summary>
public abstract class MessageDetailModuleBase : ReactiveUserControl<MessageDetailViewModel>
{
}
