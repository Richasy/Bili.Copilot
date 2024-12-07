// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Models.Moment;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 动态点赞者条目视图模型.
/// </summary>
public sealed partial class MomentUperSectionViewModel : ViewModelBase<MomentProfile>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentUperSectionViewModel"/> class.
    /// </summary>
    public MomentUperSectionViewModel(MomentProfile data)
        : base(data)
    {
        _service = this.Get<IMomentDiscoveryService>();
        _logger = this.Get<ILogger<MomentUperSectionViewModel>>();
        IsTotal = data is null;
        Avatar = data?.User?.Avatar.Uri;
        Name = data?.User?.Name ?? ResourceToolkit.GetLocalizedString(StringNames.AllDynamic);
        HasUnread = data?.IsUnread ?? false;
    }

    internal void InjectFirstPageData(MomentView momentView)
    {
        IsEmpty = false;
        _preventLoadMore = momentView.HasMoreMoments != true;
        _offset = momentView.Offset;
        _baseline = momentView.UpdateBaseline;
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            Items.Clear();
            foreach (var item in momentView.Moments)
            {
                Items.Add(new MomentItemViewModel(item, MomentCardStyle.Comprehensive, ShowComment));
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        });
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
    private void Refresh()
    {
        if (IsTotal)
        {
            throw new Exception("请在外层视图模型中刷新综合动态，以便更新 UP 列表");
        }

        _offset = default;
        _baseline = default;
        IsEmpty = false;
        _preventLoadMore = false;
        Items.Clear();

        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            await LoadItemsAsync();
        });
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
            if (IsTotal)
            {
                await LoadComprehensiveItemsAsync();
            }
            else
            {
                await LoadUperItemsAsync();
                if (HasUnread)
                {
                    HasUnread = false;
                }
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

    private async Task LoadComprehensiveItemsAsync()
    {
        var view = await _service.GetComprehensiveMomentsAsync(_offset, _baseline);
        if (view is not null)
        {
            _offset = view.Offset;
            _baseline = view.UpdateBaseline;
            _preventLoadMore = view.HasMoreMoments != true;
            foreach (var item in view.Moments.Select(p => new MomentItemViewModel(p, MomentCardStyle.Comprehensive, ShowComment)))
            {
                Items.Add(item);
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    private async Task LoadUperItemsAsync()
    {
        var (moments, offset, hasMore) = await _service.GetUserMomentsAsync(Data.User, _offset);
        _offset = offset;
        _preventLoadMore = !hasMore || string.IsNullOrEmpty(offset);
        if (moments is not null)
        {
            foreach (var item in moments.Select(p => new MomentItemViewModel(p, MomentCardStyle.Comprehensive, ShowComment)))
            {
                Items.Add(item);
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ShowComment(MomentItemViewModel item)
        => this.Get<MomentPageViewModel>().ShowCommentCommand.Execute(item.Data);
}
