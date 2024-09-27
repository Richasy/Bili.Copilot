// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using System.Windows.Input;
using BiliCopilot.UI.Controls.Components;
using BiliCopilot.UI.Controls.Danmaku;
using BiliCopilot.UI.Controls.Player;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.System;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// Bili 播放器.
/// </summary>
public sealed partial class BiliPlayer
{
    private Func<DanmakuControlBase> _createDanmakuControlFunc;
    private Func<FrameworkElement> _createTransportControlFunc;
    private Func<SubtitlePresenter> _createSubtitleControlFunc;

    private CursorGrid _overlayContainer;
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
        var rootGrid = new CursorGrid();

        if (!ViewModel.IsExternalPlayer)
        {
            _danmakuControl = _createDanmakuControlFunc?.Invoke() ?? default;
        }

        if (_danmakuControl is not null)
        {
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

        if (!ViewModel.IsExternalPlayer)
        {
            var contextFlyout = new MenuFlyout();
            contextFlyout.Items.Add(new MenuFlyoutItem
            {
                Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Screenshot),
                Command = ViewModel?.TakeScreenshotCommand,
                MinWidth = 160,
                Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Screenshot },
            });
            contextFlyout.Items.Add(new MenuFlyoutItem
            {
                Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Reload),
                Command = ViewModel?.ReloadCommand,
                MinWidth = 160,
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
        }

        _notificationContainer = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            Visibility = ViewModel?.IsFailed ?? false ? Visibility.Visible : Visibility.Collapsed,
        };
        rootGrid.Children.Add(_notificationContainer);

        if (!ViewModel.IsExternalPlayer)
        {
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
        }

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
        if (ViewModel is IslandPlayerViewModel)
        {
            rootGrid.Children.Insert(0, CreateHiddenControlsContainer());
        }

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

            if (_needRemeasureSize && PlayerPresenter?.TryGetIslandPlayer() is IslandPlayer islandPlayer)
            {
                islandPlayer.ResetWindowPosition();
            }

            _needRemeasureSize = false;
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

    private Grid CreateHiddenControlsContainer()
    {
        var grid = new Grid();
        var increaseVolumeBtn = CreateHiddenButton(VirtualKey.Up, ViewModel.IncreaseVolumeCommand);
        var decreaseVolumeBtn = CreateHiddenButton(VirtualKey.Down, ViewModel.DecreaseVolumeCommand);
        var forwardBtn = CreateHiddenButton(VirtualKey.Right, ViewModel.ForwardSkipCommand);
        var backwardBtn = CreateHiddenButton(VirtualKey.Left, ViewModel.BackwardSkipCommand);
        var miniBtn = CreateHiddenButton(VirtualKey.F9, ViewModel.ToggleCompactOverlayCommand);
        var fullBtn = CreateHiddenButton(VirtualKey.F11, ViewModel.ToggleFullScreenCommand);
        var fullWindowBtn = CreateHiddenButton(VirtualKey.F10, ViewModel.ToggleFullWindowCommand);
        var backBtn = CreateHiddenButton(VirtualKey.Escape, ViewModel.BackToDefaultModeCommand);
        grid.Children.Add(increaseVolumeBtn);
        grid.Children.Add(decreaseVolumeBtn);
        grid.Children.Add(forwardBtn);
        grid.Children.Add(backwardBtn);
        grid.Children.Add(miniBtn);
        grid.Children.Add(fullBtn);
        grid.Children.Add(fullWindowBtn);
        grid.Children.Add(backBtn);

        Button CreateHiddenButton(VirtualKey key, ICommand command, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
        {
            var accelerator = new KeyboardAccelerator();
            accelerator.Key = key;
            accelerator.Modifiers = modifiers;
            accelerator.IsEnabled = true;

            var btn = new Button
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Width = 1,
                Height = 1,
                BorderThickness = new Thickness(0),
                IsTabStop = false,
                TabFocusNavigation = Microsoft.UI.Xaml.Input.KeyboardNavigationMode.Once,
                TabNavigation = Microsoft.UI.Xaml.Input.KeyboardNavigationMode.Once,
                XYFocusKeyboardNavigation = Microsoft.UI.Xaml.Input.XYFocusKeyboardNavigationMode.Disabled,
                Command = command,
            };

            btn.KeyboardAccelerators.Add(accelerator);
            return btn;
        }

        return grid;
    }
}

/// <summary>
/// 光标网格.
/// </summary>
public partial class CursorGrid : Grid
{
    /// <summary>
    /// 隐藏光标.
    /// </summary>
    public void HideCursor()
        => ProtectedCursor?.Dispose();

    /// <summary>
    /// 显示光标.
    /// </summary>
    public void ShowCursor()
        => ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
}
