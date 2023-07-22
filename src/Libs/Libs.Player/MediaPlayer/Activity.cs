// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using Bili.Copilot.Libs.Player.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 表示一个活动.
/// </summary>
public class Activity : ObservableObject
{
    private readonly Player _player;
    private readonly Stopwatch _swKeyboard = new();
    private readonly Stopwatch _swMouse = new();
    private int _timeout;
    private bool _isEnabled;
    private ActivityMode _mode = ActivityMode.FullActive;

    /// <summary>
    /// 初始化 Activity 类的新实例.
    /// </summary>
    /// <param name="player">与活动关联的玩家.</param>
    public Activity(Player player) => _player = player;

    /// <summary>
    /// 获取或设置活动的模式.
    /// </summary>
    public ActivityMode Mode
    {
        get => _mode;
        set
        {
            if (value == _mode)
            {
                return;
            }

            _mode = value;

            if (value == ActivityMode.Idle)
            {
                _swKeyboard.Reset();
                _swMouse.Reset();
            }
            else if (value == ActivityMode.Active)
            {
                _swKeyboard.Restart();
            }
            else
            {
                _swMouse.Restart();
            }

            Utils.UI(() => SetMode());
        }
    }

    /// <summary>
    /// 获取或设置活动是否启用.
    /// </summary>
    /// <remarks>
    /// 当 Timeout 为 0 时，使用此属性进行临时禁用.
    /// </remarks>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value && _timeout <= 0)
            {
                if (_isEnabled)
                {
                    _isEnabled = false;
                    OnPropertyChanged(nameof(IsEnabled));
                }
                else
                {
                    _isEnabled = false;
                }
            }
            else
            {
                if (_isEnabled == value)
                {
                    return;
                }

                if (value)
                {
                    _swKeyboard.Restart();
                    _swMouse.Restart();
                }

                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
    }

    /// <summary>
    /// 获取或设置活动的超时时间.
    /// </summary>
    public int Timeout
    {
        get => _timeout;
        set
        {
            _timeout = value;
            IsEnabled = value > 0;
        }
    }

    /// <summary>
    /// 将模式设置为 Idle.
    /// </summary>
    public void ForceIdle()
    {
        if (Timeout > 0)
        {
            Mode = ActivityMode.Idle;
        }
    }

    /// <summary>
    /// 将模式设置为 Active.
    /// </summary>
    public void ForceActive() => Mode = ActivityMode.Active;

    /// <summary>
    /// 将模式设置为 FullActive.
    /// </summary>
    public void ForceFullActive() => Mode = ActivityMode.FullActive;

    /// <summary>
    /// 更新活动时间戳.
    /// </summary>
    public void RefreshActive() => _swKeyboard.Restart();

    /// <summary>
    /// 更新完全活动时间戳.
    /// </summary>
    public void RefreshFullActive() => _swMouse.Restart();

    /// <summary>
    /// 更新模式的 UI 值，并根据需要显示/隐藏鼠标光标.
    /// 必须在 UI 线程中调用此方法.
    /// </summary>
    internal void SetMode()
    {
        OnPropertyChanged(nameof(Mode));
        _player.Log.Trace(_mode.ToString());
    }

    /// <summary>
    /// 根据当前时间戳刷新模式的值.
    /// </summary>
    internal void RefreshMode()
    {
        _mode = !IsEnabled
            ? ActivityMode.FullActive
            : _swMouse.IsRunning && _swMouse.ElapsedMilliseconds < Timeout
                ? ActivityMode.FullActive
                : _swKeyboard.IsRunning && _swKeyboard.ElapsedMilliseconds < Timeout ? ActivityMode.Active : ActivityMode.Idle;
    }
}
