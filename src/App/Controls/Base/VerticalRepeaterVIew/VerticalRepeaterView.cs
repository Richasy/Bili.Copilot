// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections;
using System.Threading.Tasks;
using Bili.Copilot.App.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 纵向视图.
/// </summary>
public sealed partial class VerticalRepeaterView : Control
{
    private ScrollViewer _parentScrollViewer;
    private ItemsRepeater _itemsRepeater;
    private double _itemHolderHeight = 0d;

    /// <summary>
    /// Initializes a new instance of the <see cref="VerticalRepeaterView"/> class.
    /// </summary>
    public VerticalRepeaterView()
    {
        DefaultStyleKey = typeof(VerticalRepeaterView);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// 滚动到条目.
    /// </summary>
    /// <param name="index">条目索引值.</param>
    public void ScrollToItem(int index)
    {
        if (_itemsRepeater != null)
        {
            var element = _itemsRepeater.GetOrCreateElement(index);
            var options = new BringIntoViewOptions
            {
                VerticalAlignmentRatio = 0f,
            };
            element.StartBringIntoView(options);
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _itemsRepeater = GetTemplateChild("ItemsRepeater") as ItemsRepeater;

        if (_itemsRepeater != null)
        {
            _itemsRepeater.ElementPrepared += OnElementPreparedAsync;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (EnableDetectParentScrollViewer)
        {
            _parentScrollViewer = this.FindAscendantElementByType<ScrollViewer>();
            if (_parentScrollViewer != null)
            {
                _parentScrollViewer.ViewChanged += OnParentScrollViewerViewChanged;
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_parentScrollViewer != null)
        {
            _parentScrollViewer.ViewChanged -= OnParentScrollViewerViewChanged;
            _parentScrollViewer = null;
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_parentScrollViewer != null)
        {
            var currentPosition = _parentScrollViewer.VerticalOffset;
            if (_parentScrollViewer.ScrollableHeight - currentPosition <= _itemHolderHeight &&
                Visibility == Visibility.Visible)
            {
                RequestLoadMore?.Invoke(this, EventArgs.Empty);
                IncrementalTriggered?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void OnParentScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (!e.IsIntermediate && _parentScrollViewer != null)
        {
            var currentPosition = _parentScrollViewer.VerticalOffset;
            if (_parentScrollViewer.ScrollableHeight - currentPosition <= _itemHolderHeight &&
                Visibility == Visibility.Visible)
            {
                RequestLoadMore?.Invoke(this, EventArgs.Empty);
                IncrementalTriggered?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private async void OnElementPreparedAsync(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        if (args.Element != null)
        {
            if (IsAutoFillEnable &&
                args.Element is IRepeaterItem repeaterItem &&
                ItemsSource is ICollection collectionSource &&
                (_parentScrollViewer != null) &&
                args.Index >= collectionSource.Count - 1)
            {
                var size = repeaterItem.GetHolderSize();
                _itemHolderHeight = size.Height;
                var viewportWidth = _parentScrollViewer.ViewportWidth;
                var viewportHeight = _parentScrollViewer.ViewportHeight;
                bool isNeedLoadMore;
                if (double.IsInfinity(size.Width))
                {
                    isNeedLoadMore = (args.Index + 1) * size.Height <= viewportHeight;
                }
                else
                {
                    var rowCount = args.Index / (viewportWidth / size.Width);
                    isNeedLoadMore = rowCount * size.Height <= viewportHeight;
                }

                if (isNeedLoadMore)
                {
                    RequestLoadMore?.Invoke(this, EventArgs.Empty);
                    IncrementalTriggered?.Invoke(this, EventArgs.Empty);
                }
            }

            if (args.Index == 0)
            {
                await Task.Delay(200);
                await FocusManager.TryFocusAsync(args.Element, FocusState.Programmatic);
            }
        }
    }
}
