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
/// A default parser to parse a $compute clause.
/// </summary>
public class ComputeParser : QueryBinder, IComputeParser
{
    /// <summary>
    /// Parses the $compute expression to a search tree.
    /// </summary>
    /// <param name="select">The $compute expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The filter token.</returns>
    public virtual async ValueTask<ComputeClause> ParseAsync(ReadOnlyMemory<char> compute, QueryParserContext context)
    {
        if (compute.IsEmpty)
        {
            throw new ArgumentNullException(nameof(compute));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IComputeTokenizer tokenizer = context.GetOrCreateComputeTokenizer();

        ComputeToken computeToken = await tokenizer.TokenizeAsync(compute, context.TokenizerContext);
        if (computeToken == null)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        ComputeClause computeClause = new ComputeClause(/*expressionResultNode, context.ImplicitRangeVariable*/);
        foreach (ComputeItemToken item in computeToken.Items)
        {
            ComputedItem computeItem = BindComputeItem(item, context);
            computeClause.Add(computeItem);
        }

        return computeClause;
    }

    protected virtual ComputedItem BindComputeItem(ComputeItemToken item, QueryParserContext context)
    {
        SingleValueNode node = Bind(item.Expression, context) as SingleValueNode;
        return new ComputedItem(node, item.Alias, node.NodeType);
    }
}