// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using System.Security;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// 用于显示调试信息的对话框.
/// </summary>
public sealed partial class DebugDialog : ContentDialog
{
    private readonly VideoType _type;
    private readonly string _id;
    private readonly string _seasonTitle;

    private readonly ObservableCollection<FormatInformation> _formats;
    private readonly ObservableCollection<VideoIdentifier> _parts;
    private readonly ObservableCollection<EpisodeInformation> _episodes;

    private readonly AppViewModel _appViewModel;

    private string _title;
    private MediaInformation _mediaInformation;
    private SegmentInformation _video;
    private SegmentInformation _audio;

    private LiveMediaInformation _liveMediaInformation;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugDialog"/> class.
    /// </summary>
    public DebugDialog()
    {
        InitializeComponent();
        _appViewModel = AppViewModel.Instance;
        _formats = new ObservableCollection<FormatInformation>();
        _parts = new ObservableCollection<VideoIdentifier>();
        _episodes = new ObservableCollection<EpisodeInformation>();
        Loaded += OnLoadedAsync;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugDialog"/> class.
    /// </summary>
    public DebugDialog(VideoInformation video)
        : this()
    {
        _id = video.Identifier.Id;
        _type = VideoType.Video;
        _title = video.Identifier.Title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugDialog"/> class.
    /// </summary>
    public DebugDialog(LiveInformation live)
        : this()
    {
        _id = live.Identifier.Id;
        _type = VideoType.Live;
        _title = live.Identifier.Title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugDialog"/> class.
    /// </summary>
    public DebugDialog(SeasonInformation season)
        : this()
    {
        _id = season.Identifier.Id;
        _type = VideoType.Pgc;
        _title = season.Identifier.Title;
        _seasonTitle = _title;
    }

    private static string GetVideoPreferCodecId()
    {
        var preferCodec = SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodec.H264);
        var id = preferCodec switch
        {
            PreferCodec.H265 => "hev",
            PreferCodec.Av1 => "av01",
            _ => "avc",
        };

        return id;
    }

    private static string GetLivePreferCodecId()
    {
        var preferCodec = SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodec.H264);
        var id = preferCodec switch
        {
            PreferCodec.H265 => "hevc",
            PreferCodec.Av1 => "av1",
            _ => "avc",
        };

        return id;
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        ShowLoading();
        TraceLogger.LogLaunchDebug(_type);

        if (_type == VideoType.Video)
        {
            EpisodeComboBox.Visibility = Visibility.Collapsed;
            var view = await PlayerProvider.GetVideoDetailAsync(_id);
            foreach (var item in view.SubVideos)
            {
                _parts.Add(item);
            }

            PartComboBox.SelectedIndex = 0;
            await LoadPartAsync(_parts.First());
        }
        else if (_type == VideoType.Live)
        {
            DashButton.IsEnabled = false;
            AudioUrlBox.Visibility = Visibility.Collapsed;
            PartComboBox.Visibility = Visibility.Collapsed;
            EpisodeComboBox.Visibility = Visibility.Collapsed;
            var defaultQuality = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultLiveFormat, 400);
            _liveMediaInformation = await LiveProvider.GetLiveMediaInformationAsync(_id, defaultQuality, false);
            _formats.Clear();
            foreach (var item in _liveMediaInformation.Formats)
            {
                _formats.Add(item);
            }

            QualityComboBox.SelectedIndex = _formats.IndexOf(_formats.First(p => p.Quality == defaultQuality));
            LoadLiveUrlsByFormatIdAsync(defaultQuality);
        }
        else if (_type == VideoType.Pgc)
        {
            PartComboBox.Visibility = Visibility.Collapsed;
            var view = await PlayerProvider.GetPgcDetailAsync("0", _id);
            foreach (var item in view.Episodes)
            {
                _episodes.Add(item);
            }

            EpisodeComboBox.SelectedIndex = 0;
            await LoadEpisodeAsync(_episodes.First());
        }

        HideLoading();
    }

    private void LoadVideoUrlsByFormatId(int formatId)
    {
        var codecId = GetVideoPreferCodecId();
        VideoUrlBox.Text = string.Empty;
        AudioUrlBox.Text = string.Empty;

        _video = default;
        _audio = default;
        if (_mediaInformation.VideoSegments != null)
        {
            var filteredSegments = _mediaInformation.VideoSegments.Where(p => p.Id == formatId.ToString());
            if (!filteredSegments.Any())
            {
                var maxQuality = _mediaInformation.VideoSegments.Max(p => Convert.ToInt32(p.Id));
                _video = _mediaInformation.VideoSegments.First(p => p.Id == maxQuality.ToString());
            }
            else
            {
                _video = filteredSegments.FirstOrDefault(p => p.Codecs.Contains(codecId))
                    ?? filteredSegments.First();
            }
        }

        if (_mediaInformation.AudioSegments != null)
        {
            var audioQuality = SettingsToolkit.ReadLocalSetting(SettingNames.PreferAudioQuality, PreferAudio.Standard);
            if (audioQuality == PreferAudio.HighQuality)
            {
                var maxBandWidth = _mediaInformation.AudioSegments.Max(p => p.Bandwidth);
                _audio = _mediaInformation.AudioSegments.First(p => p.Bandwidth == maxBandWidth);
            }
            else if (audioQuality == PreferAudio.Near)
            {
                _audio = _mediaInformation.AudioSegments.OrderBy(p => Math.Abs(p.Bandwidth - _video.Bandwidth)).First();
            }
            else if (audioQuality == PreferAudio.Standard)
            {
                _audio = _mediaInformation.AudioSegments.Where(p => p.Bandwidth < 100000).FirstOrDefault()
                    ?? _mediaInformation.AudioSegments.OrderBy(p => p.Bandwidth).First();
            }
        }

        VideoUrlBox.Text = _video?.BaseUrl ?? string.Empty;
        AudioUrlBox.Text = _audio?.BaseUrl ?? string.Empty;
    }

    private async void LoadLiveUrlsByFormatIdAsync(int formatId)
    {
        VideoUrlBox.Text = string.Empty;

        ShowLoading();
        _liveMediaInformation = await LiveProvider.GetLiveMediaInformationAsync(_id, formatId, false);
        HideLoading();

        var codecId = GetLivePreferCodecId();
        var playLines = _liveMediaInformation.Lines.Where(p => p.Quality == formatId);
        if (playLines.Count() == 0)
        {
            playLines = _liveMediaInformation.Lines;
        }

        var url = playLines.SelectMany(p => p.Urls).FirstOrDefault(p => p.Protocol == "http_hls");
        url ??= playLines.SelectMany(p => p.Urls).FirstOrDefault(p => p.Protocol == "http_stream");
        url ??= playLines.SelectMany(p => p.Urls).FirstOrDefault();
        VideoUrlBox.Text = url.ToString();
        HideLoading();
    }

    private void ShowLoading()
    {
        LoadingRing.IsActive = true;
        DetailContainer.Visibility = Visibility.Collapsed;
    }

    private void HideLoading()
    {
        LoadingRing.IsActive = false;
        DetailContainer.Visibility = Visibility.Visible;
    }

    private void OnQualityComboBoxChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((_mediaInformation == null && _liveMediaInformation == null) || LoadingRing.IsActive || QualityComboBox.SelectedIndex == -1)
        {
            return;
        }

        var format = _formats[QualityComboBox.SelectedIndex];

        if (_type == VideoType.Video)
        {
            LoadVideoUrlsByFormatId(format.Quality);
        }
        else if (_type == VideoType.Live)
        {
            LoadLiveUrlsByFormatIdAsync(format.Quality);
        }
    }

    private async void OnCopyDashButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var mpdFilePath =
            _audio == null
                ? AppConstants.DashVideoWithoutAudioMPDFile
                : AppConstants.DashVideoMPDFile;
        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(mpdFilePath));
        var mpdStr = await FileIO.ReadTextAsync(file);

        var videoStr =
                $@"<Representation bandwidth=""{_video.Bandwidth}"" codecs=""{_video.Codecs}"" height=""{_video.Height}"" mimeType=""{_video.MimeType}"" id=""{_video.Id}"" width=""{_video.Width}"" startWithSap=""{_video.StartWithSap}"">
                               <BaseURL>{SecurityElement.Escape(_video.BaseUrl)}</BaseURL>
                               <SegmentBase indexRange=""{_video.IndexRange}"">
                                   <Initialization range=""{_video.Initialization}"" />
                               </SegmentBase>
                           </Representation>";

        var audioStr = string.Empty;

        if (_audio != null)
        {
            audioStr =
                    $@"<Representation bandwidth=""{_audio.Bandwidth}"" codecs=""{_audio.Codecs}"" mimeType=""{_audio.MimeType}"" id=""{_audio.Id}"">
                               <BaseURL>{SecurityElement.Escape(_audio.BaseUrl)}</BaseURL>
                               <SegmentBase indexRange=""{_audio.IndexRange}"">
                                   <Initialization range=""{_audio.Initialization}"" />
                               </SegmentBase>
                           </Representation>";
        }

        videoStr = videoStr.Trim();
        audioStr = audioStr.Trim();
        mpdStr = mpdStr.Replace("{video}", videoStr)
                       .Replace("{audio}", audioStr)
                       .Replace("{bufferTime}", $"PT20S");

        var dp = new DataPackage();
        dp.SetText(mpdStr);
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success);
    }

    private void LoadDashVideoMpv()
    {
        var httpParams = _type == VideoType.Live
            ? $"--cookies --no-ytdl --user-agent=\\\"Mozilla/5.0 BiliDroid/1.12.0 (bbcallen@gmail.com)\\\" --http-header-fields=\\\"Cookie: {AuthorizeProvider.GetCookieString()}\\\" --http-header-fields=\\\"Referer: https://live.bilibili.com\\\""
            : $"--cookies --user-agent=\\\"{ServiceConstants.DefaultUserAgentString}\\\" --http-header-fields=\\\"Cookie: {AuthorizeProvider.GetCookieString()}\\\" --http-header-fields=\\\"Referer: https://www.bilibili.com\\\"";
        var videoUrl = VideoUrlBox.Text;
        var audioUrl = AudioUrlBox.Text;
        var command = $"mpv {httpParams} --title=\\\"{_title}\\\" \\\"{videoUrl}\\\"";
        if (!string.IsNullOrEmpty(audioUrl))
        {
            command += $" --audio-file=\\\"{audioUrl}\\\"";
        }

        Task.Run(() =>
        {
            var startInfo = new ProcessStartInfo("powershell.exe", $"-Command \"{command}\"");
            var process = Process.Start(startInfo);
        });

        var q = (QualityComboBox.SelectedItem as FormatInformation)?.Quality ?? 0;
        if (q > 0)
        {
            TraceLogger.LogMpvLaunched(_type, q);
        }
    }

    private async Task LoadPartAsync(VideoIdentifier part)
    {
        _mediaInformation = await PlayerProvider.GetVideoMediaInformationAsync(_id, part.Id);
        _formats.Clear();
        foreach (var item in _mediaInformation.Formats)
        {
            _formats.Add(item);
        }

        QualityComboBox.SelectedIndex = 0;
        LoadVideoUrlsByFormatId(_formats.First().Quality);
    }

    private async Task LoadEpisodeAsync(EpisodeInformation info)
    {
        _title = _seasonTitle + " - " + info.Identifier.Title;
        _mediaInformation = await PlayerProvider.GetPgcMediaInformationAsync(info.PartId, info.Identifier.Id, info.SeasonType);
        _formats.Clear();
        foreach (var item in _mediaInformation.Formats)
        {
            _formats.Add(item);
        }

        QualityComboBox.SelectedIndex = 0;
        LoadVideoUrlsByFormatId(_formats.First().Quality);
    }

    private void OnOpenInMpvClickAsync(object sender, RoutedEventArgs e)
        => LoadDashVideoMpv();

    private async void OnPartSelectionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        if ((_mediaInformation == null && _liveMediaInformation == null) || LoadingRing.IsActive)
        {
            return;
        }

        var part = (VideoIdentifier)PartComboBox.SelectedItem;
        await LoadPartAsync(part);
    }

    private async void OnEpisodeSelectionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        if ((_mediaInformation == null && _liveMediaInformation == null) || LoadingRing.IsActive)
        {
            return;
        }

        var episode = (EpisodeInformation)EpisodeComboBox.SelectedItem;
        await LoadEpisodeAsync(episode);
    }
}
