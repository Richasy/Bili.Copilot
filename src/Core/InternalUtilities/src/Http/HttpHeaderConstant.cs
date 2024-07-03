// Copyright (c) Richasy. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Richasy.BiliKernel.Http;

/// <summary>Provides HTTP header names and values for common purposes.</summary>
[ExcludeFromCodeCoverage]
internal static class HttpHeaderConstant
{
    public static class Names
    {
        /// <summary>HTTP header name to use to include the Reader Kernel package version in all HTTP requests issued by Reader Kernel.</summary>
        public static string SemanticKernelVersion => "Semantic-Kernel-Version";
    }

    public static class Values
    {
        /// <summary>User agent string to use for all HTTP requests issued by Reader Kernel.</summary>
        public static string UserAgent => "Semantic-Kernel";

        /// <summary>
        /// Gets the version of the <see cref="System.Reflection.Assembly"/> in which the specific type is declared.
        /// </summary>
        /// <param name="type">Type for which the assembly version is returned.</param>
        public static string GetAssemblyVersion(Type type)
        {
            return type.Assembly.GetName().Version!.ToString();
        }
    }
}
