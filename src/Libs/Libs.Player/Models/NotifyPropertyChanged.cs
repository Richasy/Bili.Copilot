// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 可观察对象.
/// </summary>
public class ObservableObject : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// 设置属性的值，并触发属性更改事件.
    /// </summary>
    /// <typeparam name="T">属性的类型.</typeparam>
    /// <param name="field">要设置的属性字段.</param>
    /// <param name="value">要设置的属性值.</param>
    /// <param name="check">是否检查属性值是否相等.</param>
    /// <param name="propertyName">属性的名称.</param>
    /// <returns>如果属性值发生了更改，则返回 true；否则返回 false.</returns>
    protected bool Set<T>(ref T field, T value, bool check = true, [CallerMemberName] string propertyName = default)
    {
        if (!check || (field == null && value != null) || (field != null && !field.Equals(value)))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }

        return false;
    }

    /// <summary>
    /// 设置属性的值，并在 UI 线程上触发属性更改事件.
    /// </summary>
    /// <typeparam name="T">属性的类型.</typeparam>
    /// <param name="field">要设置的属性字段.</param>
    /// <param name="value">要设置的属性值.</param>
    /// <param name="check">是否检查属性值是否相等.</param>
    /// <param name="propertyName">属性的名称.</param>
    /// <returns>如果属性值发生了更改，则返回 true；否则返回 false.</returns>
    protected bool SetUI<T>(ref T field, T value, bool check = true, [CallerMemberName] string propertyName = default)
    {
        if (!check || (field == null && value != null) || (field != null && !field.Equals(value)))
        {
            field = value;
            Utils.UI(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));

            return true;
        }

        return false;
    }

    /// <summary>
    /// 触发属性更改事件.
    /// </summary>
    /// <param name="propertyName">属性的名称.</param>
    protected void Raise([CallerMemberName] string propertyName = default)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// 在 UI 线程上触发属性更改事件.
    /// </summary>
    /// <param name="propertyName">属性的名称.</param>
    protected void RaiseUI([CallerMemberName] string propertyName = default)
        => Utils.UI(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
}
