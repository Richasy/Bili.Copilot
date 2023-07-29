// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Bili.Copilot.App.Extensions;

/// <summary>
/// 视觉树扩展.
/// </summary>
public static class VisualTreeExtensions
{
    /// <summary>
    /// 绑定大小.
    /// </summary>
    /// <param name="target">目标元素.</param>
    /// <param name="source">来源视图.</param>
    public static void BindSize(this Visual target, Visual source)
    {
        var exp = target.Compositor.CreateExpressionAnimation("host.Size");
        exp.SetReferenceParameter("host", source);
        target.StartAnimation("Size", exp);
    }

    /// <summary>
    /// 通过名称查找指定的后裔元素.
    /// </summary>
    /// <typeparam name="T">元素类型.</typeparam>
    /// <param name="element">从这个元素开始往下查找.</param>
    /// <param name="name">指定元素的名称.</param>
    /// <returns>指定元素，如果没找到则返回<c>null</c>.</returns>
    public static T FindDescendantElementByName<T>(this DependencyObject element, string name)
        where T : FrameworkElement
    {
        if (element == null || string.IsNullOrWhiteSpace(name))
        {
            return default;
        }

        if (name.Equals((element as FrameworkElement)?.Name, StringComparison.OrdinalIgnoreCase))
        {
            return element as T;
        }

        var childCount = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < childCount; i++)
        {
            var result = VisualTreeHelper.GetChild(element, i).FindDescendantElementByName<T>(name);
            if (result != null)
            {
                return result;
            }
        }

        return default;
    }

    /// <summary>
    /// 通过类型查找指定的后裔元素.
    /// </summary>
    /// <typeparam name="T">查找元素的类型.</typeparam>
    /// <param name="element">从这个元素开始往下查找.</param>
    /// <returns>指定元素，如果没找到则返回<c>null</c>.</returns>
    public static T FindDescendantElementByType<T>(this DependencyObject element)
        where T : DependencyObject
    {
        T retValue = null;
        var childrenCount = VisualTreeHelper.GetChildrenCount(element);

        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child is T type)
            {
                retValue = type;
                break;
            }

            retValue = FindDescendantElementByType<T>(child);

            if (retValue != null)
            {
                break;
            }
        }

        return retValue;
    }

    /// <summary>
    /// 通过名称查找指定的祖先元素.
    /// </summary>
    /// <typeparam name="T">元素类型.</typeparam>
    /// <param name="element">从这个元素开始往上查找.</param>
    /// <param name="name">指定元素的名称.</param>
    /// <returns>指定元素，如果没找到则返回<c>null</c>.</returns>
    public static T FindAscendantElementByName<T>(this DependencyObject element, string name)
        where T : FrameworkElement
    {
        if (element == null || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var parent = VisualTreeHelper.GetParent(element);

        return parent == null
            ? null
            : name.Equals((parent as FrameworkElement)?.Name, StringComparison.OrdinalIgnoreCase)
            ? parent as T
            : parent.FindAscendantElementByName<T>(name);
    }

    /// <summary>
    /// 通过类型查找指定的祖先元素.
    /// </summary>
    /// <typeparam name="T">查找元素的类型.</typeparam>
    /// <param name="element">从这个元素开始往上查找.</param>
    /// <returns>指定元素，如果没找到则返回<c>null</c>.</returns>
    public static T FindAscendantElementByType<T>(this DependencyObject element)
        where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(element);
        return parent == null ? null : parent.GetType() == typeof(T) ? (T)parent : parent.FindAscendantElementByType<T>();
    }
}
