// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 关注页面视图模型.
/// </summary>
public sealed partial class FollowsPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FollowsPageViewModel"/> class.
    /// </summary>
    public FollowsPageViewModel(
        IRelationshipService service,
        ILogger<FollowsPageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(FollowsPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Groups.Count > 0)
        {
            return;
        }

        await LoadGroupsAsync();
        await Task.Delay(200);
        var lastSelectedGroupId = SettingsToolkit.ReadLocalSetting(SettingNames.FollowsPageLastSelectedGroupId, string.Empty);
        var group = Groups.FirstOrDefault(p => p.Id == lastSelectedGroupId) ?? Groups.FirstOrDefault(p => p.TotalCount > 0) ?? Groups.First();
        SelectGroupCommand.Execute(group);
        GroupInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectGroup(UserGroup group)
    {
        if (group == null || group == SelectedGroup)
        {
            return;
        }

        if (Users.Count > 0)
        {
            _userCache[SelectedGroup] = Users.ToList();
        }

        SelectedGroup = group;
        SettingsToolkit.WriteLocalSetting(SettingNames.FollowsPageLastSelectedGroupId, group.Id);

        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            Users.Clear();
            if (_userCache.TryGetValue(group, out var list))
            {
                foreach (var user in list)
                {
                    Users.Add(user);
                }

                UserListUpdated?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                await LoadUsersAsync();
            }

            IsEmpty = Users.Count == 0;
        });
    }

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        if (IsUserLoading || SelectedGroup is null)
        {
            return;
        }

        if (Users.Count >= SelectedGroup.TotalCount)
        {
            return;
        }

        IsUserLoading = true;
        try
        {
            var pn = 0;
            if (_offsetCache.TryGetValue(SelectedGroup, out var value))
            {
                pn = value;
            }

            var (users, nextOffset) = await _service.GetMyFollowUserGroupDetailAsync(SelectedGroup.Id, pn);
            _offsetCache[SelectedGroup] = nextOffset;
            if (users != null)
            {
                foreach (var item in users)
                {
                    Users.Add(new UserItemViewModel(item));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载关注用户时出错.");
        }
        finally
        {
            IsUserLoading = false;
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            Users.Clear();
            Groups.Clear();
            _userCache.Clear();
            _offsetCache.Clear();
            SelectedGroup = default;
            await InitializeAsync();
        });
    }

    private async Task LoadGroupsAsync()
    {
        IsGroupLoading = true;
        try
        {
            var groups = await _service.GetMyFollowUserGroupsAsync();
            if (groups != null)
            {
                foreach (var item in groups)
                {
                    Groups.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载关注分组时出错.");
        }
        finally
        {
            IsGroupLoading = false;
        }
    }
}
