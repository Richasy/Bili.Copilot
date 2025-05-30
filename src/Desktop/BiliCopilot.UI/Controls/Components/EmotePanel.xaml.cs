// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 表情面板.
/// </summary>
public sealed partial class EmotePanel : EmotePanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmotePanel"/> class.
    /// </summary>
    public EmotePanel() => InitializeComponent();

    /// <summary>
    /// 表情被选中.
    /// </summary>
    public event EventHandler<string> ItemClick;

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
    {
        await ViewModel.InitializeCommand.ExecuteAsync(default);
        await Task.Delay(400);
        PackageView.Select(0);
    }

    protected override void OnControlUnloaded()
    {
        PackageView.ItemsSource = null;
        EmoteView.ItemsSource = null;
    }

    private void OnPackageChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var package = sender.SelectedItem as EmotePackage;
        EmoteView.ItemsSource = package?.Images;
    }

    private void OnEmoteViewItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        var emote = args.InvokedItem as Emote;
        ItemClick?.Invoke(this, emote?.Key ?? string.Empty);
    }
}

/// <summary>
/// 表情面板基类.
/// </summary>
public abstract class EmotePanelBase : LayoutUserControlBase<EmoteModuleViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmotePanelBase"/> class.
    /// </summary>
    protected EmotePanelBase() => ViewModel = this.Get<EmoteModuleViewModel>();
}
