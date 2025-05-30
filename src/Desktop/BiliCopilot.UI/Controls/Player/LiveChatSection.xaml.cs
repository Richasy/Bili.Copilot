// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 直播聊天区.
/// </summary>
public sealed partial class LiveChatSection : LiveChatSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveChatSection"/> class.
    /// </summary>
    public LiveChatSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        View.ItemsSource = default;
        if (ViewModel is not null)
        {
            ViewModel.ScrollToBottomRequested -= OnScrollToBottomRequested;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(LiveChatSectionDetailViewModel? oldValue, LiveChatSectionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ScrollToBottomRequested -= OnScrollToBottomRequested;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ScrollToBottomRequested += OnScrollToBottomRequested;
    }

    private void OnScrollToBottomRequested(object? sender, EventArgs e)
    {
        if (View.ScrollView is null)
        {
            return;
        }

        if (View.ScrollView.VerticalOffset + View.ScrollView.ViewportHeight >= View.ScrollView.ExtentHeight - 50)
        {
            View.ScrollView.ScrollTo(0, View.ScrollView.ExtentHeight + View.ScrollView.ViewportHeight + 50, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
        }
    }

    private void OnDanmakuSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (string.IsNullOrEmpty(args.QueryText))
        {
            return;
        }

        ViewModel.SendDanmakuCommand.Execute(args.QueryText);
        sender.Text = string.Empty;
    }
}

/// <summary>
/// 直播聊天区基类.
/// </summary>
public abstract class LiveChatSectionBase : LayoutUserControlBase<LiveChatSectionDetailViewModel>
{
}
