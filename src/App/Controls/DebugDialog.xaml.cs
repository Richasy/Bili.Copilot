// Copyright (c) Bili Copilot. All rights reserved.

using System.Security;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Player;
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

    private readonly ObservableCollection<FormatInformation> _formats;

    private MediaInformation _mediaInformation;
    private SegmentInformation _video;
    private SegmentInformation _audio;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugDialog"/> class.
    /// </summary>
    public DebugDialog()
    {
        InitializeComponent();
        _formats = new ObservableCollection<FormatInformation>();
        Loaded += OnLoadedAsync;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugDialog"/> class.
    /// </summary>
    public DebugDialog(VideoInformation video)
        : this()
    {
        _id = string.IsNullOrEmpty(video.AlternateId) ? video.Identifier.Id : video.AlternateId;
        _type = VideoType.Video;
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

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        ShowLoading();

        if (_type == VideoType.Video)
        {
            var view = await PlayerProvider.GetVideoDetailAsync(_id);
            var currentPart = view.SubVideos.First();
            _mediaInformation = await PlayerProvider.GetVideoMediaInformationAsync(view.Information.Identifier.Id, currentPart.Id);
            _formats.Clear();
            foreach (var item in _mediaInformation.Formats)
            {
                _formats.Add(item);
            }

            QualityComboBox.SelectedIndex = 0;
            LoadVideoUrlsByFormatId(_formats.First().Quality);
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
        if (_mediaInformation == null)
        {
            return;
        }

        var format = _formats[QualityComboBox.SelectedIndex];
        LoadVideoUrlsByFormatId(format.Quality);
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
                       .Replace("{bufferTime}", $"PT4S");

        var dp = new DataPackage();
        dp.SetText(mpdStr);
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success);
    }
}
