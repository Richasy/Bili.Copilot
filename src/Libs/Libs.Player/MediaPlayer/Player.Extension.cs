// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Drawing.Imaging;
using System.IO;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;
using Windows.Foundation;
using static Bili.Copilot.Libs.Player.Misc.Logger;
using static Bili.Copilot.Libs.Player.Utils;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 表示播放器的类.
/// </summary>
public unsafe partial class Player
{
    /// <summary>
    /// 向后快进.
    /// </summary>
    public void SeekBackward() => SeekBackward_(Config.Player.SeekOffset);

    /// <summary>
    /// 向后快进2.
    /// </summary>
    public void SeekBackward2() => SeekBackward_(Config.Player.SeekOffset2);

    /// <summary>
    /// 向后快进3.
    /// </summary>
    public void SeekBackward3() => SeekBackward_(Config.Player.SeekOffset3);

    /// <summary>
    /// 执行向后快进的内部方法.
    /// </summary>
    /// <param name="offset">偏移量.</param>
    public void SeekBackward_(long offset)
    {
        if (!CanPlay)
        {
            return;
        }

        if (Config.Player.SeekAccurate)
        {
            SeekAccurate(Math.Max((int)((CurTime - offset) / 10000), 0));
        }
        else
        {
            Seek(Math.Max((int)((CurTime - offset) / 10000), 0), false);
        }
    }

    /// <summary>
    /// 向前快进.
    /// </summary>
    public void SeekForward() => SeekForwardInternal(Config.Player.SeekOffset);

    /// <summary>
    /// 向前快进2.
    /// </summary>
    public void SeekForward2() => SeekForwardInternal(Config.Player.SeekOffset2);

    /// <summary>
    /// 向前快进3.
    /// </summary>
    public void SeekForward3() => SeekForwardInternal(Config.Player.SeekOffset3);

    /// <summary>
    /// 执行向前快进的内部方法.
    /// </summary>
    /// <param name="offset">偏移量.</param>
    public void SeekForwardInternal(long offset)
    {
        if (!CanPlay)
        {
            return;
        }

        var seekTs = CurTime + offset;

        if (seekTs <= Duration || IsLive)
        {
            if (Config.Player.SeekAccurate)
            {
                SeekAccurate((int)(seekTs / 10000));
            }
            else
            {
                Seek((int)(seekTs / 10000), true);
            }
        }
    }

    /// <summary>
    /// 跳转到指定章节.
    /// </summary>
    /// <param name="chapter">章节.</param>
    public void SeekToChapter(Demuxer.Chapter chapter)
        => Seek((int)(chapter.StartTime / 10000.0), true);

    /// <summary>
    /// 显示指定帧.
    /// </summary>
    /// <param name="frameIndex">帧索引.</param>
    public void ShowFrame(int frameIndex)
    {
        if (!Video.IsOpened || !CanPlay || VideoDemuxer.IsHLSLive)
        {
            return;
        }

        lock (_lockActions)
        {
            Pause();
            SubtitleFrame = null;
            Subtitles.SubtitleText = string.Empty;
            if (Subtitles.SubtitleText != string.Empty)
            {
                UI(() => Subtitles.SubtitleText = Subtitles.SubtitleText);
            }

            Decoder.Flush();
            Decoder.RequiresResync = true;

            _videoFrame = VideoDecoder.GetFrame(frameIndex);
            if (_videoFrame == null)
            {
                return;
            }

            if (CanDebug)
            {
                Log.Debug($"SFI: {VideoDecoder.GetFrameNumber(_videoFrame.Timestamp)}");
            }

            _curTime = _videoFrame.Timestamp;
            Renderer.Present(_videoFrame);
            _reversePlaybackResync = true;
            _videoFrame = null;

            UI(() => UpdateCurTime());
        }
    }

    /// <summary>
    /// 显示下一帧.
    /// </summary>
    public void ShowFrameNext()
    {
        if (!Video.IsOpened || !CanPlay || VideoDemuxer.IsHLSLive)
        {
            return;
        }

        lock (_lockActions)
        {
            Pause();
            ReversePlayback = false;
            if (!string.IsNullOrEmpty(Subtitles.SubtitleText))
            {
                SubtitleFrame = null;
                Subtitles.SubtitleText = Subtitles.SubtitleText;
            }

            if (VideoDecoder.Frames.IsEmpty)
            {
                _videoFrame = VideoDecoder.GetFrameNext();
            }
            else
            {
                VideoDecoder.Frames.TryDequeue(out _videoFrame);
            }

            if (_videoFrame == null)
            {
                return;
            }

            if (CanDebug)
            {
                Log.Debug($"SFN: {VideoDecoder.GetFrameNumber(_videoFrame.Timestamp)}");
            }

            _curTime = _curTime = _videoFrame.Timestamp;
            Renderer.Present(_videoFrame);
            _reversePlaybackResync = true;
            _videoFrame = null;

            UI(() => UpdateCurTime());
        }
    }

    /// <summary>
    /// 显示上一帧.
    /// </summary>
    public void ShowFramePrev()
    {
        if (!Video.IsOpened || !CanPlay || VideoDemuxer.IsHLSLive)
        {
            return;
        }

        lock (_lockActions)
        {
            Pause();

            if (!ReversePlayback)
            {
                SetProperty(ref _reversePlayback, true);
                Speed = 1;
                if (!string.IsNullOrEmpty(Subtitles.SubtitleText))
                {
                    SubtitleFrame = null;
                    Subtitles.SubtitleText = Subtitles.SubtitleText;
                }

                Decoder.StopThreads();
                Decoder.Flush();
            }

            if (VideoDecoder.Frames.Count == 0)
            {
                // Temp fix for previous timestamps until we seperate GetFrame for Extractor and the Player
                _reversePlaybackResync = true;
                var askedFrame = VideoDecoder.GetFrameNumber(CurTime) - 1;

                _videoFrame = VideoDecoder.GetFrame(askedFrame);
                if (_videoFrame == null)
                {
                    return;
                }

                var recVideoFrame = VideoDecoder.GetFrameNumber(_videoFrame.Timestamp);
                if (askedFrame != recVideoFrame)
                {
                    MediaFramework.MediaDecoder.VideoDecoder.DisposeFrame(_videoFrame);
                    _videoFrame = null;
                    _videoFrame = askedFrame > recVideoFrame
                        ? VideoDecoder.GetFrame(VideoDecoder.GetFrameNumber(CurTime))
                        : VideoDecoder.GetFrame(VideoDecoder.GetFrameNumber(CurTime) - 2);
                }
            }
            else
            {
                VideoDecoder.Frames.TryDequeue(out _videoFrame);
            }

            if (_videoFrame == null)
            {
                return;
            }

            if (CanDebug)
            {
                Log.Debug($"SFB: {VideoDecoder.GetFrameNumber(_videoFrame.Timestamp)}");
            }

            _curTime = _videoFrame.Timestamp;
            Renderer.Present(_videoFrame);
            _videoFrame = null;
            UI(() => UpdateCurTime()); // For some strange reason this will not be updated on KeyDown (only on KeyUp) which doesn't happen on ShowFrameNext (GPU overload? / Thread.Sleep underlying in UI thread?)
        }
    }

    /// <summary>
    /// 加速播放速度.
    /// </summary>
    public void SpeedUp() => Speed += Config.Player.SpeedOffset;

    /// <summary>
    /// 加速播放速度2.
    /// </summary>
    public void SpeedUp2() => Speed += Config.Player.SpeedOffset2;

    /// <summary>
    /// 减速播放速度.
    /// </summary>
    public void SpeedDown() => Speed -= Config.Player.SpeedOffset;

    /// <summary>
    /// 减速播放速度2.
    /// </summary>
    public void SpeedDown2() => Speed -= Config.Player.SpeedOffset2;

    /// <summary>
    /// 向右旋转.
    /// </summary>
    public void RotateRight() => Renderer.Rotation = (Renderer.Rotation + 90) % 360;

    /// <summary>
    /// 向左旋转.
    /// </summary>
    public void RotateLeft() => Renderer.Rotation = Renderer.Rotation < 90 ? 360 + Renderer.Rotation - 90 : Renderer.Rotation - 90;

    /// <summary>
    /// 进入全屏模式.
    /// </summary>
    public void FullScreen() => TransportControls?.SetFullScreenState(true);

    /// <summary>
    /// 进入正常屏幕模式.
    /// </summary>
    public void NormalScreen() => TransportControls?.SetFullScreenState(false);

    /// <summary>
    /// 切换全屏模式.
    /// </summary>
    public void ToggleFullScreen()
    {
        if (TransportControls == null)
        {
            return;
        }

        TransportControls.SetFullScreenState(!TransportControls.IsFullScreen());
    }

    /// <summary>
    /// 开始录制（使用 Config.Player.FolderRecordings 和默认文件名 title_curTime）.
    /// </summary>
    public void StartRecording()
    {
        if (!CanPlay)
        {
            return;
        }

        try
        {
            if (!Directory.Exists(Config.Player.FolderRecordings))
            {
                Directory.CreateDirectory(Config.Player.FolderRecordings);
            }

            var filename = GetValidFileName(string.IsNullOrEmpty(Playlist.Selected.Title) ? "Record" : Playlist.Selected.Title) + $"_{new TimeSpan(CurTime):hhmmss}." + Decoder.Extension;
            filename = FindNextAvailableFile(Path.Combine(Config.Player.FolderRecordings, filename));
            StartRecording(ref filename, false);
        }
        catch
        {
        }
    }

    /// <summary>
    /// 开始录制.
    /// </summary>
    /// <param name="filename">新录制文件的路径.</param>
    /// <param name="useRecommendedExtension">是否使用推荐的扩展名.</param>
    public void StartRecording(ref string filename, bool useRecommendedExtension = true)
    {
        if (!CanPlay)
        {
            return;
        }

        Decoder.StartRecording(ref filename, useRecommendedExtension);
        IsRecording = Decoder.IsRecording;
    }

    /// <summary>
    /// 停止录制.
    /// </summary>
    public void StopRecording()
    {
        Decoder.StopRecording();
        IsRecording = Decoder.IsRecording;
    }

    /// <summary>
    /// 切换录制状态.
    /// </summary>
    public void ToggleRecording()
    {
        if (!CanPlay)
        {
            return;
        }

        if (IsRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    /// <summary>
    /// 保存当前视频帧为文件（根据文件扩展名 .bmp、.png、.jpg 进行编码）.
    /// </summary>
    /// <param name="filename">指定的文件名（null：使用 Config.Player.FolderSnapshots 和默认文件名 title_frameNumber.ext）.</param>
    /// <param name="width">指定的宽度（-1：保持高度比例）.</param>
    /// <param name="height">指定的高度（-1：保持宽度比例）.</param>
    /// <param name="frame">指定的帧（null：使用当前/最后一帧）.</param>
    public void TakeSnapshotToFile(string filename = null, int width = -1, int height = -1, VideoFrame frame = null)
    {
        if (!CanPlay)
        {
            return;
        }

        if (filename == null)
        {
            try
            {
                if (!Directory.Exists(Config.Player.FolderSnapshots))
                {
                    Directory.CreateDirectory(Config.Player.FolderSnapshots);
                }

                // TBR: if frame is specified we don't know the frame's number
                filename = GetValidFileName(string.IsNullOrEmpty(Playlist.Selected.Title) ? "Snapshot" : Playlist.Selected.Title) + $"_{(frame == null ? VideoDecoder.GetFrameNumber(CurTime).ToString() : "X")}.{Config.Player.SnapshotFormat}";
                filename = FindNextAvailableFile(Path.Combine(Config.Player.FolderSnapshots, filename));
            }
            catch
            {
                return;
            }
        }

        var ext = GetUrlExtension(filename);
        var imageFormat = ext switch
        {
            "bmp" => ImageFormat.Bmp,
            "png" => ImageFormat.Png,
            "jpg" or "jpeg" => ImageFormat.Jpeg,
            _ => throw new Exception($"Invalid snapshot extention '{ext}' (valid .bmp, .png, .jpeg, .jpg"),
        };

        if (Renderer == null)
        {
            return;
        }

        var snapshotBitmap = Renderer.GetBitmap(width, height, frame);
        if (snapshotBitmap == null)
        {
            return;
        }

        Exception e = null;
        try
        {
            snapshotBitmap.Save(filename, imageFormat);
        }
        catch (Exception e2)
        {
            e = e2;
        }

        snapshotBitmap.Dispose();

        if (e != null)
        {
            throw e;
        }
    }

    /// <summary>
    /// 返回当前或指定视频帧的位图.
    /// </summary>
    /// <param name="width">指定的宽度（-1：保持高度比例）.</param>
    /// <param name="height">指定的高度（-1：保持宽度比例）.</param>
    /// <param name="frame">指定的帧（null：使用当前/最后一帧）.</param>
    /// <returns>位图.</returns>
    public System.Drawing.Bitmap TakeSnapshotToBitmap(int width = -1, int height = -1, VideoFrame frame = null)
        => Renderer?.GetBitmap(width, height, frame);

    /// <summary>
    /// 放大.
    /// </summary>
    public void ZoomIn() => Zoom += Config.Player.ZoomOffset;

    /// <summary>
    /// 缩小.
    /// </summary>
    public void ZoomOut()
    {
        if (Zoom - Config.Player.ZoomOffset < 1)
        {
            return;
        }

        Zoom -= Config.Player.ZoomOffset;
    }

    /// <summary>
    /// 以指定中心点进行放大.
    /// </summary>
    /// <param name="p">中心点.</param>
    public void ZoomIn(Point p)
    {
        Renderer.ZoomWithCenterPoint(p, Renderer.Zoom + (Config.Player.ZoomOffset / 100.0));
        OnPropertyChanged(nameof(Zoom));
    }

    /// <summary>
    /// 以指定中心点进行缩小.
    /// </summary>
    /// <param name="p">中心点.</param>
    public void ZoomOut(Point p)
    {
        var zoom = Renderer.Zoom - (Config.Player.ZoomOffset / 100.0);
        if (zoom < 0.001)
        {
            return;
        }

        Renderer.ZoomWithCenterPoint(p, zoom);
        OnPropertyChanged(nameof(Zoom));
    }

    /// <summary>
    /// 设置缩放比例.
    /// </summary>
    /// <param name="zoom">缩放比例.</param>
    public void SetZoom(double zoom) => Renderer.SetZoom(zoom);

    /// <summary>
    /// 设置缩放中心点.
    /// </summary>
    /// <param name="p">缩放中心点.</param>
    public void SetZoomCenter(Point p) => Renderer.SetZoomCenter(p);

    /// <summary>
    /// 设置缩放比例和缩放中心点.
    /// </summary>
    /// <param name="zoom">缩放比例.</param>
    /// <param name="p">缩放中心点.</param>
    public void SetZoomAndCenter(double zoom, Point p) => Renderer.SetZoomAndCenter(zoom, p);

    /// <summary>
    /// 重置所有设置.
    /// </summary>
    public void ResetAll()
    {
        ReversePlayback = false;
        Speed = 1;

        var npx = Renderer.PanXOffset != 0;
        var npy = Renderer.PanXOffset != 0;
        var npr = Renderer.Rotation != 0;
        var npz = Renderer.Zoom != 1;
        Renderer.SetPanAll(0, 0, 0, 1, MediaFramework.MediaRenderer.Renderer.ZoomCenterPoint, true); // Pan X/Y, Rotation, Zoom, Zoomcenter, Refresh

        UI(() =>
        {
            if (npx)
            {
                OnPropertyChanged(nameof(PanXOffset));
            }

            if (npy)
            {
                OnPropertyChanged(nameof(PanYOffset));
            }

            if (npr)
            {
                OnPropertyChanged(nameof(Rotation));
            }

            if (npz)
            {
                OnPropertyChanged(nameof(Zoom));
            }
        });
    }
}
