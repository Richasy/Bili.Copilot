// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels.Components;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 会话列表.
/// </summary>
public sealed partial class ChatSessionListModule : ChatSessionListModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionListModule"/> class.
    /// </summary>
    public ChatSessionListModule()
    {
        InitializeComponent();
        ViewModel = ChatSessionListModuleViewModel.Instance;
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 会话项点击事件.
    /// </summary>
    public event EventHandler<ChatSessionItemViewModel> SessionItemClick;

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnRequestLoadMore(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnSessionItemClick(object sender, RoutedEventArgs e)
    {
        var data = (e.OriginalSource as FrameworkElement)?.DataContext as ChatSessionItemViewModel;
        if (data is not null)
        {
            foreach (var item in ViewModel.Items)
            {
                item.IsSelected = item.Equals(data);
            }

            SessionItemClick?.Invoke(this, data);
        }
    }
}

/// <summary>
/// <see cref="ChatSessionListModule"/> 的基类.
/// </summary>
public abstract class ChatSessionListModuleBase : ReactiveUserControl<ChatSessionListModuleViewModel>
{
}
