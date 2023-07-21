using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FFmpeg.AutoGen;

using Vortice.MediaFoundation;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;

public class VideoDeviceStream : DeviceStreamBase
{
    public string   MajorType           { get; }
    public string   SubType             { get; }
    public int      FrameSizeWidth      { get; }
    public int      FrameSizeHeight     { get; }
    public int      FrameRate           { get; }
    private string  FFmpegFormat        { get; }

    public VideoDeviceStream(string deviceName, Guid majorType, Guid subType, int frameSizeWidth, int frameSizeHeight, int frameRate, int frameRateDenominator) : base(deviceName)
    {
        MajorType           = MediaTypeGuids.Video == majorType ? "Video" : "Unknown";
        SubType             = GetPropertyName(subType);
        FrameSizeWidth      = frameSizeWidth;
        FrameSizeHeight     = frameSizeHeight;
        FrameRate           = frameRate / frameRateDenominator;
        FFmpegFormat        = GetFFmpegFormat(SubType);
        Url                 = $"fmt://dshow?video={DeviceFriendlyName}&video_size={FrameSizeWidth}x{FrameSizeHeight}&framerate={FrameRate}&{FFmpegFormat}";
    }

    private static string GetPropertyName(Guid guid)
    {
        var type = typeof(VideoFormatGuids);
        foreach (var property in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            if (property.FieldType == typeof(Guid))
            {
                object temp = property.GetValue(null);
                if (temp is Guid value)
                    if (value == guid)
                        return property.Name.ToUpper();
            }

        return null;
    }
    private static unsafe string GetFFmpegFormat(string subType)
    {
        switch (subType)
        {
            case "MJPG":
                var descriptorPtr = ffmpeg.avcodec_descriptor_get(AVCodecID.AV_CODEC_ID_MJPEG);
                return $"vcodec={Utils.BytePtrToStringUTF8(descriptorPtr->name)}";
            case "YUY2":
                return $"pixel_format={ffmpeg.av_get_pix_fmt_name(AVPixelFormat.AV_PIX_FMT_YUYV422)}";
            case "NV12":
                return $"pixel_format={ffmpeg.av_get_pix_fmt_name(AVPixelFormat.AV_PIX_FMT_NV12)}";
            default:
                return "";
        }
    }
    public override string ToString() => $"{SubType}, {FrameSizeWidth}x{FrameSizeHeight}, {FrameRate}FPS";

    #region VideoFormatsViaMediaSource
    public static IList<VideoDeviceStream> GetVideoFormatsForVideoDevice(string friendlyName, string symbolicLink)
    {
        List<VideoDeviceStream> formatList = new();

        using (var mediaSource = GetMediaSourceFromVideoDevice(symbolicLink))
        {
            if (mediaSource == null)
                return null;

            using var sourcePresentationDescriptor = mediaSource.CreatePresentationDescriptor();
            int sourceStreamCount = sourcePresentationDescriptor.StreamDescriptorCount;

            for (int i = 0; i < sourceStreamCount; i++)
            {
                var guidMajorType = GetMajorMediaTypeFromPresentationDescriptor(sourcePresentationDescriptor, i);
                if (guidMajorType != MediaTypeGuids.Video)
                    continue;

                sourcePresentationDescriptor.GetStreamDescriptorByIndex(i, out var streamIsSelected, out var videoStreamDescriptor);

                using (videoStreamDescriptor)
                {
                    if (streamIsSelected == false)
                        continue;

                    using var typeHandler = videoStreamDescriptor.MediaTypeHandler;
                    int mediaTypeCount = typeHandler.MediaTypeCount;

                    for (int mediaTypeId = 0; mediaTypeId < mediaTypeCount; mediaTypeId++)
                        using (var workingMediaType = typeHandler.GetMediaTypeByIndex(mediaTypeId))
                        {
                            var videoFormat = GetVideoFormatFromMediaType(friendlyName, workingMediaType);

                            if (videoFormat.SubType != "NV12") // NV12 is not playable TODO check support for video formats
                                formatList.Add(videoFormat);
                        }
                }
            }
        }

        return formatList.OrderBy(format => format.SubType).ThenBy(format => format.FrameSizeHeight).ThenBy(format => format.FrameRate).ToList();
    }

    private static IMFMediaSource GetMediaSourceFromVideoDevice(string symbolicLink)
    {
        using var attributeContainer = MediaFactory.MFCreateAttributes(2);
        attributeContainer.Set(CaptureDeviceAttributeKeys.SourceType, CaptureDeviceAttributeKeys.SourceTypeVidcap);
        attributeContainer.Set(CaptureDeviceAttributeKeys.SourceTypeVidcapSymbolicLink, symbolicLink);

        IMFMediaSource ret = null;
        try
        { ret = MediaFactory.MFCreateDeviceSource(attributeContainer); }
        catch (Exception) { }

        return ret;
    }

    private static Guid GetMajorMediaTypeFromPresentationDescriptor(IMFPresentationDescriptor presentationDescriptor, int streamIndex)
    {
        presentationDescriptor.GetStreamDescriptorByIndex(streamIndex, out _, out var streamDescriptor);

        using (streamDescriptor)
            return GetMajorMediaTypeFromStreamDescriptor(streamDescriptor);
    }

    private static Guid GetMajorMediaTypeFromStreamDescriptor(IMFStreamDescriptor streamDescriptor)
    {
        using var pHandler = streamDescriptor.MediaTypeHandler;
        var guidMajorType = pHandler.MajorType;

        return guidMajorType;
    }

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
        MediaFactory.MFGetAttributeSize(mediaType, MediaTypeAttributeKeys.FrameSize, out uint frameSizeWidth, out uint frameSizeHeight);

        // MF_MT_FRAME_RATE
        // The frame rate is expressed as a ratio.The upper 32 bits of the attribute value contain the numerator and the lower 32 bits contain the denominator. 
        // For example, if the frame rate is 30 frames per second(fps), the ratio is 30 / 1.If the frame rate is 29.97 fps, the ratio is 30,000 / 1001.
        // we report this back to the user as a decimal
        MediaFactory.MFGetAttributeRatio(mediaType, MediaTypeAttributeKeys.FrameRate, out uint frameRate, out uint frameRateDenominator);

        VideoDeviceStream videoFormat = new(videoDeviceName, majorType, subType, (int)frameSizeWidth,
            (int)frameSizeHeight,
            (int)frameRate,
            (int)frameRateDenominator);

        return videoFormat;
    }
    #endregion
}
