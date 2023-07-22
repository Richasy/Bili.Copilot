// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FFmpeg.AutoGen;

using Vortice.MediaFoundation;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;

/// <summary>
/// 视频设备流类.
/// </summary>
public class VideoDeviceStream : DeviceStreamBase
{
    /// <summary>
    /// 构造函数.
    /// </summary>
    /// <param name="deviceName">设备名称.</param>
    /// <param name="majorType">主类型.</param>
    /// <param name="subType">子类型.</param>
    /// <param name="frameSizeWidth">帧宽度.</param>
    /// <param name="frameSizeHeight">帧高度.</param>
    /// <param name="frameRate">帧率.</param>
    /// <param name="frameRateDenominator">帧率分母.</param>
    public VideoDeviceStream(string deviceName, Guid majorType, Guid subType, int frameSizeWidth, int frameSizeHeight, int frameRate, int frameRateDenominator)
        : base(deviceName)
    {
        MajorType = majorType == MediaTypeGuids.Video ? "Video" : "Unknown";
        SubType = GetPropertyName(subType);
        FrameSizeWidth = frameSizeWidth;
        FrameSizeHeight = frameSizeHeight;
        FrameRate = frameRate / frameRateDenominator;
        FFmpegFormat = GetFFmpegFormat(SubType);
        Url = $"fmt://dshow?video={DeviceFriendlyName}&video_size={FrameSizeWidth}x{FrameSizeHeight}&framerate={FrameRate}&{FFmpegFormat}";
    }

    /// <summary>
    /// 主类型.
    /// </summary>
    public string MajorType { get; }

    /// <summary>
    /// 子类型.
    /// </summary>
    public string SubType { get; }

    /// <summary>
    /// 帧宽度.
    /// </summary>
    public int FrameSizeWidth { get; }

    /// <summary>
    /// 帧高度.
    /// </summary>
    public int FrameSizeHeight { get; }

    /// <summary>
    /// 帧率.
    /// </summary>
    public int FrameRate { get; }

    /// <summary>
    /// FFmpeg 格式.
    /// </summary>
    private string FFmpegFormat { get; }

    /// <summary>
    /// 获取视频设备的视频格式列表.
    /// </summary>
    /// <param name="friendlyName">友好名称.</param>
    /// <param name="symbolicLink">符号链接.</param>
    /// <returns>视频格式列表.</returns>
    public static IList<VideoDeviceStream> GetVideoFormatsForVideoDevice(string friendlyName, string symbolicLink)
    {
        List<VideoDeviceStream> formatList = new();

        using (var mediaSource = GetMediaSourceFromVideoDevice(symbolicLink))
        {
            if (mediaSource == null)
            {
                return null;
            }

            using var sourcePresentationDescriptor = mediaSource.CreatePresentationDescriptor();
            var sourceStreamCount = sourcePresentationDescriptor.StreamDescriptorCount;

            for (var i = 0; i < sourceStreamCount; i++)
            {
                var guidMajorType = GetMajorMediaTypeFromPresentationDescriptor(sourcePresentationDescriptor, i);
                if (guidMajorType != MediaTypeGuids.Video)
                {
                    continue;
                }

                sourcePresentationDescriptor.GetStreamDescriptorByIndex(i, out var streamIsSelected, out var videoStreamDescriptor);

                using (videoStreamDescriptor)
                {
                    if (streamIsSelected == false)
                    {
                        continue;
                    }

                    using var typeHandler = videoStreamDescriptor.MediaTypeHandler;
                    var mediaTypeCount = typeHandler.MediaTypeCount;

                    for (var mediaTypeId = 0; mediaTypeId < mediaTypeCount; mediaTypeId++)
                    {
                        using (var workingMediaType = typeHandler.GetMediaTypeByIndex(mediaTypeId))
                        {
                            var videoFormat = GetVideoFormatFromMediaType(friendlyName, workingMediaType);

                            if (videoFormat.SubType != "NV12") // NV12 is not playable TODO check support for video formats
                            {
                                formatList.Add(videoFormat);
                            }
                        }
                    }
                }
            }
        }

        return formatList.OrderBy(format => format.SubType).ThenBy(format => format.FrameSizeHeight).ThenBy(format => format.FrameRate).ToList();
    }

    /// <summary>
    /// 返回对象的字符串表示形式.
    /// </summary>
    /// <returns>对象的字符串表示形式.</returns>
    public override string ToString() => $"{SubType}, {FrameSizeWidth}x{FrameSizeHeight}, {FrameRate}FPS";

    /// <summary>
    /// 从视频设备获取媒体源.
    /// </summary>
    /// <param name="symbolicLink">符号链接.</param>
    /// <returns>媒体源.</returns>
    private static IMFMediaSource GetMediaSourceFromVideoDevice(string symbolicLink)
    {
        using var attributeContainer = MediaFactory.MFCreateAttributes(2);
        attributeContainer.Set(CaptureDeviceAttributeKeys.SourceType, CaptureDeviceAttributeKeys.SourceTypeVidcap);
        attributeContainer.Set(CaptureDeviceAttributeKeys.SourceTypeVidcapSymbolicLink, symbolicLink);

        IMFMediaSource ret = null;
        try
        {
            ret = MediaFactory.MFCreateDeviceSource(attributeContainer);
        }
        catch (Exception)
        {
        }

        return ret;
    }

    /// <summary>
    /// 从演示描述符和流索引获取主类型.
    /// </summary>
    /// <param name="presentationDescriptor">演示描述符.</param>
    /// <param name="streamIndex">流索引.</param>
    /// <returns>主类型.</returns>
    private static Guid GetMajorMediaTypeFromPresentationDescriptor(IMFPresentationDescriptor presentationDescriptor, int streamIndex)
    {
        presentationDescriptor.GetStreamDescriptorByIndex(streamIndex, out _, out var streamDescriptor);

        using (streamDescriptor)
        {
            return GetMajorMediaTypeFromStreamDescriptor(streamDescriptor);
        }
    }

    /// <summary>
    /// 从流描述符获取主类型.
    /// </summary>
    /// <param name="streamDescriptor">流描述符.</param>
    /// <returns>主类型.</returns>
    private static Guid GetMajorMediaTypeFromStreamDescriptor(IMFStreamDescriptor streamDescriptor)
    {
        using var pHandler = streamDescriptor.MediaTypeHandler;
        var guidMajorType = pHandler.MajorType;

        return guidMajorType;
    }

    /// <summary>
    /// 从媒体类型获取视频格式.
    /// </summary>
    /// <param name="videoDeviceName">视频设备名称.</param>
    /// <param name="mediaType">媒体类型.</param>
    /// <returns>视频格式.</returns>
    private static VideoDeviceStream GetVideoFormatFromMediaType(string videoDeviceName, IMFMediaType mediaType)
    {
        // MF_MT_MAJOR_TYPE
        // Major type GUID, we return this as human readable text
        var majorType = mediaType.MajorType;

        // MF_MT_SUBTYPE
        // Subtype GUID which describes the basic media type, we return this as human readable text
        var subType = mediaType.Get<Guid>(MediaTypeAttributeKeys.Subtype);

        // MF_MT_FRAME_SIZE
        // the Width and height of a video frame, in pixels
        MediaFactory.MFGetAttributeSize(mediaType, MediaTypeAttributeKeys.FrameSize, out var frameSizeWidth, out var frameSizeHeight);

        // MF_MT_FRAME_RATE
        // The frame rate is expressed as a ratio.The upper 32 bits of the attribute value contain the numerator and the lower 32 bits contain the denominator.
        // For example, if the frame rate is 30 frames per second(fps), the ratio is 30 / 1.If the frame rate is 29.97 fps, the ratio is 30,000 / 1001.
        // we report this back to the user as a decimal
        MediaFactory.MFGetAttributeRatio(mediaType, MediaTypeAttributeKeys.FrameRate, out var frameRate, out var frameRateDenominator);

        VideoDeviceStream videoFormat = new(videoDeviceName, majorType, subType, (int)frameSizeWidth,
            (int)frameSizeHeight,
            (int)frameRate,
            (int)frameRateDenominator);

        return videoFormat;
    }

    /// <summary>
    /// 获取属性名称.
    /// </summary>
    /// <param name="guid">GUID.</param>
    /// <returns>属性名称.</returns>
    private static string GetPropertyName(Guid guid)
    {
        var type = typeof(VideoFormatGuids);
        foreach (var property in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (property.FieldType == typeof(Guid))
            {
                var temp = property.GetValue(null);
                if (temp is Guid value)
                {
                    if (value == guid)
                    {
                        return property.Name.ToUpper();
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 获取 FFmpeg 格式.
    /// </summary>
    /// <param name="subType">子类型.</param>
    /// <returns>FFmpeg 格式.</returns>
    private static unsafe string GetFFmpegFormat(string subType)
    {
        switch (subType)
        {
            case "MJPG":
                var descriptorPtr = ffmpeg.avcodec_descriptor_get(AVCodecID.AV_CODEC_ID_MJPEG);
                return $"vcodec={Utils.BytePtrToStringUtf8(descriptorPtr->name)}";
            case "YUY2":
                return $"pixel_format={ffmpeg.av_get_pix_fmt_name(AVPixelFormat.AV_PIX_FMT_YUYV422)}";
            case "NV12":
                return $"pixel_format={ffmpeg.av_get_pix_fmt_name(AVPixelFormat.AV_PIX_FMT_NV12)}";
            default:
                return string.Empty;
        }
    }
}
