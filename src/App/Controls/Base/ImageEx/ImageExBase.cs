﻿// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Media;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// Base Code for ImageEx.
/// </summary>
[TemplateVisualState(Name = LoadingState, GroupName = CommonGroup)]
[TemplateVisualState(Name = LoadedState, GroupName = CommonGroup)]
[TemplateVisualState(Name = UnloadedState, GroupName = CommonGroup)]
[TemplateVisualState(Name = FailedState, GroupName = CommonGroup)]
[TemplatePart(Name = PartImage, Type = typeof(object))]
public abstract partial class ImageExBase : Control, IAlphaMaskProvider
{
    /// <summary>
    /// Image name in template.
    /// </summary>
    protected const string PartImage = "Image";

    /// <summary>
    /// VisualStates name in template.
    /// </summary>
    protected const string CommonGroup = "CommonStates";

    /// <summary>
    /// Loading state name in template.
    /// </summary>
    protected const string LoadingState = "Loading";

    /// <summary>
    /// Loaded state name in template.
    /// </summary>
    protected const string LoadedState = "Loaded";

    /// <summary>
    /// Unloaded state name in template.
    /// </summary>
    protected const string UnloadedState = "Unloaded";

    /// <summary>
    /// Failed name in template.
    /// </summary>
    protected const string FailedState = "Failed";

    private bool _isInViewport;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageExBase"/> class.
    /// </summary>
    public ImageExBase()
    {
    }

    /// <inheritdoc/>
    public bool WaitUntilLoaded => true;

    /// <summary>
    /// Gets the backing image object.
    /// </summary>
    protected object Image { get; private set; }

    /// <summary>
    /// Returns a mask that represents the alpha channel of an image as a <see cref="CompositionBrush"/>.
    /// </summary>
    /// <returns><see cref="CompositionBrush"/>.</returns>
    public abstract CompositionBrush GetAlphaMask();

    /// <summary>
    /// Attach image opened event handler.
    /// </summary>
    /// <param name="handler">Routed Event Handler.</param>
    protected void AttachImageOpened(RoutedEventHandler handler)
    {
        if (Image is Image image)
        {
            image.ImageOpened += handler;
        }
        else if (Image is ImageBrush brush)
        {
            brush.ImageOpened += handler;
        }
    }

    /// <summary>
    /// Attach image tapped event handler.
    /// </summary>
    /// <param name="handler">Routed Event Handler.</param>
    protected void AttachImageTapped(TappedEventHandler handler)
    {
        if (Image is Image image)
        {
            image.Tapped += handler;
        }
    }

    /// <summary>
    /// Remove image opened handler.
    /// </summary>
    /// <param name="handler">RoutedEventHandler.</param>
    protected void RemoveImageOpened(RoutedEventHandler handler)
    {
        if (Image is Image image)
        {
            image.ImageOpened -= handler;
        }
        else if (Image is ImageBrush brush)
        {
            brush.ImageOpened -= handler;
        }
    }

    /// <summary>
    /// Remove image tapped event handler.
    /// </summary>
    /// <param name="handler">Routed Event Handler.</param>
    protected void RemoveImageTapped(TappedEventHandler handler)
    {
        if (Image is Image image)
        {
            image.Tapped -= handler;
        }
    }

    /// <summary>
    /// Attach image failed event handler.
    /// </summary>
    /// <param name="handler">Exception Routed Event Handler.</param>
    protected void AttachImageFailed(ExceptionRoutedEventHandler handler)
    {
        if (Image is Image image)
        {
            image.ImageFailed += handler;
        }
        else if (Image is ImageBrush brush)
        {
            brush.ImageFailed += handler;
        }
    }

    /// <summary>
    /// Remove Image Failed handler.
    /// </summary>
    /// <param name="handler">Exception Routed Event Handler.</param>
    protected void RemoveImageFailed(ExceptionRoutedEventHandler handler)
    {
        if (Image is Image image)
        {
            image.ImageFailed -= handler;
        }
        else if (Image is ImageBrush brush)
        {
            brush.ImageFailed -= handler;
        }
    }

    /// <summary>
    /// Update the visual state of the control when its template is changed.
    /// </summary>
    protected override void OnApplyTemplate()
    {
        RemoveImageOpened(OnImageOpened);
        RemoveImageFailed(OnImageFailed);
        RemoveImageTapped(OnImageTapped);

        Image = GetTemplateChild(PartImage);

        IsInitialized = true;

        ImageExInitialized?.Invoke(this, EventArgs.Empty);

        if (Source == null || !EnableLazyLoading || _isInViewport)
        {
            _lazyLoadingSource = null;
            SetSourceAsync(Source);
        }
        else
        {
            _lazyLoadingSource = Source;
        }

        AttachImageOpened(OnImageOpened);
        AttachImageFailed(OnImageFailed);
        AttachImageTapped(OnImageTapped);

        base.OnApplyTemplate();
    }

    /// <summary>
    /// 触发 Command.
    /// </summary>
    /// <param name="sender">Image.</param>
    /// <param name="e">Arguments.</param>
    protected virtual void OnImageTapped(object sender, TappedRoutedEventArgs e)
        => Command?.Execute(default);

    /// <summary>
    /// Underlying <see cref="Image.ImageOpened"/> event handler.
    /// </summary>
    /// <param name="sender">Image.</param>
    /// <param name="e">Event Arguments.</param>
    protected virtual void OnImageOpened(object sender, RoutedEventArgs e)
    {
        _ = VisualStateManager.GoToState(this, LoadedState, true);
        ImageExOpened?.Invoke(this, new ImageExOpenedEventArgs());
    }

    /// <summary>
    /// Underlying <see cref="Image.ImageFailed"/> event handler.
    /// </summary>
    /// <param name="sender">Image.</param>
    /// <param name="e">Event Arguments.</param>
    protected virtual void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        _ = VisualStateManager.GoToState(this, FailedState, true);
        ImageExFailed?.Invoke(this, new ImageExFailedEventArgs(new Exception(e.ErrorMessage)));
    }

    private static bool IntersectsWith(Rect rect1, Rect rect2)
    {
        return !rect1.IsEmpty && !rect2.IsEmpty
&& (rect1.Left <= rect2.Right) &&
               (rect1.Right >= rect2.Left) &&
               (rect1.Top <= rect2.Bottom) &&
               (rect1.Bottom >= rect2.Top);
    }

    private void ImageExBase_LayoutUpdated(object sender, object e) => InvalidateLazyLoading();

    private void InvalidateLazyLoading()
    {
        if (!IsLoaded)
        {
            _isInViewport = false;
            return;
        }

        // Find the first ascendant ScrollViewer, if not found, use the root element.
        FrameworkElement hostElement = null;
        var ascendants = this.FindAscendants().OfType<FrameworkElement>();
        foreach (var ascendant in ascendants)
        {
            hostElement = ascendant;
            if (hostElement is ScrollViewer)
            {
                break;
            }
        }

        if (hostElement == null)
        {
            _isInViewport = false;
            return;
        }

        var controlRect = TransformToVisual(hostElement)
            .TransformBounds(new Rect(0, 0, ActualWidth, ActualHeight));
        var lazyLoadingThreshold = LazyLoadingThreshold;
        var hostRect = new Rect(
            0 - lazyLoadingThreshold,
            0 - lazyLoadingThreshold,
            hostElement.ActualWidth + (2 * lazyLoadingThreshold),
            hostElement.ActualHeight + (2 * lazyLoadingThreshold));

        if (IntersectsWith(controlRect, hostRect))
        {
            _isInViewport = true;

            if (_lazyLoadingSource != null)
            {
                var source = _lazyLoadingSource;
                _lazyLoadingSource = null;
                SetSourceAsync(source);
            }
        }
        else
        {
            _isInViewport = false;
        }
    }
}
