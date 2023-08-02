// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// AI 功能视图模型.
/// </summary>
public sealed partial class AIFeatureViewModel
{
    private static string GetVideoSummaryPrompt(string content, string title, string description, string language)
    {
        var prompt = $"""
                Instructions: Your output should use the following template:
                ### {ResourceToolkit.GetLocalizedString(StringNames.Summary)}
                ### {ResourceToolkit.GetLocalizedString(StringNames.Highlights)}
                - [Emoji] Bulletpoint

                Use three to ten brief bullet points to summarize the video transcript below, Choose an appropriate emoji for each bullet point. and summarize a short highlight:
                
                Title: {title}
                Description:
                {description}
                Transcript:
                ${content}

                ---
                Please write in {language} language.
                """;

        return prompt;
    }

    private static string GetArticleSummaryPrompt(string content, string title)
    {
        var language = ApplicationLanguages.Languages.FirstOrDefault();
        var prompt = $"""
                Instructions: Your output should use the following template:
                ### {ResourceToolkit.GetLocalizedString(StringNames.Summary)}
                ### {ResourceToolkit.GetLocalizedString(StringNames.Highlights)}
                - [Emoji] Bulletpoint

                Use more than three brief bullet points to summarize the article below, Choose an appropriate emoji for each bullet point. and summarize a short highlight:
                
                Title: {title}
                Content:
                {content}

                ---
                Please write in {language} language.
                """;
        return prompt;
    }

    private async Task SendMessageAsync(string message, string prompt, double temperature = 0.5, int tokens = 1000, bool chat = true)
    {
        var data = new
        {
            message,
            prompt,
            temperature,
            tokens,
            chat,
        };

        if (prompt.Length > 4000)
        {
            // 需要总结的内容过长，很可能超过模型的令牌窗口，需要提醒.
            var tooLongTip = new AppTipNotification(
                string.Format(ResourceToolkit.GetLocalizedString(StringNames.AIMessageTooLong), prompt.Length),
                InfoType.Warning);
            Tips.Add(tooLongTip);
        }

        if (_connection == null)
        {
            var query = Uri.EscapeDataString(JsonSerializer.Serialize(data));

            // 如果文本过长，则保存成文件，然后发送文件链接.
            if (query.Length > 1000)
            {
                var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var tempFile = Path.Combine(desktopFolder, $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}_ai_summarize.json");
                await File.WriteAllTextAsync(tempFile, query, Encoding.UTF8);
                query = Uri.EscapeDataString($"path:{tempFile}");
            }

            await Launcher.LaunchUriAsync(new Uri($"fancop://chat?json={query}"));
            var tip = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.AIMessageSend), InfoType.Success);
            Tips.Add(tip);
        }
        else
        {
            var msg = new ValueSet
            {
                ["Command"] = "QuickChat",
                ["Request"] = JsonSerializer.Serialize(data),
            };

            IsWaiting = true;
            var response = await _connection.SendMessageAsync(msg);
            if (response.Status == AppServiceResponseStatus.Success)
            {
                if (response.Message.ContainsKey("Error"))
                {
                    var tip = new AppTipNotification(response.Message["Error"] as string, InfoType.Error);
                    Tips.Add(tip);
                }
                else
                {
                    var content = response.Message["Response"] as string;
                    ResponseText = content;
                }
            }
            else
            {
                var tip = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.AppServiceMessageSendFailed) + $" {response.Status}", InfoType.Error);
                Tips.Add(tip);
            }

            IsWaiting = false;
            _connection.Dispose();
        }
    }
}
