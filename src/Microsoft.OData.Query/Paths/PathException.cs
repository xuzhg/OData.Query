//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Paths;

/// <summary>
/// Represents a path related exception.
/// </summary>
public class PathException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PathException" /> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public PathException(string message) : base(message)
    { }
}
