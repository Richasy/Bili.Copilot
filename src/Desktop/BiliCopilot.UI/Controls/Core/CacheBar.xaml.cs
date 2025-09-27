// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Microsoft.UI.Xaml.Shapes;
using Richasy.MpvKernel.Core.Models;

namespace BiliCopilot.UI.Controls.Core;

public sealed partial class CacheBar : PlayerControlBase
{
    public CacheBar()
    {
        InitializeComponent();
    }

    protected override void OnViewModelChanged(PlayerViewModel? oldValue, PlayerViewModel? newValue)
    {
        if (oldValue != null)
        {
            oldValue.CacheStateChanged -= OnCacheStateChanged;
        }

        if (newValue != null)
        {
            newValue.CacheStateChanged += OnCacheStateChanged;
        }
    }

    protected override void OnControlUnloaded()
    {
        ViewModel?.CacheStateChanged -= OnCacheStateChanged;
    }

    private void OnCacheStateChanged(object? sender, MpvCacheStateEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var rangeIndex = 0;
            foreach (var range in e.SeekableRanges)
            {
                if (rangeIndex >= RootGrid.Children.Count)
                {
                    var newRangeEle = CacheBarItemTemplate.LoadContent() as Rectangle;
                    RootGrid.Children.Add(newRangeEle!);
                }

                var rangeEle = RootGrid.Children[rangeIndex] as Rectangle;
                var width = RootGrid.ActualWidth * (range.End - range.Start) / ViewModel.Player.Duration;
                if (double.IsInfinity(width) || double.IsNaN(width) || width < 1)
                {
                    continue;
                }

                var leftMargin = RootGrid.ActualWidth * range.Start / ViewModel.Player.Duration;
                rangeEle!.Width = width; // Ensure minimum width
                rangeEle.Margin = new Thickness(leftMargin, 0, 0, 0);
                rangeIndex++;
            }

            if (rangeIndex < RootGrid.Children.Count - 1)
            {
                // É¾³ý¶àÓàµÄ·¶Î§ÔªËØ.
                for (var i = RootGrid.Children.Count - 1; i > rangeIndex; i--)
                {
                    RootGrid.Children.RemoveAt(i);
                }
            }
        });
    }
}
