// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 直播聊天区.
/// </summary>
public sealed partial class LiveChatSection : LiveChatSectionBase
{
    private long _viewModelChangedToken;
    private LiveChatSectionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveChatSection"/> class.
    /// </summary>
    public LiveChatSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.ScrollToBottomRequested += OnScrollToBottomRequested;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ScrollToBottomRequested -= OnScrollToBottomRequested;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.ScrollToBottomRequested -= OnScrollToBottomRequested;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.ScrollToBottomRequested += OnScrollToBottomRequested;
    }

    private void OnScrollToBottomRequested(object? sender, EventArgs e)
    {
        if (View.ScrollView.VerticalOffset + View.ScrollView.ViewportHeight >= View.ScrollView.ExtentHeight - 50)
        {
            View.ScrollView.ScrollTo(0, View.ScrollView.ExtentHeight + View.ScrollView.ViewportHeight + 50, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
        }
    }
}

/// <summary>
/// 直播聊天区基类.
/// </summary>
public abstract class LiveChatSectionBase : LayoutUserControlBase<LiveChatSectionDetailViewModel>
{
}
