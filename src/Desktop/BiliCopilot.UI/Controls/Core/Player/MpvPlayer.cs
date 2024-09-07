// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core.Common;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUI.Share.Base;
using Silk.NET.OpenGL;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// MPV 播放器.
/// </summary>
public sealed class MpvPlayer : LayoutControlBase<MpvPlayerViewModel>
{
    private MpvPlayerViewModel? _viewModel;
    private long _viewModelChangedToken;
    private RenderControl _renderControl;

    /// <summary>
    /// Initializes a new instance of the <see cref="MpvPlayer"/> class.
    /// </summary>
    public MpvPlayer() => DefaultStyleKey = typeof(MpvPlayer);

    /// <inheritdoc/>
    protected async override void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChangedAsync));
        _renderControl.Render += OnRender;
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        await _viewModel.InitializeAsync(_renderControl);
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);

        _viewModel = default;
        _renderControl.Render -= OnRender;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _renderControl = (RenderControl)GetTemplateChild("RenderControl");
        _renderControl.Setting = new ContextSettings();
    }

    private async void OnViewModelPropertyChangedAsync(DependencyObject sender, DependencyProperty dp)
    {
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        await _viewModel.InitializeAsync(_renderControl);
    }

    private void OnRender(TimeSpan e) => Render();

    private void Render()
    {
        if (ViewModel?.Player?.Client?.IsInitialized is not true || ViewModel?.Player?.IsDisposed is true)
        {
            return;
        }

        RenderContext.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        RenderContext.GL.ClearColor(0, 0, 0, 1);
        ViewModel.Player.RenderGL((int)(ActualWidth * _renderControl.ScaleX), (int)(ActualHeight * _renderControl.ScaleY), _renderControl.GetBufferHandle());
    }
}
