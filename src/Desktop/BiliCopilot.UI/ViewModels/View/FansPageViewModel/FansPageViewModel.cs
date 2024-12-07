// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.User;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 粉丝页面视图模型.
/// </summary>
public sealed partial class FansPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FansPageViewModel"/> class.
    /// </summary>
    public FansPageViewModel(
        IRelationshipService service,
        ILogger<FansPageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Users.Count > 0 || IsEmpty)
        {
            return;
        }

        await LoadUsersAsync();
    }

    [RelayCommand]
    private void Refresh()
    {
        IsEmpty = false;
        _pageNumber = 0;
        TotalCount = 0;
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            Users.Clear();
            await InitializeAsync();
        });
    }

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        if (IsUserLoading || IsEmpty || (Users.Count >= TotalCount && Users.Count > 0))
        {
            return;
        }

        IsUserLoading = true;
        try
        {
            var (users, totalCount, nextPage) = await _service.GetMyFansAsync(_pageNumber);
            if (Users.Count == 0 && (users.Count == 0 || totalCount == 0))
            {
                IsEmpty = true;
            }
            else
            {
                _pageNumber = nextPage;
                TotalCount = totalCount;
                foreach (var item in users)
                {
                    Users.Add(new UserItemViewModel(item));
                }

                UserListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "无法获取粉丝列表");
        }
        finally
        {
            IsUserLoading = false;
            IsEmpty = Users.Count == 0;
        }
    }
}
