// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 稍后再看视图模型.
/// </summary>
public sealed partial class ViewLaterPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterPageViewModel"/> class.
    /// </summary>
    public ViewLaterPageViewModel(
        IViewLaterService service,
        ILogger<ViewLaterPageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Videos.Count > 0 || IsEmpty)
        {
            return;
        }

        await LoadVideosAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsEmpty = false;
        _pageNumber = 0;
        TotalCount = 0;
        Videos.Clear();
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task LoadVideosAsync()
    {
        if (IsLoading || IsEmpty || (Videos.Count >= TotalCount && Videos.Count > 0))
        {
            return;
        }

        IsLoading = true;
        try
        {
            var (videos, totalCount, nextPage) = await _service.GetViewLaterSetAsync(_pageNumber);
            if (Videos.Count == 0 && (videos is null || videos.Count == 0 || totalCount == 0))
            {
                IsEmpty = true;
            }
            else
            {
                _pageNumber = nextPage;
                TotalCount = totalCount;
                foreach (var item in videos)
                {
                    Videos.Add(new VideoItemViewModel(item, Models.Constants.VideoCardStyle.ViewLater));
                }

                ListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载稍后再看时出现异常");
        }
        finally
        {
            IsLoading = false;
            IsEmpty = Videos.Count == 0;
        }
    }

    [RelayCommand]
    private async Task ClearAllAsync()
    {
        try
        {
            await _service.CleanAsync(Richasy.BiliKernel.Models.ViewLaterCleanType.All);
            await Task.Delay(500);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除稍后再看时出错");
        }
    }
}
