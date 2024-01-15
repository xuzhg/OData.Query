//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Represents a lexical expression tokenization exception.
/// </summary>
public class OTokenizationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OTokenizationException" /> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public OTokenizationException(string message) : base(message)
    { }
}
