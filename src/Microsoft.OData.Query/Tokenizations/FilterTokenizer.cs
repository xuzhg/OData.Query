//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $filter query expression and produces the query token object.
/// </summary>
public class FilterTokenizer : QueryTokenizer, IFilterTokenizer
{
    /// <summary>
    /// Tokenizes the $filter expression like "Name eq 'Sam'".
    /// </summary>
    /// <param name="filter">The $filter expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The filter token tokenized.</returns>
    public virtual async ValueTask<IQueryToken> TokenizeAsync(ReadOnlyMemory<char> filter, QueryTokenizerContext context)
    {
        if (filter.IsEmpty)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IExpressionLexer lexer = context.CreateLexer(filter);
        if (lexer == null)
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryTokenizer_FailToCreateLexer, "$filter"));
        }

        lexer.NextToken(); // move to first token

        IQueryToken result = TokenizeExpression(lexer, context);

        return result;
    }
}
