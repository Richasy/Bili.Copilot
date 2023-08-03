// Copyright (c) Bili Copilot. All rights reserved.

using Markdig.Syntax.Inlines;

namespace Bili.Copilot.Libs.Markdown.Renderers.Inlines;

internal sealed class HtmlEntityInlineRenderer : WinUIObjectRenderer<HtmlEntityInline>
{
    protected override void Write(WinUIRenderer renderer, HtmlEntityInline obj)
    {
        var transcoded = obj.Transcoded;
        renderer.WriteText(ref transcoded);
    }
}
