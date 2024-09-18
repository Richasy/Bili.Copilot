// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 用户动态详情视图模型.
/// </summary>
public sealed partial class UserMomentDetailViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserMomentDetailViewModel"/> class.
    /// </summary>
    public UserMomentDetailViewModel(
        IMomentDiscoveryService service,
        ILogger<UserMomentDetailViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// 设置是否为视频动态.
    /// </summary>
    public void SetIsVideo(bool isVideo)
    {
        _isVideo = isVideo;
        Title = isVideo ? ResourceToolkit.GetLocalizedString(StringNames.Video) : ResourceToolkit.GetLocalizedString(StringNames.Comprehensive);
    }

    /// <summary>
    /// 设置显示评论的动作.
    /// </summary>
    public void SetShowCommentAction(Action<MomentItemViewModel> action)
        => _showCommentAction = action;

    /// <summary>
    /// 是否为视频动态模块.
    /// </summary>
    /// <returns>结果.</returns>
    public bool IsVideo()
        => _isVideo;

    [RelayCommand]
    private void Reset(UserProfile profile)
    {
        Clear();
        _user = profile;
    }

    [RelayCommand]
    private async Task InitializeAsync()
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
        Clear();
        await LoadItemsAsync();
    }

    [RelayCommand]
    private void Clear()
    {
        _offset = default;
        IsEmpty = false;
        _preventLoadMore = false;
        Items = new System.Collections.ObjectModel.ObservableCollection<MomentItemViewModel>();
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
            var (moments, offset, hasMore) = _isVideo
                ? await _service.GetUserVideoMomentsAsync(_user, _offset)
                : await _service.GetUserComprehensiveMomentsAsync(_user, _offset);
            _offset = offset;
            _preventLoadMore = !hasMore || string.IsNullOrEmpty(offset);
            if (moments is not null)
            {
                foreach (var item in moments.Select(p => new MomentItemViewModel(p, _isVideo ? MomentCardStyle.Video : MomentCardStyle.Comprehensive, _showCommentAction)))
                {
                    Items.Add(item);
                }

                ListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            _preventLoadMore = true;
            _logger.LogError(ex, "加载动态列表失败.");
        }
        finally
        {
            IsEmpty = Items.Count == 0;
            IsLoading = false;
        }
    }
}
