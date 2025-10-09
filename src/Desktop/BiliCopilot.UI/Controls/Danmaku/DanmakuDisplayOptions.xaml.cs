// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;

namespace BiliCopilot.UI.Controls.Danmaku;

/// <summary>
/// 弹幕显示选项.
/// </summary>
public sealed partial class DanmakuDisplayOptions : DanmakuControlBase
{
    public DanmakuDisplayOptions() => InitializeComponent();

    protected override void OnControlLoaded()
    {
        var renderer = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuRenderer, DanmakuRendererType.Win2D);
        RendererComboBox.SelectedIndex = renderer == DanmakuRendererType.DirectX ? 0 : 1;
    }

    private void OnRendererChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = RendererComboBox.SelectedIndex;
        if (index < 0)
        {
            return;
        }

        var renderer = index == 0 ? DanmakuRendererType.DirectX : DanmakuRendererType.Win2D;
        if (ViewModel.Renderer != renderer)
        {
            ViewModel.Renderer = renderer;
        }
    }
}
