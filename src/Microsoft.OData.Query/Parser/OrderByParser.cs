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
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        OrderByClause head = BindSingleOrderBy(orderByToken, context);
        OrderByClause previous = head;
        OrderByToken token = orderByToken.ThenBy;

        while (token != null)
        {
            OrderByClause orderByClause = BindSingleOrderBy(token, context);
            previous.ThenBy = orderByClause;
            previous = orderByClause;
            token = token.ThenBy;
        }

        return head;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="orderByToken"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual OrderByClause BindSingleOrderBy(OrderByToken orderByToken, QueryParserContext context)
    {
        QueryNode expressionNode = Bind(orderByToken.Expression, context);

        SingleValueNode expressionResultNode = expressionNode as SingleValueNode;
        //if (expressionResultNode == null ||
        //    (expressionResultNode.TypeReference != null &&
        //    !expressionResultNode.TypeReference.IsODataPrimitiveTypeKind() &&
        //    !expressionResultNode.TypeReference.IsODataEnumTypeKind() &&
        //    !expressionResultNode.TypeReference.IsODataTypeDefinitionTypeKind()))
        //{
        //    throw new ODataException(ODataErrorStrings.MetadataBinder_OrderByExpressionNotSingleValue);
        //}

        OrderByClause orderByNode = new OrderByClause(
            //thenBy,
            expressionResultNode,
            orderByToken.Direction,
            context.ImplicitRangeVariable);

        return orderByNode;
    }
}
