// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels.Components;

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

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnRequestLoadMore(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="ChatSessionListModule"/> 的基类.
/// </summary>
public abstract class ChatSessionListModuleBase : ReactiveUserControl<ChatSessionListModuleViewModel>
{
}
