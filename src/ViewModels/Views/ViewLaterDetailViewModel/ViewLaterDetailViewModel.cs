// Copyright (c) Bili Copilot. All rights reserved.

using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Local;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 稍后再看详情视图模型.
/// </summary>
public sealed partial class ViewLaterDetailViewModel : InformationFlowViewModel<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterDetailViewModel"/> class.
    /// </summary>
    public ViewLaterDetailViewModel()
    {
        AttachIsRunningToAsyncCommand(p => IsClearing = p, ClearAllCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, ClearAllCommand);
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        AccountProvider.Instance.ResetViewLaterStatus();
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

        var data = await AccountProvider.Instance.GetViewLaterListAsync();
        foreach (var item in data.Items)
        {
            if (Items.Any(p => p.Data.Equals(item)))
            {
                continue;
            }

            var videoVM = new VideoItemViewModel(item, additionalAction: RemoveVideo);
            Items.Add(videoVM);
        }

        _isEnd = Items.Count == data.TotalCount;
        IsEmpty = Items.Count == 0;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestViewLaterFailed)}\n{errorMsg}";

    private static PlaySnapshot GetSnapshot(VideoItemViewModel vm)
    {
        var info = vm.Data;
        return new PlaySnapshot(info.Identifier.Id, "0", VideoType.Video);
    }

    [RelayCommand]
    private async Task ClearAllAsync()
    {
        var result = await AccountProvider.ClearViewLaterAsync();
        if (result)
        {
            TryClear(Items);
            _ = ReloadCommand.ExecuteAsync(null);
        }
    }

    [RelayCommand]
    private void PlayAll()
    {
        if (Items.Count > 1)
        {
            AppViewModel.Instance.OpenPlaylistCommand.Execute(Items.Select(p => p.Data).ToList());
        }
        else if (Items.Count > 0)
        {
            AppViewModel.Instance.OpenPlayerCommand.Execute(GetSnapshot(Items.First()));
        }
    }

    private void RemoveVideo(VideoItemViewModel vm)
    {
        Items.Remove(vm);
        IsEmpty = Items.Count == 0;
    }
}
