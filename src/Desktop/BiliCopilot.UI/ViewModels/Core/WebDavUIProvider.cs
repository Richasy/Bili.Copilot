// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed class WebDavUIProvider : IMediaUIProvider
{
    private readonly WebDavSourceViewModel _sourceViewModel;
    private MpvPlayerWindowViewModel? _windowViewModel;

    public WebDavUIProvider(WebDavSourceViewModel vm) => _sourceViewModel = vm;

    public UIElement GetUIElement() => new WebDavPlayerOverlay(_sourceViewModel, _windowViewModel);

    public UIElement? GetBackgroundElement() => default;

    public void SetWindowViewModel(MpvPlayerWindowViewModel vm) => _windowViewModel = vm;

    public void ShowError(string title, string message) => throw new NotImplementedException();
}
