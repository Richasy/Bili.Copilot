// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 综合动态分区详情视图模型.
/// </summary>
public sealed partial class ComprehensiveMomentSectionDetailViewModel : LayoutPageViewModelBase, IMomentSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComprehensiveMomentSectionDetailViewModel"/> class.
    /// </summary>
    public ComprehensiveMomentSectionDetailViewModel(
        IMomentDiscoveryService service)
    {
        _service = service;
        _logger = this.Get<ILogger<VideoMomentSectionDetailViewModel>>();
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => "ComprehensiveMoment";

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Upers.Count > 0)
        {
            return;
        }

        IsLoading = true;
        Upers.Add(new MomentUperSectionViewModel(default));
        await LoadUpersAsync();
        IsLoading = false;
        SelectUper(Upers.First());

        Initialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (SelectedUper.IsTotal)
        {
            Upers.Clear();
            SelectedUper?.Items?.Clear();
            SelectedUper = default;
            await InitializeAsync();
        }
        else
        {
            await SelectedUper.RefreshCommand.ExecuteAsync(default);
        }
    }

    [RelayCommand]
    private void SelectUper(MomentUperSectionViewModel item)
    {
        if (item == null)
        {
            return;
        }

        SelectedUper = item;
        SelectedUper.InitializeCommand.Execute(default);
    }

    private async Task LoadUpersAsync()
    {
        try
        {
            var response = await _service.GetComprehensiveMomentsAsync();
            var upers = response.Users;
            if (upers?.Count > 0)
            {
                foreach (var item in upers)
                {
                    Upers.Add(new MomentUperSectionViewModel(item));
                }
            }

            var total = Upers.First();
            total.InjectFirstPageData(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试获取综合动态时发生错误.");
        }
    }
}
