//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics.Contracts;
using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Metadata;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the query $orderby expression and produces the lexical object model.
/// </summary>
public class OrderByParser : QueryBinder, IOrderByParser
{
    /// <summary>
    /// Parses the $orderby expression.
    /// </summary>
    /// <param name="orderBy">The $orderby expression string to parse.</param>
    /// <param name="context">The parser context.</param>
    /// <returns>The order by clause parsed.</returns>
    public virtual async ValueTask<OrderByClause> ParseAsync(ReadOnlyMemory<char> orderBy, QueryParserContext context)
    {
        if (orderBy.IsEmpty)
        {
            throw new ArgumentNullException(nameof(orderBy));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IOrderByTokenizer tokenizer = context.GetOrCreateOrderByTokenizer();
        OrderByToken orderByToken = await tokenizer.TokenizeAsync(orderBy, context.TokenizerContext);
        if (orderByToken == null)
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_FailedToTokenizeExpression,
                orderBy.Length > 16 ? orderBy.Slice(0, 16).ToString() : orderBy.Span.ToString(), // 16 is an arbitrary length to avoid overly long strings in the error message
                "$orderby"));
        }

        OrderByClause head = BindSingleOrderBy(null, orderByToken, context);
        OrderByClause previous = head;
        OrderByToken token = orderByToken.ThenBy;

        while (token != null)
        {
            previous = BindSingleOrderBy(previous, token, context);
            token = token.ThenBy;
        }

        return head;
    }

    /// <summary>
    /// Binds the single order-by token.
    /// </summary>
    /// <param name="orderByToken">The previous binded orderby clause.</param>
    /// <param name="orderByToken">The order-by token to bind.</param>
    /// <param name="context">The parser context.</param>
    /// <returns>The single order-by clause.</returns>
    protected virtual OrderByClause BindSingleOrderBy(OrderByClause previous, OrderByToken orderByToken, QueryParserContext context)
    {
        Contract.Assert(orderByToken != null);
        Contract.Assert(context != null);

        QueryNode expressionNode = Bind(orderByToken.Expression, context);

        SingleValueNode expressionResultNode = expressionNode as SingleValueNode;
        if (expressionResultNode == null ||
            (expressionResultNode.NodeType != null &&
            !expressionResultNode.NodeType.IsPrimitiveTypeKind() &&
            !expressionResultNode.NodeType.IsEnumTypeKind()))
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_OrderByExpressionNotSingleValue, orderByToken.Expression));
        }

        OrderByClause orderByNode = new OrderByClause(
            //thenBy,
            expressionResultNode,
            orderByToken.Direction,
            context.ImplicitRangeVariable);

        if (previous != null)
        {
            previous.ThenBy = orderByNode;
        }

        return orderByNode;
    }
}
