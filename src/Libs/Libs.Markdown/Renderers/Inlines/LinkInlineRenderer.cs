// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using Markdig.Syntax.Inlines;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Bili.Copilot.Libs.Markdown.Renderers.Inlines;

internal sealed class LinkInlineRenderer : WinUIObjectRenderer<LinkInline>
{
    protected override void Write(WinUIRenderer renderer, LinkInline obj)
    {
        var context = renderer.Context;
        var isValidLink = Uri.TryCreate(obj.Url, UriKind.Absolute, out var url);
        if (!isValidLink)
        {
            renderer.WriteText(obj.Url ?? obj.Label ?? string.Empty);
            return;
        }

        if (obj.IsImage)
        {
            // Draw image
            var imgSource = GetImageSource(url);
            if (imgSource != null)
            {
                var hyperLinkBtn = new HyperlinkButton
                {
                    Background = new SolidColorBrush(Colors.Transparent),
                    Padding = new Microsoft.UI.Xaml.Thickness(0),
                    NavigateUri = url,
                    CornerRadius = context.CornerRadius,
                    MinWidth = 20,
                    MinHeight = 20,
                    Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0),
                };

                var image = new Image
                {
                    Source = imgSource,
                    Stretch = context.ImageStretch,
                    HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left,
                    VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top,
                };

                if (context.ImageMaxWidth > 0)
                {
                    image.MaxWidth = context.ImageMaxWidth;
                }

                if (context.ImageMaxHeight > 0)
                {
                    image.MaxHeight = context.ImageMaxHeight;
                }

                hyperLinkBtn.Content = image;
                if (!string.IsNullOrEmpty(obj.Label))
                {
                    ToolTipService.SetToolTip(hyperLinkBtn, obj.Label);
                }
                else if (obj.FirstChild is LiteralInline literal)
                {
                    ToolTipService.SetToolTip(hyperLinkBtn, literal.Content.ToString());
                }

                var container = new InlineUIContainer { Child = hyperLinkBtn };
                renderer.Add(container);
                return;
            }
        }

        var hyperLink = new Hyperlink
        {
            NavigateUri = url,
            Foreground = context.LinkForeground ?? context.Foreground,
        };

        if (!string.IsNullOrEmpty(obj.Label))
        {
            hyperLink.Inlines.Add(new Run { Text = obj.Label });
        }
        else if (obj.FirstChild is LiteralInline literal)
        {
            hyperLink.Inlines.Add(new Run { Text = literal.Content.ToString() });
        }
        else
        {
            hyperLink.Inlines.Add(new Run { Text = obj.Url });
        }

        renderer.Add(hyperLink);
        renderer.ExtractSpanAsChild();

        static ImageSource GetImageSource(Uri imageUrl)
        {
            return Path.GetExtension(imageUrl.AbsolutePath)?.ToLowerInvariant() == ".svg"
                ? new SvgImageSource(imageUrl)
                : new BitmapImage(imageUrl);
        }
    }
}
