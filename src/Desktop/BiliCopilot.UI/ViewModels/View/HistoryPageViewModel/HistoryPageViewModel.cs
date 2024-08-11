// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 历史记录页面视图模型.
/// </summary>
public sealed partial class HistoryPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryPageViewModel"/> class.
    /// </summary>
    public HistoryPageViewModel(
        IViewHistoryService service,
        ILogger<HistoryPageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    [RelayCommand]
    private void Initialize()
    {
        if (Sections is not null)
        {
            return;
        }

        var types = Enum.GetValues<ViewHistoryTabType>().Where(p => p != ViewHistoryTabType.All).ToList();
        var sections = new List<IHistorySectionDetailViewModel>();
        foreach (var type in types)
        {
            var vm = type switch
            {
                ViewHistoryTabType.Video => (IHistorySectionDetailViewModel)new VideoHistorySectionDetailViewModel(_service),
                ViewHistoryTabType.Article => new ArticleHistorySectionDetailViewModel(_service),
                ViewHistoryTabType.Live => new LiveHistorySectionDetailViewModel(_service),
                _ => null,
            };

            if (vm is not null)
            {
                sections.Add(vm);
            }
        }

        Sections = sections;
        SelectSection(Sections.First());
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (SelectedSection is null)
        {
            return;
        }

        await SelectedSection.RefreshCommand.ExecuteAsync(this);
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        if (SelectedSection is null)
        {
            return;
        }

        await _service.CleanHistoryAsync(SelectedSection.Type);
        await Task.Delay(500);
        await SelectedSection.RefreshCommand.ExecuteAsync(this);
    }

    [RelayCommand]
    private void SelectSection(IHistorySectionDetailViewModel section)
    {
        if (section is null || section == SelectedSection)
        {
            return;
        }

        SelectedSection = section;
        section.TryFirstLoadCommand.Execute(this);
    }
}
