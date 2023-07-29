// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Dynamic;

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
        }
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestDynamicFailed)}\n{errorMsg}";

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if ((IsVideoShown && _isVideoEnd)
            || (IsAllShown && _isAllEnd))
        {
            return;
        }

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

        var dynamicInfos = Items
            .OfType<DynamicItemViewModel>()
            .Select(p => p.Data)
            .ToList();
        _caches[CurrentType] = dynamicInfos;

        IsEmpty = Items.Count == 0;
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
}
