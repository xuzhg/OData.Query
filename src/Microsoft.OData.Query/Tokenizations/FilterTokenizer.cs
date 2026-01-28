//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $filter query expression and produces the lexical object model.
/// </summary>
public class FilterTokenizer : QueryTokenizer, IFilterTokenizer
{
    internal static FilterTokenizer Default = new FilterTokenizer(ExpressionLexerFactory.Default);

    private ILexerFactory _lexerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterTokenizer" /> class.
    /// </summary>
    /// <param name="factory"></param>
    public FilterTokenizer(ILexerFactory factory)
    {
        _lexerFactory = factory;
    }

    /// <summary>
    /// Tokenizes the $filter expression like "Name eq 'Sam'" to tokens.
    /// </summary>
    /// <param name="filter">The $filter expression string to tokenize.</param>
    /// <returns>The filter token tokenized.</returns>
    public virtual async ValueTask<IQueryToken> TokenizeAsync(string filter, QueryTokenizerContext context)
    {
        if (string.IsNullOrEmpty(filter))
        {
            throw new ArgumentNullException(nameof(filter));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IExpressionLexer lexer = _lexerFactory.CreateLexer(filter.AsMemory(), LexerOptions.Default);
        lexer.NextToken(); // move to first token

        IQueryToken result = TokenizeExpression(lexer, context);

        return await ValueTask.FromResult(result);
    }

    /// <summary>
    /// Tokenizes the $filter expression like "Name eq 'Sam'".
    /// </summary>
    /// <param name="filter">The $filter expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The filter token tokenized.</returns>
    public virtual async ValueTask<IQueryToken> TokenizeAsync(ReadOnlyMemory<char> filter, QueryTokenizerContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IExpressionLexer lexer = context.CreateLexer(filter);

        lexer.NextToken(); // move to first token

        IQueryToken result = TokenizeExpression(lexer, context);

        return await ValueTask.FromResult(result);
    }
}
