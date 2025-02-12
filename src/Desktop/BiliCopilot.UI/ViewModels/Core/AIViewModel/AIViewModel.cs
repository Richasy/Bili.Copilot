// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Chat;
using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;
using Windows.ApplicationModel.DataTransfer;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// AI 视图模型.
/// </summary>
public sealed partial class AIViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIViewModel"/> class.
    /// </summary>
    public AIViewModel(
        ILogger<AIViewModel> logger,
        DispatcherQueue dispatcherQueue)
    {
        _logger = logger;
        _dispatcherQueue = dispatcherQueue;
    }

    /// <summary>
    /// 注入视频信息.
    /// </summary>
    public async void InjectVideoAsync(VideoPlayerView videoView, VideoPart videoPart)
    {
        Cancel();
        _videoView = videoView;
        _videoPart = videoPart;
        var desc = _videoView.Information.GetExtensionIfNotNull<string>(VideoExtensionDataId.Description);
        SourceCover = _videoView.Information.Identifier.Cover.Uri;
        SourceTitle = _videoView.Information.Identifier.Title;
        SourceSubtitle = string.IsNullOrEmpty(desc) ? _videoPart.Identifier.Title : desc;
        _subtitles = default;
        await InitializeVideoPromptsAsync();
    }

    /// <summary>
    /// 注入文章信息.
    /// </summary>
    public async void InjectArticleAsync(ArticleDetail article)
    {
        Cancel();
        _articleDetail = article;
        SourceCover = article.Identifier.Cover?.Uri;
        SourceTitle = article.Identifier.Title;
        SourceSubtitle = article.Identifier.Summary;
        await InitializeArticlePromptsAsync();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Services is null)
        {
            await ReloadAvailableServicesAsync();
        }

        CheckQuickItemsShown();
    }

    [RelayCommand]
    private void Clear()
    {
        SelectedService = default;
        SelectedModel = default;
        _dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            if (Services is List<ChatServiceItemViewModel> services)
            {
                services.Clear();
                Services = default;
            }

            if (Models is List<ChatModelItemViewModel> models)
            {
                models.Clear();
                Models = default;
            }
        });
    }

    [RelayCommand]
    private async Task ReloadAvailableServicesAsync()
    {
        var providers = Enum.GetValues<ChatProviderType>();
        var services = new List<ChatServiceItemViewModel>();
        var chatConfigManager = this.Get<IChatConfigManager>();
        foreach (var p in providers)
        {
            var config = await chatConfigManager.GetChatConfigAsync(p);
            if (config is not null && config.IsValid())
            {
                services.Add(new ChatServiceItemViewModel(p));
            }
        }

        Services = services;
        IsNoService = services.Count == 0;

        if (!IsNoService)
        {
            var lastSelectService = SettingsToolkit.ReadLocalSetting(UI.Models.Constants.SettingNames.LastSelectedAIService, ChatProviderType.OpenAI.ToString());
            var service = Services.FirstOrDefault(p => p.ProviderType.ToString() == lastSelectService) ?? Services.FirstOrDefault();
            SelectServiceCommand.Execute(service);
        }
    }

    [RelayCommand]
    private async Task SelectServiceAsync(ChatServiceItemViewModel service)
    {
        if (SelectedService == service)
        {
            return;
        }

        SelectedService = service;
        if (service is null)
        {
            Models = default;
            IsNoModel = true;
            return;
        }

        SettingsToolkit.WriteLocalSetting(UI.Models.Constants.SettingNames.LastSelectedAIService, service.ProviderType.ToString());
        var config = await this.Get<IChatConfigManager>().GetChatConfigAsync(service.ProviderType);
        var customModels = config?.CustomModels ?? [];
        var preModels = GlobalDependencies.Kernel.GetRequiredService<IChatService>(service.ProviderType.ToString()).GetPredefinedModels();
        var models = preModels.ToList();
        models.AddRange(customModels);
        Models = models.ConvertAll(p => new ChatModelItemViewModel(p));
        IsNoModel = Models.Count == 0;
        if (!IsNoModel)
        {
            var lastSelectModel = this.Get<ISettingsToolkit>().ReadLocalSetting($"LastSelected{service.ProviderType}Model", string.Empty);
            var model = Models.FirstOrDefault(p => p.Id == lastSelectModel) ?? Models.FirstOrDefault();
            SelectModel(model);
        }
        else
        {
            SelectedModel = default;
        }

        CheckQuickItemsShown();
    }

    [RelayCommand]
    private void SelectModel(ChatModelItemViewModel model)
    {
        if (model is null || SelectedModel == model)
        {
            return;
        }

        SelectedModel = model;
    }

    [RelayCommand]
    private void Discard()
    {
        Erase();
        RequestText = string.Empty;
        IsGenerating = false;
        _currentPrompt = default;
        CheckQuickItemsShown();
    }

    [RelayCommand]
    private void Regenerate()
    {
        if (_currentPrompt is not null)
        {
            _currentPrompt.ExecuteCommand.Execute(default);
        }
        else if (!string.IsNullOrEmpty(_lastQuestion))
        {
            SendQuestionCommand.Execute(_lastQuestion);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        if (_generateCancellationTokenSource is not null)
        {
            _generateCancellationTokenSource.Cancel();
            _generateCancellationTokenSource.Dispose();
            _generateCancellationTokenSource = default;
        }

        ProgressTip = default;
        IsGenerating = false;
        Discard();
    }

    [RelayCommand]
    private void CopyAnswer()
    {
        if (string.IsNullOrEmpty(FinalResult))
        {
            return;
        }

        var dp = new DataPackage();
        dp.SetText(FinalResult);
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private async Task SendQuestionAsync(string question)
    {
        if (_videoView is not null)
        {
            await AskVideoQuestionAsync(question);
        }
        else if (_articleDetail is not null)
        {
            await AskArticleQuestionAsync(question);
        }

        _lastQuestion = question;
    }

    private void Erase()
    {
        FinalResult = string.Empty;
        ErrorMessage = string.Empty;
        TempResult = string.Empty;
        _lastQuestion = string.Empty;
    }

    private void CheckQuickItemsShown()
        => IsQuickItemsVisible = !IsGenerating && !IsNoService && !IsNoModel && string.IsNullOrEmpty(FinalResult) && string.IsNullOrEmpty(RequestText);

    partial void OnIsGeneratingChanged(bool value)
        => CheckQuickItemsShown();

    partial void OnErrorMessageChanged(string value)
        => CheckQuickItemsShown();

    partial void OnFinalResultChanged(string value)
        => CheckQuickItemsShown();

    partial void OnSelectedModelChanged(ChatModelItemViewModel value)
    {
        if (value is not null && SelectedService is not null)
        {
            this.Get<ISettingsToolkit>().WriteLocalSetting($"LastSelected{SelectedService.ProviderType}Model", value.Id);
        }
    }
}
