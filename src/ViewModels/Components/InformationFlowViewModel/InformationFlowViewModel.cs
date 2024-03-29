﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Bili.Copilot.Models.App.Other;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 信息流视图模型.
/// </summary>
/// <typeparam name="T">核心数据集合的类型.</typeparam>
public abstract partial class InformationFlowViewModel<T> : ViewModelBase
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InformationFlowViewModel{T}"/> class.
    /// </summary>
    public InformationFlowViewModel()
    {
        Items = new ObservableCollection<T>();
        AttachIsRunningToAsyncCommand(p => IsIncrementalLoading = p, IncrementalCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayException, ReloadCommand, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, IncrementalCommand);
    }

    /// <summary>
    /// 显示错误信息.
    /// </summary>
    /// <param name="exception">错误实例.</param>
    public void DisplayException(Exception exception)
    {
        IsError = true;
        IsReloading = false;
        var msg = exception is ServiceException se
            ? se.GetMessage()
            : exception.Message;
        ErrorText = FormatException(msg);
        LogException(exception);
    }

    /// <summary>
    /// 在执行重新载入操作前的准备工作.
    /// </summary>
    protected virtual void BeforeReload()
    {
    }

    /// <summary>
    /// 从网络获取数据，并将其加入 <see cref="Items"/> 中.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    protected virtual Task GetDataAsync() => Task.CompletedTask;

    /// <summary>
    /// 拼接错误信息.
    /// </summary>
    /// <param name="errorMsg">原始错误信息.</param>
    /// <returns>格式化后的错误信息.</returns>
    protected virtual string FormatException(string errorMsg) => errorMsg;

    /// <summary>
    /// 滚动到顶部.
    /// </summary>
    protected void ScrollToTop()
        => RequestScrollToTop?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 清除错误信息.
    /// </summary>
    protected void ClearException()
    {
        IsError = false;
        ErrorText = string.Empty;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Items.Count > 0 || IsReloading)
        {
            return;
        }

        await ReloadAsync();
        IsInitialized = true;
    }

    [RelayCommand]
    private void ResetState()
    {
        BeforeReload();
        TryClear(Items);
        ClearException();
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (IsReloading)
        {
            return;
        }

        IsReloading = true;
        ResetState();

        try
        {
            IsReloading = true;
            await GetDataAsync();
            IsReloading = false;
            ScrollToTop();
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }

        IsReloading = false;
    }

    [RelayCommand]
    private async Task IncrementalAsync()
    {
        if (IsReloading)
        {
            return;
        }

        if (IsIncrementalLoading)
        {
            _isNeedLoadAgain = true;
            return;
        }

        await GetDataAsync();

        if (_isNeedLoadAgain)
        {
            _isNeedLoadAgain = false;
            await GetDataAsync();
        }
    }
}
