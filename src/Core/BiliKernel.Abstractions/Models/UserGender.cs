// Copyright (c) Richasy. All rights reserved.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Models;

/// <summary>
/// 用户性别.
/// </summary>
public enum UserGender
{
    /// <summary>
    /// 保密.
    /// </summary>
    Secret = 0,

    /// <summary>
    /// 男性.
    /// </summary>
    Male = 1,

    /// <summary>
    /// 女性.
    /// </summary>
    Female = 2,
}

/// <summary>
/// 用户性别转换器.
/// </summary>
public sealed class DefaultUserGenderConverter : JsonConverter<UserGender?>
{
    /// <inheritdoc/>
    public override UserGender? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetInt32();
        return (UserGender)value;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, UserGender? value, JsonSerializerOptions options)
        => writer.WriteNumberValue((int)(value ?? UserGender.Secret));
}
