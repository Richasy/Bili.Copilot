// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;

namespace BiliCopilot.UI.Controls.AI;

/// <summary>
/// 自定义聊天模型对话框.
/// </summary>
public sealed partial class CustomChatModelDialog : ContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomChatModelDialog"/> class.
    /// </summary>
    public CustomChatModelDialog()
    {
        InitializeComponent();
        Title = ResourceToolkit.GetLocalizedString(StringNames.CreateCustomModel);
        XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot;
        AppToolkit.ResetControlTheme(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomChatModelDialog"/> class.
    /// </summary>
    public CustomChatModelDialog(ChatModel model, bool isIdEnabled = true)
        : this()
    {
        Title = ResourceToolkit.GetLocalizedString(StringNames.ModifyCustomModel);
        ModelNameBox.Text = model.DisplayName;
        ModelIdBox.Text = model.Id;
        ModelIdBox.IsEnabled = isIdEnabled;
    }

    /// <summary>
    /// 获取或设置模型.
    /// </summary>
    public ChatModel Model { get; private set; }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var modelName = ModelNameBox.Text?.Trim() ?? string.Empty;
        var modelId = ModelIdBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(modelId))
        {
            args.Cancel = true;
            var appVM = this.Get<AppViewModel>();
            appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.ModelNameOrIdCanNotBeEmpty), InfoType.Error));
            return;
        }

        var model = new ChatModel
        {
            DisplayName = modelName,
            Id = modelId,
            IsCustomModel = true,
        };

        Model = model;
    }
}
