// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// UGC合集详情视图模型.
/// </summary>
public sealed partial class UgcSeasonFavoriteSectionDetailViewModel : ViewModelBase<VideoFavoriteFolder>, IFavoriteSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UgcSeasonFavoriteSectionDetailViewModel"/> class.
    /// </summary>
    public UgcSeasonFavoriteSectionDetailViewModel(
        VideoFavoriteFolder data,
        IFavoriteService service)
        : base(data)
    {
        _service = service;
        _logger = this.Get<ILogger<UgcSeasonFavoriteSectionDetailViewModel>>();
        Cover = data.Cover?.Uri;
    }

    [RelayCommand]
    private async Task TryFirstLoadAsync()
    {
        if (Items.Count > 0 || _preventLoadMore)
        {
            return;
        }

        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsEmpty = false;
        Items.Clear();
        _preventLoadMore = false;
        _pageNumber = 0;
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsEmpty || IsLoading || _preventLoadMore)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var (detail, total, nextPn) = await _service.GetUgcSeasonVideosAsync(Data, _pageNumber);
            _preventLoadMore = detail is null || nextPn is null;
            _pageNumber = nextPn ?? 0;
            TotalCount = total;
            if (!_preventLoadMore || detail.Count > 0)
            {
                foreach (var item in detail)
                {
                    Items.Add(new VideoItemViewModel(item, Models.Constants.VideoCardStyle.Favorite));
                }

                ListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            _preventLoadMore = false;
            _logger.LogError(ex, $"获取合集收藏列表时出错.");
        }
        finally
        {
            IsEmpty = Items.Count == 0;
            IsLoading = false;
        }
    }
}
