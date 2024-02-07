//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Represents a query tokenization exception.
/// </summary>
public class QueryTokenizerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryTokenizerException" /> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public QueryTokenizerException(string message) : base(message)
    { }
}
