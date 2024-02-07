//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Lexical tokenization is conversion of a text into meaningful lexical tokens.
/// </summary>
public interface ILexerFactory
{
    /// <summary>
    /// Gets the token processed.
    /// </summary>
    IExpressionLexer CreateLexer(string text, LexerOptions options);
}

public class ExpressionLexerFactory : ILexerFactory
{
    internal static ExpressionLexerFactory Default = new ExpressionLexerFactory();

    /// <summary>
    /// Gets the token processed.
    /// </summary>
    public IExpressionLexer CreateLexer(string text, LexerOptions options)
    {
        return new ExpressionLexer(text, options);
    }
}