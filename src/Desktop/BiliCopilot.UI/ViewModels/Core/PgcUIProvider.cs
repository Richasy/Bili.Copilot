// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Controls.Danmaku;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed class PgcUIProvider : IMediaUIProvider
{
    private readonly PgcSourceViewModel _sourceViewModel;
    private MpvPlayerWindowViewModel? _windowViewModel;

    public PgcUIProvider(PgcSourceViewModel vm) => _sourceViewModel = vm;

    public UIElement GetUIElement() => new PgcPlayerOverlay(_sourceViewModel, _windowViewModel);

    public UIElement? GetBackgroundElement() => new VideoDanmakuPanel { ViewModel = _sourceViewModel.Danmaku, Margin = new Thickness(0, 8, 0, 0) };

    public void SetWindowViewModel(MpvPlayerWindowViewModel vm) => _windowViewModel = vm;

    public void ShowError(string title, string message) => throw new NotImplementedException();
}
