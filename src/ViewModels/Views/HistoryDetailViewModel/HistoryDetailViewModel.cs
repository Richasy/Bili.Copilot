// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 历史记录详情模块的视图模型.
/// </summary>
public sealed partial class HistoryDetailViewModel : InformationFlowViewModel<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryDetailViewModel"/> class.
    /// </summary>
    private HistoryDetailViewModel()
    {
        AttachIsRunningToAsyncCommand(p => IsClearing = p, ClearAllCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, ClearAllCommand);
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        AccountProvider.Instance.ResetHistoryStatus();
        IsEmpty = false;
        _isEnd = false;
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_isEnd)
        {
            return;
        }

        var data = await AccountProvider.Instance.GetMyHistorySetAsync();
        foreach (var item in data.Items)
        {
            var videoVM = new VideoItemViewModel(item, additionalAction: RemoveVideo);
            Items.Add(videoVM);
        }

        _isEnd = data.IsFinished;
        IsEmpty = Items.Count == 0;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.RequestHistoryFailed)}\n{errorMsg}";

    [RelayCommand]
    private async Task ClearAllAsync()
    {
        var result = await AccountProvider.ClearHistoryAsync();
        if (result)
        {
            TryClear(Items);
            _ = ReloadCommand.ExecuteAsync(null);
        }
    }

    private void RemoveVideo(VideoItemViewModel vm)
    {
        _ = Items.Remove(vm);
        IsEmpty = Items.Count == 0;
    }
}
