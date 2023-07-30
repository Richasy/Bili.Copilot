// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Video;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// AI 总结对话框.
/// </summary>
public sealed partial class AISummarizeDialog : ContentDialog
{
    private readonly VideoIdentifier _video;
    private CancellationTokenSource _cts;

    /// <summary>
    /// Initializes a new instance of the <see cref="AISummarizeDialog"/> class.
    /// </summary>
    public AISummarizeDialog(VideoIdentifier video)
    {
        _video = video;
        InitializeComponent();
        Loaded += OnLoadedAsync;
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        var sb = new StringBuilder();
        _cts = new CancellationTokenSource();
        try
        {
            sb.AppendLine(ResourceToolkit.GetLocalizedString(StringNames.LaunchingFantasyCopilot));
            StepBox.Text = sb.ToString();
            await Launcher.LaunchUriAsync(new Uri("fancop://"));
            await Task.Delay(1000);
            sb.AppendLine(ResourceToolkit.GetLocalizedString(StringNames.LaunchedFantasyCopilot));
            StepBox.Text = sb.ToString();
            if (_cts.IsCancellationRequested)
            {
                return;
            }

            sb.AppendLine(ResourceToolkit.GetLocalizedString(StringNames.GettingVideoInformation));
            StepBox.Text = sb.ToString();
            var videoInfo = await PlayerProvider.GetVideoDetailAsync(_video.Id);

            if (_cts.IsCancellationRequested)
            {
                return;
            }

            sb.AppendLine(ResourceToolkit.GetLocalizedString(StringNames.GettingSubtitles));
            StepBox.Text = sb.ToString();
            var videoId = videoInfo.Information.Identifier.Id;
            var subVideo = videoInfo.SubVideos.FirstOrDefault();

            // 获取字幕索引.
            var subtitles = await PlayerProvider.GetSubtitleIndexAsync(videoId, subVideo.Id);

            if (!subtitles.Any())
            {
                sb.AppendLine(ResourceToolkit.GetLocalizedString(StringNames.NoSubtitle));
                StepBox.Text = sb.ToString();
                _cts = default;
                return;
            }

            if (_cts.IsCancellationRequested)
            {
                return;
            }

            // 获取字幕详情.
            var subtitle = await PlayerProvider.GetSubtitleDetailAsync(subtitles.First().Url);

            if (_cts.IsCancellationRequested)
            {
                return;
            }

            sb.AppendLine(ResourceToolkit.GetLocalizedString(StringNames.SummaryMessageSend));
            StepBox.Text = sb.ToString();

            // 发送总结.
            var videoContent = string.Join("\n", subtitle.Select(p => $"{p.Content}"));
            var language = subtitles.First().LanguageName;
            var prompt = $"""
                Instructions: Your output should use the following template:
                ### {ResourceToolkit.GetLocalizedString(StringNames.Summary)}
                ### {ResourceToolkit.GetLocalizedString(StringNames.Highlights)}
                - [Emoji] Bulletpoint

                Use three to ten brief bullet points to summarize the video transcript below, Choose an appropriate emoji for each bullet point. and summarize a short highlight:
                
                Title: {videoInfo.Information.Identifier.Title}
                Description:
                {videoInfo.Information.Description}
                Transcript:
                ${videoContent}

                ---
                Please write in {language} language.
                """;

            var message = string.Format(ResourceToolkit.GetLocalizedString(StringNames.SummarizeMessage), videoInfo.Information.Identifier.Title);
            var data = new
            {
                message,
                prompt,
                temperature = 0.5,
                tokens = Math.Min(videoContent.Length, 1000),
                chat = true,
            };
            var query = Uri.EscapeDataString(JsonSerializer.Serialize(data));

            // 如果文本过长，则保存成文件，然后发送文件链接.
            if (query.Length > 1000)
            {
                var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var tempFile = Path.Combine(desktopFolder, $"{videoInfo.Information.Identifier.Id}_ai_summarize.json");
                await File.WriteAllTextAsync(tempFile, query, Encoding.UTF8);
                query = Uri.EscapeDataString($"path:{tempFile}");
            }

            await Launcher.LaunchUriAsync(new Uri($"fancop://chat?json={query}"));
        }
        catch (Exception ex)
        {
            sb.AppendLine(ex.Message);
            StepBox.Text = sb.ToString();
        }
    }
}
