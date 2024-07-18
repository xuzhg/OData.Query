//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenizes the $filter query expression and produces the lexical object model.
/// </summary>
public class FilterOptionTokenizer : QueryTokenizer, IFilterOptionTokenizer
{
    internal static FilterOptionTokenizer Default = new FilterOptionTokenizer(ExpressionLexerFactory.Default);

    private ILexerFactory _lexerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterOptionTokenizer" /> class.
    /// </summary>
    /// <param name="factory"></param>
    public FilterOptionTokenizer(ILexerFactory factory)
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

        IExpressionLexer lexer = _lexerFactory.CreateLexer(filter, LexerOptions.Default);
        lexer.NextToken(); // move to first token

        IQueryToken result = TokenizeExpression(lexer, context);

        return await ValueTask.FromResult(result);
    }
}
