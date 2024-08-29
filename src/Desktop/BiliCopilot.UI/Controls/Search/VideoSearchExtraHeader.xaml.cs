// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.Controls.Search;

/// <summary>
/// 视频搜索附加头.
/// </summary>
public sealed partial class VideoSearchExtraHeader : VideoSectionDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoSearchExtraHeader"/> class.
    /// </summary>
    public VideoSearchExtraHeader() => InitializeComponent();

    private void OnSortComboBoxChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || SortComboBox.SelectedIndex == -1)
        {
            return;
        }

        var sort = (ComprehensiveSearchSortType)SortComboBox.SelectedItem;
        ViewModel.ChangeSortTypeCommand.Execute(sort);
    }
}
