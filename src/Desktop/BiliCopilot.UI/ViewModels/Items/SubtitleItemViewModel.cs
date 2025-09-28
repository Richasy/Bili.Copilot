// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Windows.Storage;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Subtitle;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

public sealed partial class SubtitleItemViewModel : ViewModelBase<SubtitleMeta>
{
    private readonly Func<SubtitleItemViewModel, Task> _activeFunc;
    private readonly Func<SubtitleItemViewModel, Task> _deactiveFunc;
    private List<SubtitleInformation>? _subtitles;
    internal string? _srtFilePath;

    public SubtitleItemViewModel(SubtitleMeta meta, Func<SubtitleItemViewModel, Task> activeFunc, Func<SubtitleItemViewModel, Task> deactiveFunc)
        : base(meta)
    {
        _activeFunc = activeFunc;
        _deactiveFunc = deactiveFunc;
        Name = meta.LanguageName;
        IsAI = meta.IsAI;
    }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial bool IsAI { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public override bool Equals(object? obj) => obj is SubtitleItemViewModel model && Data.Id == model.Data.Id;

    public override int GetHashCode() => HashCode.Combine(Data.Id);

    [RelayCommand]
    private async Task ActiveAsync()
    {
        if (IsSelected)
        {
            await _deactiveFunc(this);
            return;
        }

        await SelectAsync();
    }

    [RelayCommand]
    private async Task SelectAsync()
    {
        if (_subtitles is null)
        {
            try
            {
                var subtitles = await this.Get<ISubtitleService>().GetSubtitleDetailAsync(Data);
                _subtitles = [.. subtitles];
            }
            catch (Exception ex)
            {
                this.Get<ILogger<SubtitleItemViewModel>>().LogError(ex, "加载字幕失败");
                return;
            }
        }

        if (_subtitles?.Count > 0 && string.IsNullOrEmpty(_srtFilePath))
        {
            var srtContent = ConvertToSrt(_subtitles);
            var folderPath = Path.Combine(ApplicationData.GetDefault().TemporaryPath, "Subtitles");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            _srtFilePath = Path.Combine(folderPath, $"{Data.Id}.srt");
            await File.WriteAllTextAsync(_srtFilePath, srtContent, Encoding.UTF8);
        }

        await _activeFunc(this);
    }

    /// <summary>
    /// 将一组 SubtitleInformation 转换为标准 SRT 字幕内容（字符串）。
    /// </summary>
    /// <param name="subtitles">字幕信息集合</param>
    /// <returns>符合 .srt 格式的字符串</returns>
    private static string ConvertToSrt(IEnumerable<SubtitleInformation> subtitles)
    {
        var sb = new StringBuilder();
        var index = 1;
        foreach (var subtitle in subtitles)
        {
            sb.AppendLine(index.ToString());

            var start = SecondsToSrtTime(subtitle.StartPosition);
            var end = SecondsToSrtTime(subtitle.EndPosition);

            sb.AppendLine($"{start} --> {end}");
            sb.AppendLine(subtitle.Content ?? string.Empty);
            sb.AppendLine();
            index++;
        }

        return sb.ToString();
    }

    /// <summary>
    /// 将秒数转换为 SRT 格式时间字符串 "hh:mm:ss,fff"。
    /// </summary>
    private static string SecondsToSrtTime(double seconds)
    {
        var hours = (int)(seconds / 3600);
        var minutes = (int)(seconds % 3600 / 60);
        var secs = (int)(seconds % 60);
        var millis = (int)((seconds - (int)seconds) * 1000);

        // 注意格式用逗号和前导零
        return string.Format("{0:D2}:{1:D2}:{2:D2},{3:D3}", hours, minutes, secs, millis);
    }
}
