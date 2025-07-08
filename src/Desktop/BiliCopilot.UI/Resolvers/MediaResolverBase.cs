// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.MpvKernel.Player;
using Richasy.MpvKernel.Player.Models;

namespace BiliCopilot.UI.Resolvers;

public abstract class MediaResolverBase : IMpvMediaSourceResolver
{
    public IntPtr WindowHandle { get; set; }

    public abstract Task<MpvMediaSource> GetSourceAsync();
}
