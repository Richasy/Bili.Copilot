// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels.Components;
using Microsoft.UI.Input;
using Windows.UI.Core;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 会话模块.
/// </summary>
public sealed partial class ChatSessionModule : ChatSessionModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionModule"/> class.
    /// </summary>
    public ChatSessionModule()
    {
        InitializeComponent();
        ViewModel = ChatSessionViewModel.Instance;
        Loaded += OnLoadedAsync;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 重置焦点.
    /// </summary>
    public void ResetFocus()
        => InputBox?.Focus(FocusState.Programmatic);

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChatSessionViewModel oldVM)
        {
            oldVM.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        }

        if (e.NewValue == null)
        {
            return;
        }

        var vm = e.NewValue as ChatSessionViewModel;
        vm.RequestScrollToBottom += OnRequestScrollToBottomAsync;
        if (IsLoaded)
        {
            ResetFocus();
        }
    }

    private async void OnRequestScrollToBottomAsync(object sender, EventArgs e)
        => await ScrollToBottomAsync();

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        ResetFocus();
        await ScrollToBottomAsync();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.RequestScrollToBottom -= OnRequestScrollToBottomAsync;

    private void OnInputBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
            if (!isShiftDown)
            {
                e.Handled = true;
                ViewModel.SendMessageCommand.Execute(default);
            }
        }
    }

    private async Task ScrollToBottomAsync()
    {
        await Task.Delay(200);
        _ = MessageViewer.ChangeView(0, MessageViewer.ScrollableHeight + MessageViewer.ActualHeight + MessageViewer.VerticalOffset, default);
    }

    private void OnItemClick(object sender, string e)
        => ViewModel.Input += e;

    private void OnFlyoutClosed(object sender, object e)
        => InputBox.Focus(FocusState.Programmatic);
}

/// <summary>
/// <see cref="ChatSessionModule"/> 的基类.
/// </summary>
public abstract class ChatSessionModuleBase : ReactiveUserControl<ChatSessionViewModel>
{
}


