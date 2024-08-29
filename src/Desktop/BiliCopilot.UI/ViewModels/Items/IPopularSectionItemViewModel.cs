// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 流行视频区块项视图模型.
/// </summary>
public interface IPopularSectionItemViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 区块标题.
    /// </summary>
    string Title { get; }
}
