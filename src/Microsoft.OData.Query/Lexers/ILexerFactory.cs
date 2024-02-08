//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// The factory to create <see cref="IExpressionLexer"/>.
/// </summary>
public interface ILexerFactory
{
    /// <summary>
    /// Create the <see cref="IExpressionLexer"/>.
    /// </summary>
    IExpressionLexer CreateLexer(string text, LexerOptions options);
}
