﻿// Copyright (c) Reader Copilot. All rights reserved.
// <auto-generated />

using Markdig.Syntax;
using Bili.Copilot.App.Controls.Markdown.TextElements;

namespace Bili.Copilot.App.Controls.Markdown.Renderers.ObjectRenderers;

internal class QuoteBlockRenderer : WinUIObjectRenderer<QuoteBlock>
{
    protected override void Write(WinUIRenderer renderer, QuoteBlock obj)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var quote = new MyQuote(obj);

        renderer.Push(quote);
        renderer.WriteChildren(obj);
        renderer.Pop();
    }
}
