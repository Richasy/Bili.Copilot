﻿// Copyright (c) Rodel. All rights reserved.
// <auto-generated />

using Markdig.Syntax;
using BiliCopilot.UI.Controls.Markdown.TextElements;

namespace BiliCopilot.UI.Controls.Markdown.Renderers.ObjectRenderers;

internal class ThematicBreakRenderer : WinUIObjectRenderer<ThematicBreakBlock>
{
    protected override void Write(WinUIRenderer renderer, ThematicBreakBlock obj)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var thematicBreak = new MyThematicBreak(obj);

        renderer.WriteBlock(thematicBreak);
    }
}
