// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Local;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 固定模块视图模型.
/// </summary>
public sealed partial class FixModuleViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FixModuleViewModel"/> class.
    /// </summary>
    private FixModuleViewModel()
    {
        FixedItemCollection = new();
        HasFixedItems = false;

        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, AddFixedItemCommand, RemoveFixedItemCommand, InitializeCommand);
    }

    /// <summary>
    /// 新增固定的条目.
    /// </summary>
    /// <param name="item">条目信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private async Task AddFixedItemAsync(FixedItem item)
    {
        if (AccountViewModel.Instance.AccountInformation == null || FixedItemCollection.Contains(item))
        {
            return;
        }

        FixedItemCollection.Add(item);
        await FileToolkit.WriteLocalDataAsync(
            string.Format(AppConstants.FixedContentFileName, AuthorizeProvider.Instance.CurrentUserId),
            FixedItemCollection.ToList(),
            AppConstants.FixedFolderName);
        HasFixedItems = true;
        AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.Fixed), InfoType.Success);
    }

    /// <summary>
    /// 移除固定的条目.
    /// </summary>
    /// <param name="itemId">条目Id.</param>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private async Task RemoveFixedItemAsync(string itemId)
    {
        if (AccountViewModel.Instance.AccountInformation == null || !FixedItemCollection.Any(p => p.Id == itemId))
        {
            return;
        }

        FixedItemCollection.Remove(FixedItemCollection.FirstOrDefault(p => p.Id == itemId));
        await FileToolkit.WriteLocalDataAsync(
            string.Format(AppConstants.FixedContentFileName, AuthorizeProvider.Instance.CurrentUserId),
            FixedItemCollection.ToList(),
            AppConstants.FixedFolderName);
        HasFixedItems = FixedItemCollection.Count > 0;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (!string.IsNullOrEmpty(AuthorizeProvider.Instance.CurrentUserId))
        {
            var data = await FileToolkit.ReadLocalDataAsync<List<FixedItem>>(
                string.Format(AppConstants.FixedContentFileName, AuthorizeProvider.Instance.CurrentUserId),
                "[]",
                AppConstants.FixedFolderName);
            TryClear(FixedItemCollection);
            if (data.Count > 0)
            {
                data.ForEach(FixedItemCollection.Add);
                HasFixedItems = true;
                return;
            }
        }

        HasFixedItems = false;
    }
}
