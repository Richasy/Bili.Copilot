// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 用户动态页视图模型.
/// </summary>
public sealed partial class UserSpacePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSpacePageViewModel"/> class.
    /// </summary>
    public UserSpacePageViewModel(
        CommentMainViewModel comment,
        IRelationshipService relationshipService,
        IUserService userService,
        ISearchService searchService,
        ILogger<UserSpacePageViewModel> logger)
    {
        CommentModule = comment;
        _relationshipService = relationshipService;
        _userService = userService;
        _searchService = searchService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task InitializeAsync(UserProfile profile)
    {
        _profile = profile;
        Card = default;
        UserName = profile.Name;
        if (IsSearchMode)
        {
            ExitSearch();
        }

        if (Sections is null)
        {
            Sections = new List<UserMomentDetailViewModel>
            {
                this.Get<UserMomentDetailViewModel>(),
                this.Get<UserMomentDetailViewModel>(),
            };

            Sections.ElementAt(0).SetIsVideo(true);
            Sections.ElementAt(1).SetIsVideo(false);
        }

        foreach (var item in Sections)
        {
            item.SetShowCommentAction(ShowComment);
            item.ResetCommand.Execute(profile);
        }

        if (SelectedSection is null)
        {
            var isLastVideoSection = SettingsToolkit.ReadLocalSetting(SettingNames.LastUserSpaceSectionIsVideo, true);
            var section = isLastVideoSection ? Sections.First() : Sections.Last();
            SelectSection(section);
        }
        else
        {
            SelectedSection.InitializeCommand.Execute(default);
        }

        await InitializeRelationAsync();

        Initialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Search(string text)
    {
        ExitSearch();
        SearchKeyword = text;
        IsSearchMode = true;
        LoadMoreSearchResultCommand.Execute(default);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (IsSearchMode)
        {
            var keyword = SearchKeyword;
            SearchCommand.Execute(keyword);
            return;
        }

        await SelectedSection?.RefreshCommand.ExecuteAsync(default);
    }

    [RelayCommand]
    private void SelectSection(UserMomentDetailViewModel section)
    {
        if (section is null || section == SelectedSection)
        {
            return;
        }

        SelectedSection = section;
        SettingsToolkit.WriteLocalSetting(SettingNames.LastUserSpaceSectionIsVideo, SelectedSection.IsVideo());
        SelectedSection.InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private async Task ToggleFollowAsync()
    {
        if (IsFollowed)
        {
            try
            {
                await _relationshipService.UnfollowUserAsync(_profile.Id);
                IsFollowed = false;
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Unfollowed), InfoType.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消关注用户时失败");
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToUnfollowUser), InfoType.Error));
            }
        }
        else
        {
            try
            {
                await _relationshipService.FollowUserAsync(_profile.Id);
                IsFollowed = true;
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Followed), InfoType.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "关注用户时失败");
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToFollowUser), InfoType.Error));
            }
        }
    }

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem(_profile.Id, _profile.Name, _profile.Avatar.Uri.ToString(), PinContentType.User);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    [RelayCommand]
    private async Task InitializeUserInformationAsync()
    {
        try
        {
            Card ??= await _userService.GetUserInformationAsync(_profile.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户信息时失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToGetUserInformation), InfoType.Error));
        }
    }

    [RelayCommand]
    private async Task LoadMoreSearchResultAsync()
    {
        if (_preventLoadMoreSearch || IsSearching || !IsSearchMode || string.IsNullOrEmpty(SearchKeyword))
        {
            return;
        }

        try
        {
            IsSearching = true;
            _searchCancellationTokenSource = new CancellationTokenSource();
            var (videos, total, hasMore) = await _searchService.SearchUserVideosAsync(_profile.Id, SearchKeyword, _searchPn, _searchCancellationTokenSource.Token);
            _preventLoadMoreSearch = !hasMore || videos is null || videos.Count == 0;
            if (videos is not null)
            {
                foreach (var item in videos)
                {
                    SearchVideos.Add(new VideoItemViewModel(item, VideoCardStyle.Search));
                }

                SearchUpdated?.Invoke(this, EventArgs.Empty);
            }

            if (!_preventLoadMoreSearch)
            {
                _searchPn++;
            }
        }
        catch (TaskCanceledException)
        {
            // Do nothing.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索用户视频时失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToSearchUserVideos), InfoType.Error));
        }
        finally
        {
            IsSearchEmpty = SearchVideos.Count == 0;
            IsSearching = false;
        }
    }

    [RelayCommand]
    private void ExitSearch()
    {
        IsSearchMode = false;
        SearchKeyword = string.Empty;
        IsSearchEmpty = false;
        SearchVideos.Clear();
        _preventLoadMoreSearch = false;
        _searchPn = 0;
        CancelSearch();
    }

    private async Task InitializeRelationAsync()
    {
        try
        {
            var state = await _relationshipService.GetRelationshipAsync(_profile.Id);
            IsFollowed = state != UserRelationStatus.Unknown && state != UserRelationStatus.Unfollow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户关系时失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToGetUserRelation), InfoType.Error));
        }
    }

    private void ShowComment(MomentItemViewModel data)
    {
        var moment = data.Data;
        if (CommentModule.Id == moment.CommentId)
        {
            return;
        }

        IsCommentsOpened = true;
        CommentModule.Initialize(moment.CommentId, moment.CommentType!.Value, Richasy.BiliKernel.Models.CommentSortType.Hot);
        CommentModule.RefreshCommand.Execute(default);
    }

    private void CancelSearch()
    {
        if (_searchCancellationTokenSource is not null)
        {
            _searchCancellationTokenSource.Cancel();
            _searchCancellationTokenSource.Dispose();
            _searchCancellationTokenSource = null;
        }
    }
}
