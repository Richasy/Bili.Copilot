// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections;
using Bili.Copilot.App.Extensions;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 纵向视图.
/// </summary>
public sealed partial class VerticalRepeaterView : Control
{
    private double _itemHolderHeight = 80d;
    private ScrollViewer _parentScrollViewer;
    private ItemsRepeater _itemsRepeater;
    private ItemsControl _itemsControl;

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
    /// <param name="item">条目.</param>
    public void ScrollToItem(object item)
    {
        var element = _itemsControl.ContainerFromItem(item);
        if (element is FrameworkElement ele)
        {
            var options = new BringIntoViewOptions
            {
                VerticalAlignmentRatio = 0f,
            };
            ele.StartBringIntoView(options);
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _itemsControl = GetTemplateChild("ItemsControl") as ItemsControl;
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
            if (args.Element is IRepeaterItem repeaterItem &&
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
                _ = await FocusManager.TryFocusAsync(args.Element, FocusState.Programmatic);
            }
        }
    }
}
