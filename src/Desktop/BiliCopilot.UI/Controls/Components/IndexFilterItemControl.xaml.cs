// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 索引过滤器项控件.
/// </summary>
public sealed partial class IndexFilterItemControl : IndexFilterItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexFilterItemControl"/> class.
    /// </summary>
    public IndexFilterItemControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ConditionComboBox.SelectionChanged += OnSelectionChanged;

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ConditionComboBox.SelectionChanged -= OnSelectionChanged;

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var condition = ConditionComboBox.SelectedItem as Condition;
        if (condition is not null)
        {
            ViewModel.ChangeConditionCommand.Execute(condition);
        }
    }
}

/// <summary>
/// 索引过滤器项控件基类.
/// </summary>
public abstract class IndexFilterItemControlBase : LayoutUserControlBase<IndexFilterViewModel>
{
}
