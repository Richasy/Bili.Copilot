// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// 根布局，用于包裹整个应用程序的布局.
/// </summary>
public sealed partial class RootLayout : LayoutUserControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RootLayout"/> class.
    /// </summary>
    public RootLayout()
    {
        InitializeComponent();
        InitializeSubtitle();
    }

    /// <summary>
    /// 获取主标题栏.
    /// </summary>
    /// <returns><see cref="AppTitleBar"/>.</returns>
    public AppTitleBar GetMainTitleBar() => MainTitleBar;

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
    }

    private void InitializeSubtitle()
    {
#if LOCAL_DEV
        var subtitles = new List<string>();
        subtitles.Add("🛠️");
#if DEBUG
        subtitles.Add("Debug");
#else
        subtitles.Add("Release");
#endif
#if ARCH_X64
        subtitles.Add("x64");
#elif ARCH_X86
        subtitles.Add("x86");
#elif ARCH_ARM64
        subtitles.Add("ARM64");
#endif
        if (subtitles.Count > 0)
        {
            MainTitleBar.Subtitle = string.Join(" | ", subtitles);
        }
#endif
    }

    private void OnNavViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        => OnBackRequested(default, default);

    private void OnNavViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        _ = this;
    }

    private void OnBackRequested(object sender, EventArgs e)
    {
    }
}
