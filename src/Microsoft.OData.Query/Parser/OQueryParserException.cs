//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Represents a query option parser exception.
/// </summary>
public class OQueryParserException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OQueryParserException" /> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public OQueryParserException(string message) : base(message)
    { }
}
