// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Base;

internal class StaggeredItem
{
    public StaggeredItem(int index) => Index = index;

    public double Top { get; internal set; }

    public double Height { get; internal set; }

    public int Index { get; }

    public UIElement Element { get; internal set; }
}
