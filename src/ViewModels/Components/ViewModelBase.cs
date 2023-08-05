// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Bili.Copilot.Models.App.Other;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视图模型基类.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    /// <summary>
    /// 日志记录器.
    /// </summary>
    protected Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 从错误中获取错误的消息文本.
    /// </summary>
    /// <param name="exception">抛出的异常.</param>
    /// <returns>错误消息.</returns>
    protected static string GetErrorMessage(Exception exception)
    {
        var msg = exception is ServiceException se
            ? se.GetMessage()
            : exception.Message;
        return msg;
    }

    /// <summary>
    /// 为异步命令添加错误回调.
    /// </summary>
    /// <param name="handler">错误回调.</param>
    /// <param name="commands">命令集.</param>
    protected static void AttachExceptionHandlerToAsyncCommand(Action<Exception> handler, params IAsyncRelayCommand[] commands)
    {
        foreach (var command in commands)
        {
            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AsyncRelayCommand.ExecutionTask) &&
                    ((IAsyncRelayCommand)s).ExecutionTask is Task task &&
                    task.Exception is AggregateException exception)
                {
                    exception.Handle(ex =>
                    {
                        handler(ex);
                        return true;
                    });
                }
            };
        }
    }

    /// <summary>
    /// 为异步命令的 <see cref="AsyncRelayCommand.IsRunning"/> 属性添加回调.
    /// </summary>
    /// <param name="handler">回调.</param>
    /// <param name="commands">命令集合.</param>
    protected static void AttachIsRunningToAsyncCommand(Action<bool> handler, params IAsyncRelayCommand[] commands)
    {
        foreach (var command in commands)
        {
            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AsyncRelayCommand.IsRunning))
                {
                    handler(command.IsRunning);
                }
            };
        }
    }

    /// <summary>
    /// 尝试清除集合.
    /// </summary>
    /// <typeparam name="T">条目类型.</typeparam>
    /// <param name="collection">集合.</param>
    protected static void TryClear<T>(ObservableCollection<T> collection)
    {
        try
        {
            collection.Clear();
        }
        catch (Exception)
        {
            // Do nothing.
        }
    }

    /// <summary>
    /// 记录错误信息.
    /// </summary>
    /// <param name="exception">错误信息.</param>
    protected void LogException(Exception exception)
    {
#if DEBUG
        Debug.WriteLine(exception.StackTrace);
#endif
        Logger.Error(exception);
    }
}
