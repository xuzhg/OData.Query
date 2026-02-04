//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Lexers;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// The extenions for tokenizer context
/// </summary>
internal static class QueryTokenizerContextExtensions
{
    public static IExpressionLexer CreateLexer(this QueryTokenizerContext context, ReadOnlyMemory<char> text)
    {
        Debug.Assert(context != null, "The tokenizer context should not be null.");

        ILexerFactory factory = context.GetOrCreateLexerFactory();
        Debug.Assert(factory != null, "Lexer factory should not be null. Since it is either retrieved from the service provider or a new instance is created.");

        return factory.CreateLexer(text, context.LexerOptions);
    }

    public static IExpressionLexer CreateLexer(this QueryTokenizerContext context, ReadOnlyMemory<char> text, LexerOptions options)
    {
        ILexerFactory factory = context.GetOrCreateLexerFactory();
        return factory.CreateLexer(text, options);
    }

    public static ILexerFactory GetOrCreateLexerFactory(this QueryTokenizerContext context)
        => context?.ServiceProvider?.GetService<ILexerFactory>() ?? new ExpressionLexerFactory();
}