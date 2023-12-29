// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// A delegate for <see cref="ImageEx"/> opened.
/// </summary>
/// <param name="sender">The sender.</param>
/// <param name="e">The event arguments.</param>
public delegate void ImageExOpenedEventHandler(object sender, ImageExOpenedEventArgs e);

/// <summary>
/// Provides data for the <see cref="ImageEx"/> ImageOpened event.
/// </summary>
public class ImageExOpenedEventArgs : EventArgs
{
}
