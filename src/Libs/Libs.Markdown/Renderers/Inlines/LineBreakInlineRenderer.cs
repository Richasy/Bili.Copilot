// Copyright (c) Bili Copilot. All rights reserved.

using Markdig.Syntax.Inlines;
using Microsoft.UI.Xaml.Documents;

namespace Bili.Copilot.Libs.Markdown.Renderers.Inlines;

internal sealed class LineBreakInlineRenderer : WinUIObjectRenderer<LineBreakInline>
{
    protected override void Write(WinUIRenderer renderer, LineBreakInline obj)
    {
        if (obj.IsHard)
        {
            renderer.Add(new LineBreak());
        }
        else
        {
            // Do nothing.
        }
    }
}
