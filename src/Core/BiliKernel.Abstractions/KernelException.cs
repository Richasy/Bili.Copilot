﻿// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel;

/// <summary>
/// Represents the base exception from which all Bili Kernel exceptions derive.
/// </summary>
/// <remarks>
/// Instances of this class optionally contain telemetry information in the Exception.Data property using keys that are consistent with the OpenTelemetry standard.
/// See https://opentelemetry.io/ for more information.
/// </remarks>
public class KernelException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KernelException"/> class.
    /// </summary>
    public KernelException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KernelException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public KernelException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KernelException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public KernelException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
