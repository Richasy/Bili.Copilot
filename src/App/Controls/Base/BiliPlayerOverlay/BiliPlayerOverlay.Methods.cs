// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bili.Copilot.App.Controls.Danmaku;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器覆盖层.
/// </summary>
public partial class BiliPlayerOverlay
{
    private void InitializeDanmakuTimer()
    {
        if (_danmakuTimer == null)
        {
            _danmakuTimer = new DispatcherTimer();
            _danmakuTimer.Interval = TimeSpan.FromSeconds(1);
            _danmakuTimer.Tick += OnDanmkuTimerTick;
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

        var group = list.GroupBy(p => p.Time).ToDictionary(x => x.Key, x => x.ToList());
        foreach (var g in group)
        {
            if (_danmakuDictionary.ContainsKey(g.Key))
            {
                _danmakuDictionary[g.Key] = _danmakuDictionary[g.Key].Concat(g.Value).ToList();
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

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        CheckDanmakuZoom();
        ViewModel.DanmakuViewModel.CanShowDanmaku = e.NewSize.Width >= 480;
    }

    private void OnDanmakuViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.DanmakuViewModel.DanmakuZoom))
        {
            CheckDanmakuZoom();
        }
        else if (e.PropertyName == nameof(ViewModel.DanmakuViewModel.DanmakuArea))
        {
            _danmakuView.DanmakuArea = ViewModel.DanmakuViewModel.DanmakuArea;
        }
        else if (e.PropertyName == nameof(ViewModel.DanmakuViewModel.DanmakuSpeed))
        {
            _danmakuView.DanmakuDuration = Convert.ToInt32((2.1 - ViewModel.DanmakuViewModel.DanmakuSpeed) * 10);
        }
    }

    private void OnSendDanmakuSucceeded(object sender, string e)
    {
        var model = new DanmakuModel
        {
            Color = AppToolkit.HexToColor(ViewModel.DanmakuViewModel.Color),
            Size = ViewModel.DanmakuViewModel.IsStandardSize ? 25 : 18,
            Text = e,
            Location = ViewModel.DanmakuViewModel.Location,
        };

        _danmakuView.AddScreenDanmaku(model, true);
    }

    private void OnLiveDanmakuAdded(object sender, LiveDanmakuInformation e)
    {
        if (_danmakuView != null)
        {
            var myName = AccountViewModel.Instance.Name;
            var isOwn = !string.IsNullOrEmpty(myName) && myName == e.UserName;
            _danmakuView.AddLiveDanmaku(e.Text, isOwn, AppToolkit.HexToColor(e.TextColor));
        }
    }

    private void OnRequestClearDanmaku(object sender, EventArgs e)
    {
        _danmakuDictionary.Clear();
        _danmakuTimer.Stop();
        _danmakuView.ClearAll();
    }

    private void OnDanmakuListAdded(object sender, IEnumerable<DanmakuInformation> e)
    {
        InitializeDanmaku(e);
        _danmakuTimer.Start();
    }

    private void OnDanmkuTimerTick(object sender, object e)
    {
        try
        {
            if (ViewModel.Status != PlayerStatus.Playing)
            {
                return;
            }

            var position = ViewModel.Player.Position.TotalSeconds;
            var positionInt = Convert.ToInt32(position);
            if (_danmakuDictionary.ContainsKey(positionInt))
            {
                var data = _danmakuDictionary[positionInt];

                if (ViewModel.DanmakuViewModel.IsDanmakuMerge)
                {
                    data = data.Distinct().ToList();
                }

                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (var item in data)
                    {
                        _danmakuView.AddScreenDanmaku(item, false);
                    }
                });
            }
        }
        catch (Exception)
        {
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Status))
        {
            if (ViewModel.Status == PlayerStatus.Playing)
            {
                _danmakuView.ResumeDanmaku();
            }
            else
            {
                _danmakuView.PauseDanmaku();
            }
        }
    }
}
