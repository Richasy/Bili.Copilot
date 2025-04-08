// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Core;

public sealed partial class VideoPlayerOverlay : MpvPlayerControlBase
{
    public VideoSourceViewModel Source { get; }

    public VideoPlayerOverlay(VideoSourceViewModel sourceVM, MpvPlayerWindowViewModel stateVM)
    {
        InitializeComponent();
        Source = sourceVM;
        ViewModel = stateVM;
    }
}

public abstract class MpvPlayerControlBase : LayoutUserControlBase<MpvPlayerWindowViewModel>;