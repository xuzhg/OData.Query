//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Default factory to create <see cref="IExpressionLexer"/>.
/// </summary>
public class ExpressionLexerFactory : ILexerFactory
{
    /// <summary>
    /// The static default.
    /// </summary>
    internal static ExpressionLexerFactory Default = new ExpressionLexerFactory();

    /// <summary>
    /// Gets the token processed.
    /// </summary>
    public IExpressionLexer CreateLexer(string text, LexerOptions options)
    {
        return new ExpressionLexer(text, options);
    }
}