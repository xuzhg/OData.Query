//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Lexers;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// The extenions for tokenizer context
/// </summary>
internal static class QueryTokenizerContextExtensions
{
    public static IExpressionLexer CreateLexer(this QueryTokenizerContext context, ReadOnlyMemory<char> text, LexerOptions options)
    {
        ILexerFactory factory = context.GetOrCreateLexerFactory();
        return factory.CreateLexer(text, options);
    }

    public static ILexerFactory GetOrCreateLexerFactory(this QueryTokenizerContext context)
        => context?.ServiceProvider?.GetService<ILexerFactory>() ?? new ExpressionLexerFactory();
}