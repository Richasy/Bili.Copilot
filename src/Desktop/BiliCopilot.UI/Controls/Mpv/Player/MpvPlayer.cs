// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Mpv.Common;
using BiliCopilot.UI.ViewModels.Core;
using OpenTK.Graphics.OpenGL;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// MPV 播放器.
/// </summary>
public sealed partial class MpvPlayer : LayoutControlBase<PlayerViewModel>
{
    private PlayerViewModel? _viewModel;
    private long _viewModelChangedToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="MpvPlayer"/> class.
    /// </summary>
    public MpvPlayer() => DefaultStyleKey = typeof(MpvPlayer);

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChangedAsync));
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
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _renderControl = (RenderControl)GetTemplateChild("RenderControl");
        _renderControl.Setting = new ContextSettings()
        {
            MajorVersion = 4,
            MinorVersion = 6,
            GraphicsProfile = OpenTK.Windowing.Common.ContextProfile.Compatability,
        };
        _renderControl.Render += OnRender;
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

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        ViewModel.Player.RenderGL((int)(ActualWidth * _renderControl.ScaleX), (int)(ActualHeight * _renderControl.ScaleY), _renderControl.GetBufferHandle());
    }
}
