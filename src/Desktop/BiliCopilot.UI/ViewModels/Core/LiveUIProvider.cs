// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Controls.Danmaku;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed class LiveUIProvider : IMediaUIProvider
{
    private readonly LiveSourceViewModel _sourceViewModel;
    private MpvPlayerWindowViewModel? _windowViewModel;

    public LiveUIProvider(LiveSourceViewModel vm) => _sourceViewModel = vm;

    public UIElement GetUIElement() => new LivePlayerOverlay(_sourceViewModel, _windowViewModel);

    public UIElement? GetBackgroundElement() => new LiveDanmakuPanel { ViewModel = _sourceViewModel.Danmaku, Margin = new Thickness(0, 8, 0, 0) };

    public void SetWindowViewModel(MpvPlayerWindowViewModel vm) => _windowViewModel = vm;

    public void ShowError(string title, string message) => throw new NotImplementedException();
}
