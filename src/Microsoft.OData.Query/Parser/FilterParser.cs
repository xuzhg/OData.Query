//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Metadata;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a filter clause.
/// </summary>
public class FilterParser : QueryBinder, IFilterParser
{
    /// <summary>
    /// Parses the $filter expression like "Name eq 'Sam'" to a search tree.
    /// </summary>
    /// <param name="filter">The $filter expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The filter token.</returns>
    public virtual async ValueTask<FilterClause> ParseAsync(ReadOnlyMemory<char> filter, QueryParserContext context)
    {
        if (filter.IsEmpty)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IFilterTokenizer filterTokenizer = context.GetOrCreateFilterTokenizer();

        IQueryToken token = await filterTokenizer.TokenizeAsync(filter, context.TokenizerContext);
        if (token == null)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        QueryNode expressionNode = Bind(token, context);

        SingleValueNode expressionResultNode = expressionNode as SingleValueNode;
        if (expressionResultNode == null ||
            (expressionResultNode.NodeType != null && !expressionResultNode.NodeType.IsPrimitiveTypeKind()))
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        PrimitiveTypeKind kind = expressionResultNode.NodeType.GetPrimitiveTypeKind();
        if (kind != PrimitiveTypeKind.Boolean)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotBooleanType");
        }

        FilterClause filterNode = new FilterClause(expressionResultNode, context.ImplicitRangeVariable);

        return filterNode;
    }
}
