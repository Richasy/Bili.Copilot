// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// ��ʾ����������¼�����.
/// </summary>
public sealed class ShowCommentEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCommentEventArgs"/> class.
    /// </summary>
    /// <param name="type">����������.</param>
    /// <param name="sortType">����������ʽ.</param>
    /// <param name="sourceId">����Դ Id.</param>
    public ShowCommentEventArgs(CommentType type, CommentSortType sortType, string sourceId)
    {
        Type = type;
        SortType = sortType;
        SourceId = sourceId;
    }

    /// <summary>
    /// ����������.
    /// </summary>
    public CommentType Type { get; }

    /// <summary>
    /// ����������ʽ.
    /// </summary>
    public CommentSortType SortType { get; }

    /// <summary>
    /// ����Դ Id.
    /// </summary>
    public string SourceId { get; }
}

