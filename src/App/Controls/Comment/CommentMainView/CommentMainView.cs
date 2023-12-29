// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Comment;

/// <summary>
/// 评论区主视图.
/// </summary>
public sealed class CommentMainView : ReactiveControl<CommentMainModuleViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentMainView"/> class.
    /// </summary>
    public CommentMainView() => DefaultStyleKey = typeof(CommentMainView);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var orderComboBox = GetTemplateChild("OrderTypeComboBox") as ComboBox;
        orderComboBox.SelectionChanged += OnOrderComboBoxSelectionChanged;
    }

    private void OnOrderComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as ComboBox).SelectedItem is CommentSortHeader item && item != ViewModel.CurrentSort)
        {
            ViewModel.ChangeSortCommand.Execute(item);
        }
    }
}
