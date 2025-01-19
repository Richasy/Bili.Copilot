// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliAgent.Interfaces;
using BiliAgent.Models;
using BiliCopilot.UI.Controls.AI;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUI.Share.Base;
using Richasy.WinUI.Share.ViewModels;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// AI服务项目视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class AIServiceItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIServiceItemViewModel"/> class.
    /// </summary>
    public AIServiceItemViewModel(
        ProviderType providerType)
    {
        ProviderType = providerType;
        Name = GetProviderName();

        var serverModels = this.Get<IAgentClient>()
            .GetPredefinedModels(ProviderType);
        ServerModels.Clear();
        serverModels.ToList().ForEach(p => ServerModels.Add(new ChatModelItemViewModel(p)));
        IsServerModelVisible = ServerModels.Count > 0;
        CheckCustomModelsCount();
    }

    /// <summary>
    /// 自定义模型.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> CustomModels { get; } = new();

    /// <summary>
    /// 服务模型.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> ServerModels { get; } = new();

    /// <summary>
    /// 设置配置.
    /// </summary>
    /// <param name="config">配置内容.</param>
    public void SetConfig(ClientConfigBase config)
    {
        Config = config;
        if (config?.IsCustomModelNotEmpty() ?? false)
        {
            CustomModels.Clear();
            config.CustomModels.ForEach(p => CustomModels.Add(new ChatModelItemViewModel(p, DeleteCustomModel)));
            CheckCustomModelsCount();
        }

        CheckCurrentConfig();
    }

    /// <summary>
    /// 检查当前配置是否有效.
    /// </summary>
    public void CheckCurrentConfig()
        => IsCompleted = Config?.IsValid() ?? false;

    /// <summary>
    /// 模型是否已存在于列表之中.
    /// </summary>
    /// <param name="model">模型.</param>
    /// <returns>是否存在.</returns>
    public bool IsModelExist(ChatModel model)
        => CustomModels.Any(p => p.Id == model.Id) || ServerModels.Any(p => p.Id == model.Id);

    /// <summary>
    /// 添加自定义模型.
    /// </summary>
    /// <param name="model">模型.</param>
    public void AddCustomModel(ChatModel model)
    {
        if (IsModelExist(model))
        {
            return;
        }

        CustomModels.Add(new ChatModelItemViewModel(model, DeleteCustomModel));
        Config.CustomModels ??= new();
        Config.CustomModels.Add(model);
        CheckCustomModelsCount();
        CheckCurrentConfig();
    }

    [RelayCommand]
    private async Task CreateCustomModelAsync()
    {
        var dialog = new CustomChatModelDialog();
        var dialogResult = await dialog.ShowAsync();
        if (dialogResult == ContentDialogResult.Primary)
        {
            var model = dialog.Model;
            if (model == null)
            {
                return;
            }

            if (IsModelExist(model))
            {
                this.Get<AppViewModel>()
                    .ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.ModelAlreadyExist), InfoType.Error));
                return;
            }

            AddCustomModel(model);
        }
    }

    private void DeleteCustomModel(ChatModelItemViewModel model)
    {
        CustomModels.Remove(model);
        Config.CustomModels?.Remove(model.Data);
        CheckCustomModelsCount();
        CheckCurrentConfig();
    }

    private void CheckCustomModelsCount()
        => IsCustomModelsEmpty = CustomModels.Count == 0;

    private string GetProviderName()
    {
        return ProviderType switch
        {
            ProviderType.OpenAI => "Open AI",
            ProviderType.AzureOpenAI => "Azure Open AI",
            ProviderType.Gemini => "Gemini",
            ProviderType.Anthropic => "Anthropic",
            ProviderType.DeepSeek => "Deep Seek",
            ProviderType.SiliconFlow => "Silicon Clound",
            ProviderType.OpenRouter => "Open Router",
            ProviderType.Moonshot => "月之暗面",
            ProviderType.ZhiPu => "智谱 AI",
            ProviderType.LingYi => "零一万物",
            ProviderType.Qwen => "通义千问",
            ProviderType.Ernie => "文心一言",
            ProviderType.Hunyuan => "腾讯混元",
            ProviderType.Spark => "讯飞星火",
            ProviderType.TogetherAI => "Together AI",
            ProviderType.Groq => "Groq",
            ProviderType.Perplexity => "Perplexity",
            ProviderType.Mistral => "Mistral AI",
            ProviderType.Ollama => "Ollama",
            ProviderType.Doubao => "字节豆包",
            ProviderType.XAI => "xAI",
            _ => throw new NotImplementedException(),
        };
    }
}
