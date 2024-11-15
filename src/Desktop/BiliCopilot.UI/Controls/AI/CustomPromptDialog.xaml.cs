// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.AI;

/// <summary>
/// 自定义提示词对话框.
/// </summary>
public sealed partial class CustomPromptDialog : ContentDialog
{
    private readonly string _promptId;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomPromptDialog"/> class.
    /// </summary>
    public CustomPromptDialog(string promptId, string title, bool isTitleEnabled = false)
    {
        InitializeComponent();
        _promptId = promptId;
        TitleBox.Text = title;
        TitleBox.IsEnabled = isTitleEnabled;
        Loaded += OnLoadedAsync;
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        var fileName = $"{_promptId}.txt";
        PromptBox.Text = await FileToolkit.ReadLocalDataAsync<string>(fileName, default, string.Empty, "Prompt");
    }

    private async void OnPrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        args.Cancel = true;
        if (string.IsNullOrEmpty(PromptBox.Text))
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.PromptCanNotBeEmpty), InfoType.Error));
            return;
        }

        var fileName = $"{_promptId}.txt";
        await FileToolkit.WriteLocalDataAsync(fileName, PromptBox.Text, default, "Prompt");
        Hide();
    }
}
