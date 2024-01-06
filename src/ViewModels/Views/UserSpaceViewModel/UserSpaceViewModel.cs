// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 用户空间视图模型.
/// </summary>
public sealed partial class UserSpaceViewModel : InformationFlowViewModel<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSpaceViewModel"/> class.
    /// </summary>
    public UserSpaceViewModel()
    {
        SearchVideos = new ObservableCollection<VideoItemViewModel>();

        AttachExceptionHandlerToAsyncCommand(DisplayException, SearchCommand);
        AttachIsRunningToAsyncCommand(p => IsSearching = p, SearchCommand);
        IsMainShown = true;
        Fans = new FansDetailViewModel();
        Follows = new FollowsDetailViewModel();
    }

    /// <summary>
    /// 设置用户资料.
    /// </summary>
    /// <param name="user">用户信息.</param>
    public void SetUserProfile(UserProfile user)
    {
        _userProfile = user;
        IsMe = user.Id == AccountProvider.Instance.UserId.ToString();
        Fans.SetProfile(user);
        Follows.SetProfile(user);
        TryClear(Items);
        BeforeReload();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        UserViewModel = null;
        _isSpaceVideoFinished = false;
        IsSpaceVideoEmpty = false;
        ExitSearchMode();
        AccountProvider.Instance.ResetSpaceVideoStatus(_userProfile.Id);
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (IsRequestModeFinished())
        {
            return;
        }

        if (UserViewModel == null || !UserViewModel.Data.Equals(_userProfile))
        {
            // 请求用户数据.
            var view = await AccountProvider.Instance.GetUserSpaceInformationAsync(_userProfile.Id);
            var userVM = new UserItemViewModel(view.Account);
            UserViewModel = userVM;
            LoadVideoSet(view.VideoSet);
        }
        else
        {
            if (IsSearchMode)
            {
                await RequestSearchVideosAsync();
            }
            else
            {
                var videoSet = await AccountProvider.Instance.GetUserSpaceVideoSetAsync(_userProfile.Id);
                LoadVideoSet(videoSet);
            }
        }
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestUserInformationFailed)}\n{errorMsg}";

    [RelayCommand]
    private void EnterSearchMode()
        => IsSearchMode = true;

    [RelayCommand]
    private void ExitSearchMode()
    {
        IsSearchMode = false;
        Keyword = string.Empty;
        ClearSearchData();
    }

    private void ClearSearchData()
    {
        _requestKeyword = string.Empty;
        _isSearchVideoFinished = false;
        IsSearchVideoEmpty = false;
        AccountProvider.Instance.ResetSpaceSearchStatus(_userProfile.Id);
        TryClear(SearchVideos);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrEmpty(Keyword))
        {
            return;
        }

        _requestKeyword = Keyword;
        await RequestSearchVideosAsync();
    }

    [RelayCommand]
    private void OpenFans()
    {
        if (IsMe)
        {
            AppViewModel.Instance.ShowFansCommand.Execute(_userProfile);
        }
        else
        {
            IsInFans = true;
            Fans.InitializeCommand.Execute(default);
        }
    }

    [RelayCommand]
    private void OpenFollows()
    {
        if (IsMe)
        {
            AppViewModel.Instance.ShowFollowsCommand.Execute(default);
        }
        else
        {
            IsInFollows = true;
            Follows.InitializeCommand.Execute(default);
        }
    }

    private async Task RequestSearchVideosAsync()
    {
        if (string.IsNullOrEmpty(_requestKeyword))
        {
            return;
        }

        var data = await AccountProvider.Instance.SearchUserSpaceVideoAsync(_userProfile.Id, _requestKeyword);
        LoadVideoSet(data);
    }

    private void LoadVideoSet(VideoSet set)
    {
        var collection = IsSearchMode ? SearchVideos : Items;
        foreach (var item in set.Items)
        {
            var videoVM = new VideoItemViewModel(item);
            collection.Add(videoVM);
        }

        var isFinished = set.TotalCount <= collection.Count;
        if (IsSearchMode)
        {
            IsSearchVideoEmpty = SearchVideos.Count == 0;
            _isSearchVideoFinished = isFinished;
        }
        else
        {
            IsSpaceVideoEmpty = Items.Count == 0;
            _isSpaceVideoFinished = isFinished;
        }
    }

    private bool IsRequestModeFinished()
        => IsSearchMode ? _isSearchVideoFinished : _isSpaceVideoFinished;

    private void CheckMainShown()
        => IsMainShown = !IsInFans && !IsInFollows;

    partial void OnKeywordChanged(string value)
    {
        CanSearch = !string.IsNullOrEmpty(value);
        ClearSearchData();
        TryClear(SearchVideos);
        if (CanSearch && !IsSearchMode)
        {
            EnterSearchMode();
        }
        else if (!CanSearch && IsSearchMode)
        {
            ExitSearchMode();
        }
    }

    partial void OnIsInFansChanged(bool value)
        => CheckMainShown();

    partial void OnIsInFollowsChanged(bool value)
        => CheckMainShown();
}
