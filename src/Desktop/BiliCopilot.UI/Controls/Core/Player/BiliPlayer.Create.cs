// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Controls.Components;
using BiliCopilot.UI.Controls.Danmaku;
using BiliCopilot.UI.Controls.Player;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// Bili 播放器.
/// </summary>
public sealed partial class BiliPlayer
{
    private Func<DanmakuControlBase> _createDanmakuControlFunc;
    private Func<FrameworkElement> _createTransportControlFunc;
    private Func<SubtitlePresenter> _createSubtitleControlFunc;

    private Grid _overlayContainer;
    private Panel _operationContainer;
    private DanmakuControlBase _danmakuControl;
    private FrameworkElement _transportControl;
    private SubtitlePresenter _subtitlePresenter;
    private ProgressBar _progressBar;
    private Rectangle _interactionControl;
    private StackPanel _notificationContainer;
    private LoadingWidget _loadingWidget;
    private EmptyHolder _failedHolder;

    /// <summary>
    /// 注入弹幕控件创建函数.
    /// </summary>
    public void InjectDanmakuControlFunc(Func<DanmakuControlBase> createDanmakuControlFunc)
        => _createDanmakuControlFunc = createDanmakuControlFunc;

    /// <summary>
    /// 注入控制面板创建函数.
    /// </summary>
    public void InjectTransportControlFunc(Func<FrameworkElement> createTransportControlFunc)
        => _createTransportControlFunc = createTransportControlFunc;

    /// <summary>
    /// 注入字幕控件创建函数.
    /// </summary>
    public void InjectSubtitleControlFunc(Func<SubtitlePresenter> createSubtitleControlFunc)
        => _createSubtitleControlFunc = createSubtitleControlFunc;

    private void CreateOverlayContainer()
    {
        var rootGrid = new Grid();
        _danmakuControl = _createDanmakuControlFunc?.Invoke() ?? default;
        if (_danmakuControl is not null)
        {
            _danmakuControl.Margin = new Thickness(3, 4, 12, 12);
            _danmakuControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            _danmakuControl.VerticalAlignment = VerticalAlignment.Stretch;
            rootGrid.Children.Add(_danmakuControl);
        }

        _progressBar = new ProgressBar
        {
            Margin = new Thickness(0, 0, 0, 1),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
            IsIndeterminate = ViewModel?.IsBuffering ?? false,
            Maximum = ViewModel?.Duration ?? 0,
            Value = ViewModel?.Position ?? 0,
            ShowPaused = ViewModel?.IsPaused ?? false,
            Visibility = ViewModel?.IsBottomProgressVisible ?? false ? Visibility.Visible : Visibility.Collapsed,
        };
        rootGrid.Children.Add(_progressBar);

        var contextFlyout = new MenuFlyout();
        contextFlyout.Items.Add(new MenuFlyoutItem
        {
            Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Screenshot),
            Command = ViewModel?.TakeScreenshotCommand,
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Screenshot },
        });
        contextFlyout.Items.Add(new MenuFlyoutItem
        {
            Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Reload),
            Command = ViewModel?.ReloadCommand,
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.ArrowClockwise },
        });
        _interactionControl = new Rectangle
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Fill = new SolidColorBrush(Colors.Transparent),
            IsHoldingEnabled = true,
            Visibility = ViewModel?.IsExternalPlayer ?? false ? Visibility.Collapsed : Visibility.Visible,
            ContextFlyout = contextFlyout,
        };
        HookInteractionControlEvents();
        rootGrid.Children.Add(_interactionControl);

        _notificationContainer = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            Visibility = ViewModel?.IsFailed ?? false ? Visibility.Visible : Visibility.Collapsed,
        };
        rootGrid.Children.Add(_notificationContainer);

        _operationContainer = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
        };

        _subtitlePresenter = _createSubtitleControlFunc?.Invoke() ?? default;
        if (_subtitlePresenter is not null)
        {
            _subtitlePresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
            _operationContainer.Children.Add(_subtitlePresenter);
        }

        _transportControl = _createTransportControlFunc?.Invoke() ?? default;
        if (_transportControl is not null)
        {
            _transportControl.MinHeight = 16;
            _transportControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            _transportControl.VerticalAlignment = VerticalAlignment.Bottom;
            _operationContainer.Children.Add(_transportControl);
        }

        rootGrid.Children.Add(_operationContainer);
        _loadingWidget = new LoadingWidget
        {
            Margin = new Thickness(8),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Visibility = ViewModel?.IsPlayerDataLoading ?? false ? Visibility.Visible : Visibility.Collapsed,
        };
        rootGrid.Children.Add(_loadingWidget);

        _failedHolder = new EmptyHolder
        {
            Title = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.FailedToPlay),
            Description = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.FailedToPlayDescription),
            Emoji = "😫",
            Visibility = ViewModel?.IsFailed ?? false ? Visibility.Visible : Visibility.Collapsed,
            ActionElement = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Command = ViewModel?.ReloadCommand,
                Content = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Reload),
            },
        };
        rootGrid.Children.Add(_failedHolder);
        _overlayContainer = rootGrid;
        HookRootPointerEvents();
    }

    private void HandleViewModelPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsPaused))
        {
            if (_progressBar != null)
            {
                _progressBar.ShowPaused = ViewModel.IsPaused;
            }
        }
        else if (e.PropertyName == nameof(ViewModel.IsBuffering))
        {
            if (_progressBar != null)
            {
                _progressBar.IsIndeterminate = ViewModel.IsBuffering;
            }
        }
        else if (e.PropertyName == nameof(ViewModel.Duration))
        {
            if (_progressBar != null)
            {
                _progressBar.Maximum = ViewModel.Duration;
            }
        }
        else if (e.PropertyName == nameof(ViewModel.Position))
        {
            if (_progressBar != null)
            {
                _progressBar.Value = ViewModel.Position;
            }
        }
        else if (e.PropertyName == nameof(ViewModel.IsBottomProgressVisible))
        {
            if (_progressBar != null)
            {
                _progressBar.Visibility = ViewModel.IsBottomProgressVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        else if (e.PropertyName == nameof(ViewModel.IsFailed))
        {
            if (_notificationContainer != null)
            {
                _notificationContainer.Visibility = ViewModel.IsFailed ? Visibility.Collapsed : Visibility.Visible;
            }

            if (_operationContainer != null)
            {
                _operationContainer.Visibility = ViewModel.IsFailed ? Visibility.Collapsed : Visibility.Visible;
            }

            if (_failedHolder != null)
            {
                _failedHolder.Visibility = ViewModel.IsFailed ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        else if (e.PropertyName == nameof(ViewModel.IsExternalPlayer))
        {
            if (_danmakuControl != null)
            {
                _danmakuControl.Visibility = ViewModel.IsExternalPlayer ? Visibility.Collapsed : Visibility.Visible;
            }

            if (_interactionControl != null)
            {
                _interactionControl.Visibility = ViewModel.IsExternalPlayer ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        else if (e.PropertyName == nameof(ViewModel.IsPlayerDataLoading))
        {
            if (_loadingWidget != null)
            {
                _loadingWidget.Visibility = ViewModel.IsPlayerDataLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
