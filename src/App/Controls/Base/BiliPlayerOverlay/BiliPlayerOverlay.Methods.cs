﻿// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Danmaku;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using Microsoft.UI.Input;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器覆盖层.
/// </summary>
public partial class BiliPlayerOverlay
{
    private static bool IsManualMode()
        => SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerControlModeManual, false);

    private void InitializeDanmakuTimer()
    {
        if (_danmakuTimer == null)
        {
            _danmakuTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.5),
            };
            _danmakuTimer.Tick += OnDanmkuTimerTick;
        }
    }

    private void InitializeUnitTimer()
    {
        if (_unitTimer == null)
        {
            _unitTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.5),
            };
            _unitTimer.Tick += OnUnitTimerTick;
        }
    }

    private void InitializeDanmaku(IEnumerable<DanmakuInformation> elements)
    {
        var list = new List<DanmakuModel>();
        foreach (var item in elements)
        {
            var location = DanmakuLocation.Scroll;
            if (item.Mode == 4)
            {
                location = DanmakuLocation.Bottom;
            }
            else if (item.Mode == 5)
            {
                location = DanmakuLocation.Top;
            }

            var newDm = new DanmakuModel()
            {
                Color = AppToolkit.HexToColor(item.Color.ToString()),
                Location = location,
                Id = item.Id,
                Size = item.FontSize,
                Text = item.Content,
                Time = Convert.ToInt32(item.StartPosition),
            };

            list.Add(newDm);
        }

        list = list.GroupBy(p => p.Id)
            .Select(g => g.First())
            .ToList();
        var group = list.GroupBy(p => p.Time).ToDictionary(x => x.Key, x => x.ToList());
        foreach (var g in group)
        {
            if (_danmakuDictionary.ContainsKey(g.Key))
            {
                _danmakuDictionary[g.Key] = g.Value.ToList();
            }
            else
            {
                _danmakuDictionary.Add(g.Key, g.Value);
            }
        }
    }

    private void CheckDanmakuZoom()
    {
        if (ActualWidth == 0 || ActualHeight == 0 || _danmakuView == null)
        {
            return;
        }

        var baseWidth = 800d;
        var baseHeight = 600d;
        var scale = Math.Min(ActualWidth / baseWidth, ActualHeight / baseHeight);
        if (scale > 1)
        {
            scale = 1;
        }
        else if (scale < 0.4)
        {
            scale = 0.4;
        }

        scale *= ViewModel.DanmakuViewModel.DanmakuZoom;
        _danmakuView.DanmakuSizeZoom = scale;
    }

    private void ShowTempMessage(string msg)
    {
        _tempMessageContainer.Visibility = string.IsNullOrEmpty(msg)
            ? Visibility.Collapsed
            : Visibility.Visible;

        if (!string.IsNullOrEmpty(msg))
        {
            _tempMessageBlock.Text = msg;
            _tempMessageStayTime = 0;
        }
        else
        {
            _tempMessageStayTime = -1;
        }
    }

    private void HideTempMessage()
    {
        _tempMessageContainer.Visibility = Visibility.Collapsed;
        _tempMessageBlock.Text = string.Empty;
        _tempMessageStayTime = -1;
    }

    private void HandleTransportAutoHide()
    {
        if (_transportStayTime > 1.2 && ViewModel.Player != null)
        {
            _transportStayTime = 0;
            var isManual = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerControlModeManual, false);
            if (isManual || ViewModel.Player.Status == PlayerStatus.Pause)
            {
                return;
            }

            if (!_transportControls.IsDanmakuBoxFocused
            && (_isTouch || !_transportControls.IsPointerStay || !IsPointerStay))
            {
                if (_isForceHiddenTransportControls)
                {
                    ViewModel.IsShowMediaTransport = true;
                    _isForceHiddenTransportControls = false;
                }

                ViewModel.IsShowMediaTransport = false;
            }
        }
    }

    private void HandleCursorAutoHide()
    {
        if (_cursorStayTime > 1.5
            && !ViewModel.IsShowMediaTransport
            && IsPointerStay
            && !_rootSplitView.IsPaneOpen
            && ViewModel.Player != null
            && ViewModel.Player.Status == PlayerStatus.Playing)
        {
            ProtectedCursor.Dispose();
            _cursorStayTime = 0;
        }
    }

    private void HandleTempMessageAutoHide()
    {
        if (_tempMessageStayTime >= 2)
        {
            HideTempMessage();
        }
    }

    private void HandleNextVideoAutoHide()
    {
        if (_nextVideoStayTime > 5)
        {
            _nextVideoStayTime = 0;
            ViewModel.NextVideoCountdown = 0;
            ViewModel.IsShowNextVideoTip = false;
            ViewModel.PlayNextVideoCommand.Execute(default);
        }
        else
        {
            ViewModel.NextVideoCountdown = Math.Ceiling(5 - _nextVideoStayTime);
        }
    }

    private void HandleProgressTipAutoHide()
    {
        if (_progressTipStayTime > 5)
        {
            _progressTipStayTime = 0;
            ViewModel.ProgressTipCountdown = 0;
            ViewModel.IsShowProgressTip = false;
        }
        else
        {
            ViewModel.ProgressTipCountdown = Math.Ceiling(5 - _progressTipStayTime);
        }
    }

    private void HandleAutoCloseWindowAutoHide()
    {
        if (_autoCloseWindowStayTime > 5)
        {
            _autoCloseWindowStayTime = 0;
            ViewModel.AutoCloseWindowCountdown = 0;
            ViewModel.IsShowAutoCloseWindowTip = false;
            ViewModel.AutoCloseWindowCommand.Execute(default);
        }
        else
        {
            ViewModel.AutoCloseWindowCountdown = Math.Ceiling(5 - _autoCloseWindowStayTime);
        }
    }

    private void ShowAndResetMediaTransport(bool isMouse)
    {
        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
        if (!ViewModel.IsShowMediaTransport
            && isMouse)
        {
            _ = DispatcherQueue.TryEnqueue(() =>
            {
                ViewModel.IsShowMediaTransport = true;
            });
        }

        _cursorStayTime = 0;
        _transportStayTime = 0;
    }

    private void HideAndResetMediaTransport()
    {
        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
        _cursorStayTime = 0;
        _transportStayTime = 0;
    }

    private void ResizeSubtitle()
    {
        if (ActualWidth == 0 || ActualHeight == 0 || _subtitleBlock == null)
        {
            return;
        }

        var baseWidth = 800d;
        var baseHeight = 600d;
        var scale = Math.Min(ActualWidth / baseWidth, ActualHeight / baseHeight);
        if (scale > 1.6)
        {
            scale = 1.6;
        }
        else if (scale < 0.4)
        {
            scale = 0.4;
        }

        _subtitleBlock.FontSize = 18 * scale;
    }
}
