// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Appearance;

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// ��ʾͼƬ�¼�����.
/// </summary>
public class ShowImageEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShowImageEventArgs"/> class.
    /// </summary>
    /// <param name="images">ͼƬ��ַ�б�.</param>
    /// <param name="firstIndex">��ʼ����.</param>
    public ShowImageEventArgs(IEnumerable<Image> images, int firstIndex = 0)
    {
        Images = images;
        ShowIndex = firstIndex;
    }

    /// <summary>
    /// ͼƬ��ַ.
    /// </summary>
    public IEnumerable<Image> Images { get; set; }

    /// <summary>
    /// ��ʼ��ʾͼƬ����.
    /// </summary>
    public int ShowIndex { get; set; }
}

