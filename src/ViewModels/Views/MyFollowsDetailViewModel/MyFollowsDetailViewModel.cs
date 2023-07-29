// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 我的关注页面视图模型.
/// </summary>
public sealed partial class MyFollowsDetailViewModel : InformationFlowViewModel<UserItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyFollowsDetailViewModel"/> class.
    /// </summary>
    public MyFollowsDetailViewModel()
    {
        _cache = new Dictionary<string, IEnumerable<UserItemViewModel>>();
        Groups = new ObservableCollection<FollowGroup>();
        AttachIsRunningToAsyncCommand(p => IsSwitching = p, SelectGroupCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayException, SelectGroupCommand);
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        AccountProvider.Instance.ClearMyFollowStatus();
        UserName = AccountViewModel.Instance.Name;
        TryClear(Groups);
        CurrentGroup = null;
        _cache.Clear();
        IsCurrentGroupEmpty = false;
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        IsCurrentGroupEmpty = false;
        if (Groups.Count == 0)
        {
            // 加载分组.
            var groups = await AccountProvider.GetMyFollowingGroupsAsync();
            groups.ToList().ForEach(p => Groups.Add(p));
            CurrentGroup = Groups.FirstOrDefault(p => p.TotalCount > 0) ?? Groups.FirstOrDefault();
        }

        if (CurrentGroup == null
            || CurrentGroup.TotalCount <= Items.Count)
        {
            IsCurrentGroupEmpty = Items.Count == 0;
            return;
        }

        var data = await AccountProvider.Instance.GetMyFollowingGroupDetailAsync(CurrentGroup.Id);
        foreach (var item in data)
        {
            if (Items.Any(p => p.User.Equals(item.User)))
            {
                return;
            }

            var accVM = new UserItemViewModel(item);
            Items.Add(accVM);
        }

        IsCurrentGroupEmpty = Items.Count == 0;
        _cache.Remove(CurrentGroup.Id);
        _cache.Add(CurrentGroup.Id, Items.ToList());
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestFollowsFailed)}\n{errorMsg}";

    [RelayCommand]
    private async Task SelectGroupAsync(FollowGroup group)
    {
        CurrentGroup = group;
        TryClear(Items);
        if (_cache.TryGetValue(group.Id, out var cache))
        {
            cache.ToList().ForEach(p => Items.Add(p));
            IsCurrentGroupEmpty = Items.Count == 0;
        }
        else
        {
            await GetDataAsync();
        }
    }
}
