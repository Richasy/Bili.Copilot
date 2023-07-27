// Copyright (c) Bili Copilot. All rights reserved.

using System.Linq;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 视频分集视图.
/// </summary>
public sealed partial class VideoPartView : VideoPartViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartView"/> class.
    /// </summary>
    public VideoPartView() => InitializeComponent();

    private void OnPartItemClick(object sender, RoutedEventArgs e)
    {
        var card = sender as CardPanel;
        var data = card.DataContext as VideoIdentifierSelectableViewModel;
        if (!data.Data.Equals(ViewModel.CurrentVideoPart))
        {
            ViewModel.ChangeVideoPartCommand.Execute(data.Data);
        }
        else
        {
            data.IsSelected = true;
        }
    }

    private void RelocateSelectedItem()
    {
        var vm = ViewModel.VideoParts.FirstOrDefault(p => p.IsSelected);
        if (vm != null)
        {
            var index = ViewModel.VideoParts.IndexOf(vm);
            if (index >= 0)
            {
                PartRepeater.ScrollToItem(index);
                if (ViewModel.IsOnlyShowIndex)
                {
                    var ele = IndexRepeater.GetOrCreateElement(index);
                    ele?.StartBringIntoView(new BringIntoViewOptions { VerticalAlignmentRatio = 0f });
                }
            }
        }
    }

    private void OnPartRepeaterLoaded(object sender, RoutedEventArgs e)
        => RelocateSelectedItem();
}

/// <summary>
/// <see cref="VideoPartView"/> 的基类.
/// </summary>
public abstract class VideoPartViewBase : ReactiveUserControl<VideoPlayerPageViewModel>
{
}
