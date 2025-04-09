// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
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

    private void OnFormatChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (sender as ComboBox).SelectedItem as PlayerFormatItemViewModel;
        if (item is not null && item != Source.SelectedFormat)
        {
            Source.ChangeFormatCommand.Execute(item);
        }
    }
}

public abstract class MpvPlayerControlBase : LayoutUserControlBase<MpvPlayerWindowViewModel>;