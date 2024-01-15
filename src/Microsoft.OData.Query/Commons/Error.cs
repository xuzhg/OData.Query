//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.OData.Query.Commons;

[ExcludeFromCodeCoverage]
internal static class Error
{
    /// <summary>
    /// Formats the specified resource string using <see cref="CultureInfo.CurrentCulture"/>.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>The formatted string.</returns>
    internal static string Format(string format, params object[] args)
        => string.Format(CultureInfo.CurrentCulture, format, args);
}
