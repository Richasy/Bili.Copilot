// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 字幕配置面板.
/// </summary>
public sealed partial class SubtitleConfigPanel : SubtitleConfigPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubtitleConfigPanel"/> class.
    /// </summary>
    public SubtitleConfigPanel() => InitializeComponent();

    private void OnMetaItemClick(object sender, ItemClickEventArgs e)
    {
        var data = e.ClickedItem as SubtitleMeta;
        if (ViewModel.CurrentMeta != data)
        {
            ViewModel.ChangeMetaCommand.Execute(data);
        }
    }
}

/// <summary>
/// <see cref="SubtitleConfigPanel"/> 的基类.
/// </summary>
public abstract class SubtitleConfigPanelBase : ReactiveUserControl<SubtitleModuleViewModel>
{
}
