// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Popular;

/// <summary>
/// 流行视频页面侧边栏主体.
/// </summary>
public sealed partial class PopularSideBody : PopularPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularSideBody"/> class.
    /// </summary>
    public PopularSideBody()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    private void OnSectionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        _ = this;
        var item = sender.SelectedItem as IPopularSectionItemViewModel;
        Debug.WriteLine($"Section {item?.Title} selected.");
    }
}
