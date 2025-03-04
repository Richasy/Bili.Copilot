// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 原生播放器.
/// </summary>
public sealed partial class NativePlayer : LayoutControlBase<NativePlayerViewModel>
{
    private MediaPlayerElement _playerElement;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativePlayer"/> class.
    /// </summary>
    public NativePlayer() => DefaultStyleKey = typeof(NativePlayer);

    /// <inheritdoc/>
    protected async override void OnControlLoaded()
    {
        if (ViewModel is null)
        {
            return;
        }

        await ViewModel.InitializeAsync(_playerElement);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var rootGrid = GetTemplateChild("RootGrid") as Grid;
        _playerElement = new MediaPlayerElement();
        _playerElement.HorizontalAlignment = HorizontalAlignment.Stretch;
        _playerElement.VerticalAlignment = VerticalAlignment.Stretch;
        _playerElement.AreTransportControlsEnabled = false;
        rootGrid.Children.Add(_playerElement);
    }

    /// <inheritdoc/>
    protected override async void OnViewModelChanged(NativePlayerViewModel? oldValue, NativePlayerViewModel? newValue)
    {
        if (_playerElement is null)
        {
            return;
        }

        await newValue?.InitializeAsync(_playerElement);
    }
}
