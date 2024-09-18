// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core.Common;
using BiliCopilot.UI.ViewModels.Core;
using OpenTK.Graphics.ES30;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// MPV 播放器.
/// </summary>
public sealed partial class MpvPlayer : LayoutControlBase<MpvPlayerViewModel>
{
    private RenderControl _renderControl;

    /// <summary>
    /// Initializes a new instance of the <see cref="MpvPlayer"/> class.
    /// </summary>
    public MpvPlayer() => DefaultStyleKey = typeof(MpvPlayer);

    /// <inheritdoc/>
    protected async override void OnControlLoaded()
    {
        _renderControl.Render += OnRender;
        if (ViewModel is null)
        {
            return;
        }

        await ViewModel.InitializeAsync(_renderControl);
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        _renderControl.Render -= OnRender;
    }

    /// <inheritdoc/>
    protected async override void OnViewModelChanged(MpvPlayerViewModel? oldValue, MpvPlayerViewModel? newValue)
    {
        if (_renderControl is null)
        {
            return;
        }

        await newValue?.InitializeAsync(_renderControl);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _renderControl = (RenderControl)GetTemplateChild("RenderControl");
        _renderControl.Setting = new ContextSettings();
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
