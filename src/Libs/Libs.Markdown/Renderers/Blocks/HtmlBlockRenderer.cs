// Copyright (c) Bili Copilot. All rights reserved.

using Markdig.Syntax;

namespace Bili.Copilot.Libs.Markdown.Renderers.Blocks;

internal sealed class HtmlBlockRenderer : WinUIObjectRenderer<HtmlBlock>
{
    protected override void Write(WinUIRenderer renderer, HtmlBlock obj)
    {
        renderer.WriteLeafRawLines(obj);
    }
}
