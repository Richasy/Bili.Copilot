// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 下载按钮.
/// </summary>
public sealed partial class DownloadButton : DownloadButtonBase
{
    private long _viewModelChangedToken;
    private DownloadViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadButton"/> class.
    /// </summary>
    public DownloadButton() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.MetaInitialized += OnMetaInitialized;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.MetaInitialized -= OnMetaInitialized;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.MetaInitialized -= OnMetaInitialized;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.MetaInitialized += OnMetaInitialized;
        DownloadFlyout.Items.Clear();
    }

    private void OnMetaInitialized(object? sender, EventArgs e)
        => DownloadFlyout.Items.Clear();

    private void InitializeDownloadFlyout()
    {
        if (ViewModel is null || ViewModel.Formats is null)
        {
            return;
        }

        var formatHeader = MenuFlyoutItemHeader.LoadContent() as MenuFlyoutSeparator;
        formatHeader.Tag = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Quality);
        DownloadFlyout.Items.Add(formatHeader);
        foreach (var item in ViewModel.Formats)
        {
            var menuItem = new MenuFlyoutItem()
            {
                Text = item.Description,
                MinWidth = this.ActualWidth,
                Command = ViewModel.DownloadWithFormatCommand,
                CommandParameter = item,
            };
            DownloadFlyout.Items.Add(menuItem);
        }

        if (ViewModel.Parts is not null || ViewModel.Episodes is not null)
        {
            DownloadFlyout.Items.Add(new MenuFlyoutSeparator());
            var partHeader = MenuFlyoutItemHeader.LoadContent() as MenuFlyoutSeparator;
            partHeader.Tag = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.BatchDownload);
            var allPartsItem = new MenuFlyoutItem
            {
                MinWidth = this.ActualWidth,
                Command = ViewModel.BatchDownloadAllPartsCommand,
                Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.DownloadAllParts),
            };
            var selectPartsItem = new MenuFlyoutItem
            {
                MinWidth = this.ActualWidth,
                Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.SelectPartsDownload),
            };
            selectPartsItem.Click += (_, _) => ShowPartsSelectionTip();
            DownloadFlyout.Items.Add(partHeader);
            DownloadFlyout.Items.Add(allPartsItem);
            DownloadFlyout.Items.Add(selectPartsItem);
        }

        DownloadFlyout.Items.Add(new MenuFlyoutSeparator());
        var extraHeader = MenuFlyoutItemHeader.LoadContent() as MenuFlyoutSeparator;
        extraHeader.Tag = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Extra);
        DownloadFlyout.Items.Add(extraHeader);
        var downloadCoverItem = new MenuFlyoutItem
        {
            MinWidth = this.ActualWidth,
            Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.DownloadCover),
            Command = ViewModel.DownloadCoverCommand,
        };
        var downloadDanmakuItem = new MenuFlyoutItem
        {
            MinWidth = this.ActualWidth,
            Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.DownloadDanmaku),
            Command = ViewModel.DownloadDanmakuCommand,
        };

        DownloadFlyout.Items.Add(downloadCoverItem);
        DownloadFlyout.Items.Add(downloadDanmakuItem);

        if (ViewModel.HasAvailableSubtitle)
        {
            var downloadSubtitleItem = new MenuFlyoutItem
            {
                MinWidth = this.ActualWidth,
                Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.DownloadSubtitle),
                Command = ViewModel.DownloadSubtitleCommand,
            };

            DownloadFlyout.Items.Add(downloadSubtitleItem);
        }
    }

    private void ShowPartsSelectionTip()
    {
        SelectionBox.Text = string.Empty;
        SelectionTip.IsOpen = true;
    }

    private void OnSelectionSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var text = args.QueryText;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        SelectionTip.IsOpen = false;
        SelectionBox.Text = string.Empty;
        ViewModel.BatchDownloadSelectedPartsCommand.Execute(text);
    }

    private void OnBtnClick(object sender, RoutedEventArgs e)
    {
        if (DownloadFlyout.Items.Count == 0)
        {
            InitializeDownloadFlyout();
        }
    }
}

/// <summary>
/// 下载按钮基类.
/// </summary>
public abstract class DownloadButtonBase : LayoutUserControlBase<DownloadViewModel>
{
}
