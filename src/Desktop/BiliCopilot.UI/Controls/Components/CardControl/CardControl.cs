// Copyright (c) Bili Copilot. All rights reserved.

using System.Numerics;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 卡片控件.
/// </summary>
public sealed class CardControl : Button
{
    private const float PointerOverOffsetY = -2f;

    private static readonly TimeSpan _pointerOverShadowDuration = TimeSpan.FromMilliseconds(240);
    private static readonly TimeSpan _pressedShadowDuration = TimeSpan.FromMilliseconds(200);
    private static readonly TimeSpan _restShadowDuration = TimeSpan.FromMilliseconds(250);

    private Compositor _compositor;
    private FrameworkElement _shadowContainer;
    private AttachedShadowBase _initialShadow;
    private bool _loaded;
    private bool _templateApplied;
    private bool _shadowCreated;
    private bool _shouldDestoryShadow;
    private long _pointerOverToken;
    private long _pressedToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="CardControl"/> class.
    /// </summary>
    public CardControl()
    {
        DefaultStyleKey = typeof(CardControl);
        _compositor = this.Get<AppViewModel>().ActivatedWindow.Compositor;
        Loaded += OnLoadedAsync;
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc/>
    protected override async void OnApplyTemplate()
    {
        _shadowContainer = GetTemplateChild("ShadowContainer") as FrameworkElement;
        ElementCompositionPreview.SetIsTranslationEnabled(_shadowContainer, true);

        _initialShadow = CommunityToolkit.WinUI.Effects.GetShadow(_shadowContainer);
        _templateApplied = true;
        await ApplyShadowAnimationAsync().ConfigureAwait(true);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoadedAsync;
        Unloaded -= OnUnloaded;

        UnregisterPropertyChangedCallback(IsPointerOverProperty, _pointerOverToken);
        UnregisterPropertyChangedCallback(IsPressedProperty, _pressedToken);
        _loaded = false;
        _initialShadow = default;

        DestroyShadow();
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        _pointerOverToken = RegisterPropertyChangedCallback(IsPointerOverProperty, OnButtonStateChangedAsync);
        _pressedToken = RegisterPropertyChangedCallback(IsPressedProperty, OnButtonStateChangedAsync);
        _loaded = true;

        await ApplyShadowAnimationAsync().ConfigureAwait(true);
    }

    private async void OnButtonStateChangedAsync(DependencyObject sender, DependencyProperty dp)
        => await ApplyShadowAnimationAsync().ConfigureAwait(true);

    private void CreateShadow()
    {
        if (_shadowCreated || !_loaded || !_templateApplied)
        {
            return;
        }

        Effects.SetShadow(_shadowContainer, _initialShadow);
        var shadowContext = _initialShadow.GetElementContext(_shadowContainer);
        shadowContext.CreateResources();

        if (shadowContext.Shadow is DropShadow dropShadow)
        {
            dropShadow.Offset = GetShadowOffset();
            dropShadow.BlurRadius = GetShadowRadius();
            dropShadow.Opacity = GetShadowOpacity();
        }

        _shadowCreated = true;
    }

    private void DestroyShadow()
    {
        if (!_shadowCreated)
        {
            return;
        }

        _shadowCreated = false;
        Effects.SetShadow(_shadowContainer, default);
        _compositor = default;
    }

    private async Task ApplyShadowAnimationAsync()
    {
        if (!_templateApplied)
        {
            return;
        }

        var duration = IsPointerOver ? _pointerOverShadowDuration : IsPressed ? _pressedShadowDuration : _restShadowDuration;
        if (IsPointerOver)
        {
            await AnimationBuilder.Create().Translation(Axis.Y, PointerOverOffsetY, duration: duration, easingMode: Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseIn).StartAsync(this).ConfigureAwait(true);
        }
        else
        {
            await AnimationBuilder.Create().Translation(Axis.Y, 0, duration: duration, easingMode: Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseOut).StartAsync(this).ConfigureAwait(true);
        }

        var shadowOpacity = GetShadowOpacity();
        var shadowRadius = GetShadowRadius();
        var shadowOffset = GetShadowOffset();

        _shouldDestoryShadow = shadowOpacity <= 0;
        if (!_shouldDestoryShadow)
        {
            CreateShadow();
        }

        var shadowContext = _initialShadow.GetElementContext(_shadowContainer);
        if (shadowContext.Shadow is DropShadow dropShadow)
        {
            using var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            var shadowAnimationGroup = _compositor.CreateAnimationGroup();
            shadowAnimationGroup.Add(_compositor.CreateScalarKeyFrameAnimation(nameof(DropShadow.BlurRadius), shadowRadius, duration: duration));
            shadowAnimationGroup.Add(_compositor.CreateVector3KeyFrameAnimation(nameof(DropShadow.Offset), shadowOffset, duration: duration));
            shadowAnimationGroup.Add(_compositor.CreateScalarKeyFrameAnimation(nameof(DropShadow.Opacity), shadowOpacity, duration: duration));
            dropShadow.StartAnimationGroup(shadowAnimationGroup);

            if (_shouldDestoryShadow)
            {
                DestroyShadow();
            }
        }
    }

    private float GetShadowRadius()
        => IsPointerOver ? 12f : IsPressed ? 2f : 6f;

    private float GetShadowOpacity()
        => IsPointerOver ? 0.2f : IsPressed ? 0.12f : 0.02f;

    private Vector3 GetShadowOffset()
        => new Vector3(0, IsPointerOver ? 4f : IsPressed ? 0f : 2f, 0);
}
