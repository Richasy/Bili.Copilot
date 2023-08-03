// Copyright (c) Bili Copilot. All rights reserved.

using Markdig.Syntax.Inlines;

namespace Bili.Copilot.Libs.Markdown.Renderers.Inlines;

internal sealed class LiteralInlineRenderer : WinUIObjectRenderer<LiteralInline>
{
    protected override void Write(WinUIRenderer renderer, LiteralInline obj)
    {
        if (obj.Content.IsEmpty)
        {
            return;
        }

        renderer.WriteText(ref obj.Content);
    }
}
