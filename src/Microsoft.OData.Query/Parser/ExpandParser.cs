//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse an $expand clause.
/// </summary>
public class ExpandParser : QueryBinder, IExpandParser
{
    /// <summary>
    /// Parses the $expand expression to a search tree.
    /// </summary>
    /// <param name="expand">The $expand expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The expand clause parsed.</returns>
    public virtual async ValueTask<ExpandClause> ParseAsync(ReadOnlyMemory<char> expand, QueryParserContext context)
    {
        if (expand.IsEmpty)
        {
            throw new ArgumentNullException(nameof(expand));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IExpandTokenizer tokenizer = context.GetOrCreateExpandTokenizer();

        ExpandToken token = await tokenizer.TokenizeAsync(expand, context.TokenizerContext);
        if (token == null)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        // TODO: Normalize the expand token

        // $expand=Nav1;Nav2($expand=...;$select=...)
        ExpandClause expandClause = new ExpandClause();
        foreach (ExpandItemToken item in token)
        {
            expandClause.Add(BindExpandItem(item, context));
        }

        return expandClause;
    }

    protected virtual ExpandedItem BindExpandItem(ExpandItemToken expandItem, QueryParserContext context)
    {
        return null;
    }
}