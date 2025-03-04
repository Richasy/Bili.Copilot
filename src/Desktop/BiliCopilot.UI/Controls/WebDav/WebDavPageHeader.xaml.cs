// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;

namespace BiliCopilot.UI.Controls.WebDav;

/// <summary>
/// WebDAV 页面头部控件.
/// </summary>
public sealed partial class WebDavPageHeader : WebDavPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPageHeader"/> class.
    /// </summary>
    public WebDavPageHeader() => InitializeComponent();

    private void OnPathSegmentClick(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        var seg = args.Item as WebDavPathSegment;
        ViewModel.LoadPathCommand.Execute(seg.Path);
    }
}
