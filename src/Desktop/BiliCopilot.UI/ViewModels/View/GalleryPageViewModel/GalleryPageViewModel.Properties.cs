// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Models.Appearance;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 图片库页面视图模型.
/// </summary>
public sealed partial class GalleryPageViewModel
{
    [ObservableProperty]
    private IReadOnlyCollection<BiliImage>? _images;

    [ObservableProperty]
    private bool _isGroup;

    [ObservableProperty]
    private bool _isMenuHide;

    [ObservableProperty]
    private BiliImage _selectedImage;
}
