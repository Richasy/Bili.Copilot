// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.MpvKernel.Player;
using Richasy.MpvKernel.Player.Models;

namespace BiliCopilot.UI.Resolvers;

internal abstract class MediaSourceResolverBase : IMpvMediaSourceResolver
{
    public IntPtr WindowHandle { get; set; }

    public abstract Task<MpvMediaSource> GetSourceAsync();

    public abstract IMpvMediaSourceResolver Clone();
}
