// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 原生播放器.
/// </summary>
public sealed partial class NativePlayer : LayoutControlBase<NativePlayerViewModel>
{
    private NativePlayerViewModel? _viewModel;
    private long _viewModelChangedToken;
    private MediaPlayerElement _playerElement;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativePlayer"/> class.
    /// </summary>
    public NativePlayer() => DefaultStyleKey = typeof(NativePlayer);

    /// <inheritdoc/>
    protected async override void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChangedAsync));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        await _viewModel.InitializeAsync(_playerElement);
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
        _playerElement = (MediaPlayerElement)GetTemplateChild("PlayerElement");
    }

    private async void OnViewModelPropertyChangedAsync(DependencyObject sender, DependencyProperty dp)
    {
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        await _viewModel.InitializeAsync(_playerElement);
    }
}
