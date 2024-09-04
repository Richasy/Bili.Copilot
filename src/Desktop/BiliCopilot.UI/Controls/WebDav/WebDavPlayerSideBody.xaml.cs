﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;

namespace BiliCopilot.UI.Controls.WebDav;

/// <summary>
/// WebDAV 播放器侧边栏主体.
/// </summary>
public sealed partial class WebDavPlayerSideBody : WebDavPlayerPageControlBase
{
    private long _viewModelChangedToken;
    private WebDavPlayerPageViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPlayerSideBody"/> class.
    /// </summary>
    public WebDavPlayerSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.VideoSelectionChanged += OnViewModelVideoSelectionChangedAsync;
        await CheckSelectedItemAsync();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.VideoSelectionChanged -= OnViewModelVideoSelectionChangedAsync;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.VideoSelectionChanged -= OnViewModelVideoSelectionChangedAsync;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.VideoSelectionChanged += OnViewModelVideoSelectionChangedAsync;
    }

    private async void OnViewModelVideoSelectionChangedAsync(object? sender, EventArgs e)
        => await CheckSelectedItemAsync();

    private void OnVideoSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as WebDavStorageItemViewModel;
        if (item is not null && ViewModel.Current != item)
        {
            ViewModel.InitializeCommand.Execute(item.Data);
        }
    }

    private async Task CheckSelectedItemAsync()
    {
        await Task.Delay(100);
        if (ViewModel.Current is null || View.ActualWidth < 50)
        {
            return;
        }

        var index = ViewModel.Playlist.ToList().IndexOf(ViewModel.Current);
        View.Select(index);
        View.StartBringItemIntoView(index, new BringIntoViewOptions { VerticalAlignmentRatio = 0.5 });
    }

    private async void OnSizeChangedAsync(object sender, SizeChangedEventArgs e)
    {
        var old = e.PreviousSize.Width;
        var current = e.NewSize.Width;

        // 从折叠到显示，需要刷新选中项
        if (current - old > 100)
        {
            await CheckSelectedItemAsync();
        }
    }
}
