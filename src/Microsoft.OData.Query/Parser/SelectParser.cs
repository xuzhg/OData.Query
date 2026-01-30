//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a $select clause.
/// </summary>
public class SelectParser : QueryBinder, ISelectParser
{
    /// <summary>
    /// Parses the $select expression to a search tree.
    /// </summary>
    /// <param name="select">The $select expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The select clause parsed.</returns>
    public virtual async ValueTask<SelectClause> ParseAsync(ReadOnlyMemory<char> select, QueryParserContext context)
    {
        if (select.IsEmpty)
        {
            throw new ArgumentNullException(nameof(select));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        ISelectTokenizer tokenizer = context.GetOrCreateSelectTokenizer();

        IQueryToken token = await tokenizer.TokenizeAsync(select, context.TokenizerContext);
        if (token == null)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        QueryNode expressionNode = Bind(token, context);

        SelectClause selectClause = new SelectClause(/*expressionResultNode, context.ImplicitRangeVariable*/);

        return selectClause;
    }
}
