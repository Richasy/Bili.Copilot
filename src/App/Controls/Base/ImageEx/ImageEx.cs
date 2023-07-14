// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// The ImageEx control extends the default Image platform control improving the performance and responsiveness of your Apps.
/// Source images are downloaded asynchronously showing a load indicator while in progress.
/// Once downloaded, the source image is stored in the App local cache to preserve resources and load time next time the image needs to be displayed.
/// </summary>
public partial class ImageEx : ImageExBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageEx"/> class.
    /// </summary>
    public ImageEx()
        : base() => DefaultStyleKey = typeof(ImageEx);
}
