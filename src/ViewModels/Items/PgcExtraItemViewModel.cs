// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Pgc;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC 附加内容条目视图模型.
/// </summary>
public sealed partial class PgcExtraItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title;

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcExtraItemViewModel"/> class.
    /// </summary>
    public PgcExtraItemViewModel(string title, IEnumerable<EpisodeInformation> episodes, string currentId)
    {
        Episodes = new ObservableCollection<EpisodeItemViewModel>();
        Title = title;
        foreach (var item in episodes)
        {
            var vm = new EpisodeItemViewModel(item);
            vm.IsSelected = item.Identifier.Id == currentId;
            Episodes.Add(vm);
        }
    }

    /// <summary>
    /// 分集.
    /// </summary>
    public ObservableCollection<EpisodeItemViewModel> Episodes { get; }
}
