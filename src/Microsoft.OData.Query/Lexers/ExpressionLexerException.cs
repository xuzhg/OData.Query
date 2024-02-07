//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Represents a lexical expression lexer exception.
/// </summary>
public class ExpressionLexerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionLexerException" /> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ExpressionLexerException(string message) : base(message)
    { }
}
