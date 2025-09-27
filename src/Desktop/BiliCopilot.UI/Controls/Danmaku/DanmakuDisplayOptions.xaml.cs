// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Danmaku;

/// <summary>
/// 弹幕显示选项.
/// </summary>
public sealed partial class DanmakuDisplayOptions : DanmakuControlBase
{
    public DanmakuDisplayOptions() => InitializeComponent();

    protected override void OnControlLoaded()
    {
        DanmakuRefreshRateBox.SelectedIndex = ViewModel.DanmakuRefreshRate == 60 ? 0 : 1;
        //DanmakuRendererBox.SelectedIndex = ViewModel.ForceSoftwareRenderer ? 1 : 0;
    }

    private void OnRefreshRateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.DanmakuRefreshRate = DanmakuRefreshRateBox.SelectedIndex == 0 ? 60 : 90;
    }

    //private void OnRendererChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    if (!IsLoaded)
    //    {
    //        return;
    //    }

    //    ViewModel.ForceSoftwareRenderer = DanmakuRendererBox.SelectedIndex == 1;
    //}
}
