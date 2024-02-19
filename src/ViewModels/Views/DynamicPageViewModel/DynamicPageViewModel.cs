// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Dynamic;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels.DynamicPageViewModel;

/// <summary>
/// 动态页面的视图模型.
/// </summary>
public sealed partial class DynamicPageViewModel : InformationFlowViewModel<DynamicItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicPageViewModel"/> class.
    /// </summary>
    private DynamicPageViewModel()
    {
        _caches = new Dictionary<DynamicDisplayType, IEnumerable<DynamicInformation>>();
        DisplayUps = new ObservableCollection<UserItemViewModel>();
        UserSpaceDynamics = new ObservableCollection<DynamicItemViewModel>();
        CurrentType = SettingsToolkit.ReadLocalSetting(SettingNames.LastDynamicType, DynamicDisplayType.Video);
        CheckModuleState();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        if (IsVideoShown)
        {
            _isVideoEnd = false;
            CommunityProvider.Instance.ResetVideoDynamicStatus();
        }
        else if (IsAllShown)
        {
            _isAllEnd = false;
            CommunityProvider.Instance.ResetComprehensiveDynamicStatus();
            SelectedUp = null;
            DisplayUps.Clear();
            _allFootprint = string.Empty;
            IsAllDynamicSelected = true;
        }
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestDynamicFailed)}\n{errorMsg}";

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if ((IsVideoShown && _isVideoEnd)
            || (IsAllShown && _isAllEnd)
            || (IsAllShown && SelectedUp != null && _isCurrentUserEnd)
            || _isGettingData)
        {
            return;
        }

        _isGettingData = true;
        if (SelectedUp != null && IsAllShown)
        {
            var data = await CommunityProvider.GetUserDynamicAsync(SelectedUp.Data.Id, _userDynamicOffset);
            if (data.Dynamics != null && data.Dynamics.Count() > 0)
            {
                foreach (var item in data.Dynamics)
                {
                    var vm = new DynamicItemViewModel(item);
                    UserSpaceDynamics.Add(vm);
                }
            }

            _userDynamicOffset = data.Offset;
            _isCurrentUserEnd = !data.HasMore;
            IsCurrentSpaceEmpty = UserSpaceDynamics.Count == 0;

            try
            {
                await CommunityProvider.MarkUserDynamicReadAsync(SelectedUp.Data.Id, _userDynamicOffset, _allFootprint);
            }
            catch (System.Exception)
            {
            }
        }
        else
        {
            var data = IsVideoShown
                ? await CommunityProvider.Instance.GetDynamicVideoListAsync()
                : await CommunityProvider.Instance.GetDynamicComprehensiveListAsync();

            if (IsVideoShown)
            {
                _isVideoEnd = data.Dynamics == null || !data.Dynamics.Any();
            }
            else if (IsAllShown)
            {
                _isAllEnd = data.Dynamics == null || !data.Dynamics.Any();
            }

            if ((IsVideoShown && _isVideoEnd)
                || (IsAllShown && _isAllEnd))
            {
                IsEmpty = Items.Count == 0;
                return;
            }

            var dynamics = data.Dynamics;
            foreach (var item in dynamics)
            {
                var vm = new DynamicItemViewModel(item);
                Items.Add(vm);
            }

            if (DisplayUps.Count == 0 && IsAllShown)
            {
                foreach (var item in data.Ups)
                {
                    var vm = new UserItemViewModel(item.User);
                    vm.IsUnread = item.IsUnread;
                    DisplayUps.Add(vm);
                }
            }

            if (IsAllShown)
            {
                _allFootprint = data.Footprint;
            }

            IsNoUps = DisplayUps.Count == 0;
            var dynamicInfos = Items
                .OfType<DynamicItemViewModel>()
                .Select(p => p.Data)
                .ToList();
            _caches[CurrentType] = dynamicInfos;

            IsEmpty = Items.Count == 0;
        }

        _isGettingData = false;
    }

    [RelayCommand]
    private async Task SelectUpAsync(UserItemViewModel user)
    {
        IsAllDynamicSelected = false;
        SelectedUp = user;
        SelectedUp.IsUnread = false;
        foreach (var item in DisplayUps)
        {
            item.IsSelected = item.Equals(user);
        }

        _isCurrentUserEnd = false;
        _userDynamicOffset = string.Empty;
        TryClear(UserSpaceDynamics);

        IsAllLoading = true;
        await GetDataAsync();
        IsAllLoading = false;
        ScrollToTop();
    }

    private void CheckModuleState()
    {
        IsVideoShown = CurrentType == DynamicDisplayType.Video;
        IsAllShown = CurrentType == DynamicDisplayType.All;
    }

    partial void OnCurrentTypeChanged(DynamicDisplayType value)
    {
        CheckModuleState();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastDynamicType, value);

        if (!IsInitialized)
        {
            return;
        }

        TryClear(Items);
        if (_caches.ContainsKey(value))
        {
            var data = _caches[value];
            foreach (var item in data)
            {
                var vm = new DynamicItemViewModel(item);
                Items.Add(vm);
            }

            IsEmpty = Items.Count == 0;
        }
        else
        {
            _ = InitializeCommand.ExecuteAsync(default);
        }
    }

    partial void OnIsAllDynamicSelectedChanged(bool value)
    {
        if (value)
        {
            foreach (var item in DisplayUps)
            {
                item.IsSelected = false;
            }
        }
    }
}
