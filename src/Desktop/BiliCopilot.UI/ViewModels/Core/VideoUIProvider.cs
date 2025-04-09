// Copyright (c) Bili Copilot. All rights reserved.


using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Controls.Danmaku;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed class VideoUIProvider : IMediaUIProvider
{
    private readonly VideoSourceViewModel _sourceViewModel;
    private MpvPlayerWindowViewModel? _windowViewModel;

    public VideoUIProvider(VideoSourceViewModel vm) => _sourceViewModel = vm;

    public UIElement GetUIElement() => new VideoPlayerOverlay(_sourceViewModel, _windowViewModel);

    public UIElement? GetBackgroundElement() => new VideoDanmakuPanel { ViewModel = _sourceViewModel.Danmaku, Margin = new Thickness(0,8,0,0) };

    public void SetWindowViewModel(MpvPlayerWindowViewModel vm) => _windowViewModel = vm;

    public void ShowError(string title, string message) => throw new NotImplementedException();
}
