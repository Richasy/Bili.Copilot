// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Richasy.WinUI.Share.Base;
using Windows.UI.ViewManagement;

namespace BiliCopilot.UI.Controls.AI;

/// <summary>
/// AI Logo 控件.
/// </summary>
public sealed partial class AILogo : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="Provider"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ProviderProperty =
        DependencyProperty.Register(
            nameof(Provider),
            typeof(string),
            typeof(AILogo),
            new PropertyMetadata("OpenAI", new PropertyChangedCallback(OnProviderChanged)));

    /// <summary>
    /// <see cref="AvatarPadding"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty AvatarPaddingProperty =
        DependencyProperty.Register(
            nameof(AvatarPadding),
            typeof(Thickness),
            typeof(AILogo),
            new PropertyMetadata(new Thickness(6)));

    /// <summary>
    /// <see cref="IsAvatar"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsAvatarProperty =
        DependencyProperty.Register(nameof(IsAvatar), typeof(bool), typeof(AILogo), new PropertyMetadata(false));

    /// <summary>
    /// Initializes a new instance of the <see cref="AILogo"/> class.
    /// </summary>
    public AILogo() => InitializeComponent();

    /// <summary>
    /// 服务提供商.
    /// </summary>
    public string Provider
    {
        get => (string)GetValue(ProviderProperty);
        set => SetValue(ProviderProperty, value);
    }

    /// <summary>
    /// 是否为头像.
    /// </summary>
    public bool IsAvatar
    {
        get => (bool)GetValue(IsAvatarProperty);
        set => SetValue(IsAvatarProperty, value);
    }

    /// <summary>
    /// 头像模式下的内距.
    /// </summary>
    public Thickness AvatarPadding
    {
        get => (Thickness)GetValue(AvatarPaddingProperty);
        set => SetValue(AvatarPaddingProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ResetLogo();

    private static void OnProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((AILogo)d).ResetLogo();

    private void ResetLogo()
    {
        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        var highContrast = new AccessibilitySettings().HighContrast;
        var themeText = highContrast
            ? "dark"
            : localTheme == ElementTheme.Default
                ? App.Current.RequestedTheme.ToString().ToLower()
                : localTheme.ToString().ToLower();

        var logoFileName = IsAvatar
            ? $"ms-appx:///Assets/Providers/{Provider.ToLower()}-avatar.png"
            : $"ms-appx:///Assets/Providers/{Provider.ToLower()}-{themeText}.png";
        if (Logo != null)
        {
            Logo.Source = new BitmapImage(new Uri(logoFileName));
        }

        var stateName = IsAvatar ? nameof(AvatarState) : nameof(FullState);
        AvatarContainer.Background = IsAvatar ? GetLogoBrush() : new SolidColorBrush(Colors.Transparent);
        AvatarContainer.Padding = IsAvatar ? AvatarPadding : new Thickness(0);
        VisualStateManager.GoToState(this, stateName, false);
    }

    private Brush GetLogoBrush()
    {
        var brushName = $"{Provider}Color";
        return Application.Current.Resources.ContainsKey(brushName) ? (Brush)Application.Current.Resources[brushName] : new SolidColorBrush(Colors.Transparent);
    }
}
