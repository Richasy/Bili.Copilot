// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 分段式播放进度条：分段构建与填充计算.
/// </summary>
public sealed partial class ChapterSegmentBar
{
    /// <summary>
    /// 按当前章节数据同步分段元素。优先复用已存在的分段，仅在数量变化时增删，
    /// 避免窗口缩放（SizeChanged）时反复创建/销毁大量视觉对象.
    /// </summary>
    private void Rebuild()
    {
        var duration = Duration;
        var width = _canvas.ActualWidth;
        if (ViewModel is null || duration <= 0 || width <= 0)
        {
            return;
        }

        var spans = ResolveSpans(duration);
        ApplyThumbBrushes();

        while (_segments.Count > spans.Count)
        {
            var last = _segments[^1];
            _canvas.Children.Remove(last.Container);
            _segments.RemoveAt(_segments.Count - 1);
        }

        while (_segments.Count < spans.Count)
        {
            _segments.Add(CreateSegmentVisual());
        }

        _hovered = null;
        for (var i = 0; i < spans.Count; i++)
        {
            var (start, end, title) = spans[i];

            // 首段左端、末段右端贴齐控件边缘；相邻分段之间各让出半个间隙，
            // 这样两端不留缝、分段之间的缝隙也保持均匀.
            var isFirst = i == 0;
            var isLast = i == spans.Count - 1;
            var left = (start / duration * width) + (isFirst ? 0d : SegmentGap / 2d);
            var right = (end / duration * width) - (isLast ? 0d : SegmentGap / 2d);
            var segmentWidth = Math.Max(right - left, MinSegmentWidth);

            _segments[i].Update(start, end, title, left, segmentWidth);
            LayoutTrack(_segments[i], TrackHeight);
        }

        UpdateCache();
        UpdateFill();
        UpdateCurrentChapter();
    }

    /// <summary>
    /// 创建一个分段视觉（容器 + 轨道 / 缓存 / 已播放三层）并加入画布.
    /// </summary>
    private SegmentVisual CreateSegmentVisual()
    {
        // 容器使用整条控件的高度作为交互热区，避免轨道过细时难以命中.
        var container = new Canvas
        {
            Height = HitAreaHeight,
            Background = HitAreaBackgroundBrush,
        };

        var track = MakeRect(TrackResolvedBrush, 0, TrackHeight);
        var cache = MakeRect(CacheResolvedBrush, 0, TrackHeight);
        var played = MakeRect(PlayedBrush, 0, TrackHeight);

        container.Children.Add(track);
        container.Children.Add(cache);
        container.Children.Add(played);
        Canvas.SetTop(container, (LayoutHeight - HitAreaHeight) / 2);
        _canvas.Children.Insert(0, container);

        return new SegmentVisual(container, track, cache, played);
    }

    /// <summary>
    /// 计算各分段的起止时间。无章节数据时退化为单一整段.
    /// </summary>
    private List<(double Start, double End, string? Title)> ResolveSpans(double duration)
    {
        var spans = new List<(double Start, double End, string? Title)>();
        var chapters = ViewModel?.Chapters;
        _hasChapters = chapters is { Count: > 0 };

        if (chapters is { Count: > 0 })
        {
            foreach (var chapter in chapters)
            {
                var start = Math.Clamp(chapter.Position, 0, duration);
                var rawEnd = chapter.EndPosition > chapter.Position ? chapter.EndPosition : duration;
                var end = Math.Clamp(rawEnd, 0, duration);
                if (end > start)
                {
                    spans.Add((start, end, chapter.Title));
                }
            }
        }

        if (spans.Count == 0)
        {
            spans.Add((0, duration, null));
        }

        return spans;
    }

    /// <summary>
    /// 按给定高度把三层矩形居中放在交互热区内。圆角取高度的一半，形成胶囊形端点.
    /// </summary>
    private static void LayoutTrack(SegmentVisual segment, double height)
    {
        var top = (HitAreaHeight - height) / 2;
        foreach (var rect in new[] { segment.Track, segment.Cache, segment.Played })
        {
            rect.Height = height;
            rect.RadiusX = height / 2;
            rect.RadiusY = height / 2;
            Canvas.SetTop(rect, top);
        }
    }

    /// <summary>
    /// 构造分段中的一层矩形。使用 Rectangle 而非 Border，因为 RadiusX / RadiusY 是可动画的
    /// double，悬浮放大时能让圆角随高度一起变化并保持正圆.
    /// </summary>
    private static Rectangle MakeRect(Brush fill, double width, double height) => new()
    {
        Width = width,
        Height = height,
        RadiusX = height / 2,
        RadiusY = height / 2,
        Fill = fill,
    };

    private void UpdateFill()
    {
        var duration = Duration;
        var width = _canvas.ActualWidth;
        if (duration <= 0 || width <= 0)
        {
            return;
        }

        var position = ResolveDisplayPosition();

        foreach (var segment in _segments)
        {
            var span = segment.End - segment.Start;
            if (span <= 0)
            {
                continue;
            }

            var segWidth = segment.Container.Width;
            var playedSeconds = Math.Clamp(position, segment.Start, segment.End) - segment.Start;
            segment.Played.Width = Math.Clamp(playedSeconds / span * segWidth, 0, segWidth);
        }

        var ratio = Math.Clamp(position / duration, 0, 1);
        var thumbVisibility = ShowThumb ? Visibility.Visible : Visibility.Collapsed;
        if (_thumb.Visibility != thumbVisibility)
        {
            _thumb.Visibility = thumbVisibility;
        }

        Canvas.SetLeft(_thumb, (ratio * width) - (ThumbOuterSize / 2));
        Canvas.SetTop(_thumb, (LayoutHeight - ThumbOuterSize) / 2);
    }

    /// <summary>
    /// 决定进度条应显示的位置：控件自身拖动 &gt; 播放器手势预览 &gt; 实际播放位置.
    /// 播放器的 <c>PreviewPosition</c> 对应原版 <c>ProgressSliderFake</c> 的职责.
    /// </summary>
    private double ResolveDisplayPosition()
        => _dragPosition
            ?? (ViewModel?.IsPreviewProgressChanging == true ? ViewModel.PreviewPosition : Position);

    /// <summary>
    /// 刷新各分段的缓冲宽度。缓冲范围仅在 <see cref="OnCacheStateChanged"/> 时变化，
    /// 因此与 <see cref="UpdateFill"/> 分离，避免每次播放位置更新都重算.
    /// </summary>
    private void UpdateCache()
    {
        foreach (var segment in _segments)
        {
            var span = segment.End - segment.Start;
            if (span <= 0)
            {
                continue;
            }

            var segWidth = segment.Container.Width;
            segment.Cache.Width = Math.Clamp(ResolveCachedWidth(segment, span, segWidth), 0, segWidth);
        }
    }

    /// <summary>
    /// 判断指针是否落在轨道之外的上下边缘带内。
    /// 边缘带代表“跳到章节起点”，轨道本身代表精确跳转.
    /// </summary>
    /// <param name="y">相对画布的纵坐标.</param>
    /// <returns>是否位于上下边缘带.</returns>
    private bool IsOnVerticalEdge(double y)
    {
        var trackTop = (LayoutHeight - TrackHeight) / 2d;
        var trackBottom = trackTop + TrackHeight;
        return y < trackTop || y > trackBottom;
    }

    /// <summary>
    /// 更新光标形状：上下边缘带用手形提示可跳转章节，轨道本身用箭头.
    /// </summary>
    private void UpdateCursor(bool onEdge)
    {
        if (_isOnEdge == onEdge)
        {
            return;
        }

        _isOnEdge = onEdge;
        var shape = onEdge ? InputSystemCursorShape.Hand : InputSystemCursorShape.Arrow;
        if (_cursorShape == shape)
        {
            return;
        }

        _cursorShape = shape;
        ProtectedCursor = InputSystemCursor.Create(shape);
    }

    /// <summary>
    /// 取该分段内已缓冲的宽度（多个可跳转区间取覆盖最远者）.
    /// </summary>
    private double ResolveCachedWidth(SegmentVisual segment, double span, double segWidth)
    {
        var cached = 0d;
        foreach (var (cacheStart, cacheEnd) in _cachedRanges)
        {
            var from = Math.Max(cacheStart, segment.Start);
            var to = Math.Min(cacheEnd, segment.End);
            if (to > from)
            {
                cached = Math.Max(cached, (to - segment.Start) / span * segWidth);
            }
        }

        return cached;
    }
}
