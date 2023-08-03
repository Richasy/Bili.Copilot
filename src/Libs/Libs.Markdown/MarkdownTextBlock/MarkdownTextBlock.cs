// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.Libs.Markdown;

/// <summary>
/// An efficient and extensible control that can parse and render markdown.
/// </summary>
public partial class MarkdownTextBlock : Control
{
    private long _fontSizePropertyToken;
    private long _flowDirectionPropertyToken;
    private long _backgroundPropertyToken;
    private long _borderBrushPropertyToken;
    private long _borderThicknessPropertyToken;
    private long _characterSpacingPropertyToken;
    private long _fontFamilyPropertyToken;
    private long _fontStretchPropertyToken;
    private long _fontStylePropertyToken;
    private long _fontWeightPropertyToken;
    private long _foregroundPropertyToken;
    private long _paddingPropertyToken;
    private long _requestedThemePropertyToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownTextBlock"/> class.
    /// </summary>
    public MarkdownTextBlock()
    {
        // Set our style.
        DefaultStyleKey = typeof(MarkdownTextBlock);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        RegisterThemeChangedHandler();

        // Grab our root
        _rootElement = GetTemplateChild("RootElement") as Border;

        // And make sure to render any markdown we have.
        RenderMarkdown();
    }

    private void ThemeListener_ThemeChanged(CommunityToolkit.WinUI.UI.Helpers.ThemeListener sender)
        => RenderMarkdown();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RegisterThemeChangedHandler();

        // Register for property callbacks that are owned by our parent class.
        _fontSizePropertyToken = RegisterPropertyChangedCallback(FontSizeProperty, OnPropertyChanged);
        _flowDirectionPropertyToken = RegisterPropertyChangedCallback(FlowDirectionProperty, OnPropertyChanged);
        _backgroundPropertyToken = RegisterPropertyChangedCallback(BackgroundProperty, OnPropertyChanged);
        _borderBrushPropertyToken = RegisterPropertyChangedCallback(BorderBrushProperty, OnPropertyChanged);
        _borderThicknessPropertyToken = RegisterPropertyChangedCallback(BorderThicknessProperty, OnPropertyChanged);
        _characterSpacingPropertyToken = RegisterPropertyChangedCallback(CharacterSpacingProperty, OnPropertyChanged);
        _fontFamilyPropertyToken = RegisterPropertyChangedCallback(FontFamilyProperty, OnPropertyChanged);
        _fontStretchPropertyToken = RegisterPropertyChangedCallback(FontStretchProperty, OnPropertyChanged);
        _fontStylePropertyToken = RegisterPropertyChangedCallback(FontStyleProperty, OnPropertyChanged);
        _fontWeightPropertyToken = RegisterPropertyChangedCallback(FontWeightProperty, OnPropertyChanged);
        _foregroundPropertyToken = RegisterPropertyChangedCallback(ForegroundProperty, OnPropertyChanged);
        _paddingPropertyToken = RegisterPropertyChangedCallback(PaddingProperty, OnPropertyChanged);
        _requestedThemePropertyToken = RegisterPropertyChangedCallback(RequestedThemeProperty, OnPropertyChanged);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_themeListener != null)
        {
            _themeListener.ThemeChanged -= ThemeListener_ThemeChanged;
            _themeListener.Dispose();
            _themeListener = null;
        }

        // Unregister property callbacks
        UnregisterPropertyChangedCallback(FontSizeProperty, _fontSizePropertyToken);
        UnregisterPropertyChangedCallback(FlowDirectionProperty, _flowDirectionPropertyToken);
        UnregisterPropertyChangedCallback(BackgroundProperty, _backgroundPropertyToken);
        UnregisterPropertyChangedCallback(BorderBrushProperty, _borderBrushPropertyToken);
        UnregisterPropertyChangedCallback(BorderThicknessProperty, _borderThicknessPropertyToken);
        UnregisterPropertyChangedCallback(CharacterSpacingProperty, _characterSpacingPropertyToken);
        UnregisterPropertyChangedCallback(FontFamilyProperty, _fontFamilyPropertyToken);
        UnregisterPropertyChangedCallback(FontStretchProperty, _fontStretchPropertyToken);
        UnregisterPropertyChangedCallback(FontStyleProperty, _fontStylePropertyToken);
        UnregisterPropertyChangedCallback(FontWeightProperty, _fontWeightPropertyToken);
        UnregisterPropertyChangedCallback(ForegroundProperty, _foregroundPropertyToken);
        UnregisterPropertyChangedCallback(PaddingProperty, _paddingPropertyToken);
        UnregisterPropertyChangedCallback(RequestedThemeProperty, _requestedThemePropertyToken);
    }

    private void RegisterThemeChangedHandler()
    {
        _themeListener ??= new CommunityToolkit.WinUI.UI.Helpers.ThemeListener();
        _themeListener.ThemeChanged -= ThemeListener_ThemeChanged;
        _themeListener.ThemeChanged += ThemeListener_ThemeChanged;
    }
}
