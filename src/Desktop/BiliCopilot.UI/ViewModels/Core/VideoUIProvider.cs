// Copyright (c) Bili Copilot. All rights reserved.


using BiliCopilot.UI.Controls.Core;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed class VideoUIProvider : IMediaUIProvider
{
    private readonly VideoSourceViewModel _sourceViewModel;
    private MpvPlayerWindowViewModel? _windowViewModel;

    public VideoUIProvider(VideoSourceViewModel vm) => _sourceViewModel = vm;

    public UIElement GetUIElement() => new VideoPlayerOverlay(_sourceViewModel, _windowViewModel);

    public void SetWindowViewModel(MpvPlayerWindowViewModel vm) => _windowViewModel = vm;

    public void ShowError(string title, string message) => throw new NotImplementedException();
}
