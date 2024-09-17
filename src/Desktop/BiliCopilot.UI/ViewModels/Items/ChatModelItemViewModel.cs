// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Models;
using BiliCopilot.UI.Controls.AI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUI.Share.ViewModels;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 聊天模型项视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class ChatModelItemViewModel : ViewModelBase<ChatModel>
{
    private readonly Action<ChatModelItemViewModel> _deleteAction;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatModelItemViewModel"/> class.
    /// </summary>
    public ChatModelItemViewModel(ChatModel model, Action<ChatModelItemViewModel> deleteAction = null)
        : base(model)
    {
        Name = model.DisplayName;
        Id = model.Id;
        _deleteAction = deleteAction;
    }

    [RelayCommand]
    private void Delete()
        => _deleteAction?.Invoke(this);

    [RelayCommand]
    private async Task ModifyAsync()
    {
        var dialog = new CustomChatModelDialog(Data);
        var dialogResult = await dialog.ShowAsync();
        if (dialogResult == ContentDialogResult.Primary)
        {
            Name = dialog.Model.DisplayName;
            Id = dialog.Model.Id;
        }
    }

    partial void OnNameChanged(string value)
    {
        if (Data.DisplayName != value)
        {
            Data.DisplayName = value;
        }
    }

    partial void OnIdChanged(string value)
    {
        if (Data.Id != value)
        {
            Data.Id = value;
        }
    }
}
