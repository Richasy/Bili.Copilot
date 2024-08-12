// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 消息页面视图模型.
/// </summary>
public sealed partial class MessagePageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePageViewModel"/> class.
    /// </summary>
    public MessagePageViewModel(
        IMessageService service,
        ILogger<MessagePageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(MessagePage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Sections.Count > 0)
        {
            RestoreSelection();
            SectionInitialized?.Invoke(this, EventArgs.Empty);
            return;
        }

        IsLoading = true;
        Sections.Add(new NotifyMessageSectionDetailViewModel(NotifyMessageType.Reply, _service));
        Sections.Add(new NotifyMessageSectionDetailViewModel(NotifyMessageType.At, _service));
        Sections.Add(new NotifyMessageSectionDetailViewModel(NotifyMessageType.Like, _service));
        IsLoading = false;
        RestoreSelection();
        await Task.CompletedTask;
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        Sections.Clear();
        await InitializeAsync();
    }

    [RelayCommand]
    private void SelectSection(IMessageSectionDetailViewModel vm)
    {
        if (vm is null || vm == CurrentSection)
        {
            return;
        }

        var sectionSettingValue = string.Empty;
        if (vm is NotifyMessageSectionDetailViewModel nfVM)
        {
            sectionSettingValue = $"nf_{nfVM.Type}";
        }

        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.LastSelectedMessageSection, sectionSettingValue);
        CurrentSection = vm;
        CurrentSection.InitializeCommand.Execute(default);
    }

    private void RestoreSelection()
    {
        var lastSelectedSection = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.LastSelectedMessageSection, string.Empty);
        if (string.IsNullOrEmpty(lastSelectedSection))
        {
            SelectSection(Sections.First());
            return;
        }

        var isSelected = false;
        if (lastSelectedSection.StartsWith("nf"))
        {
            var isSuccess = Enum.TryParse<NotifyMessageType>(lastSelectedSection.Replace("nf_", string.Empty), out var type);
            if (isSuccess)
            {
                var section = Sections.OfType<NotifyMessageSectionDetailViewModel>().FirstOrDefault(p => p.Type == type);
                if (section is not null)
                {
                    isSelected = true;
                    SelectSection(section);
                }
            }
        }
        else if (lastSelectedSection.StartsWith("chat"))
        {
            var id = lastSelectedSection.Replace("chat_", string.Empty);
        }

        if (!isSelected && CurrentSection is null)
        {
            SelectSection(Sections.First());
        }
    }
}
