// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUIKernel.Share.Base;
using System.ComponentModel;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 外部播放器.
/// </summary>
public sealed partial class ExternalPlayer : LayoutControlBase<ExternalPlayerViewModel>
{
    private TextBlock _holderBlock;
    private TextBlock _messageBlock;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalPlayer"/> class.
    /// </summary>
    public ExternalPlayer() => DefaultStyleKey = typeof(ExternalPlayer);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _holderBlock = GetTemplateChild("HolderBlock") as TextBlock ?? throw new NullReferenceException();
        _messageBlock = GetTemplateChild("MessageBlock") as TextBlock ?? throw new NullReferenceException();
        UpdateState();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        _holderBlock = default;
        _messageBlock = default;
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ExternalPlayerViewModel? oldValue, ExternalPlayerViewModel? newValue)
    {
        if (oldValue != null)
        {
            oldValue.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (newValue != null)
        {
            newValue.PropertyChanged += OnViewModelPropertyChanged;
            UpdateState();
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.LogMessage))
        {
            UpdateState();
        }
    }

    private void UpdateState()
    {
        if (ViewModel is null || _holderBlock is null || _messageBlock is null)
        {
            return;
        }

        _holderBlock.Visibility = string.IsNullOrEmpty(ViewModel.LogMessage) ? Visibility.Visible : Visibility.Collapsed;
        _messageBlock.Visibility = string.IsNullOrEmpty(ViewModel.LogMessage) ? Visibility.Collapsed : Visibility.Visible;
        _messageBlock.Text = ViewModel.LogMessage ?? string.Empty;
    }
}
