//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the query $orderby expression and produces the lexical object model.
/// </summary>
public class OrderByOptionParser : QueryOptionParser, IOrderByOptionParser
{
    public OrderByOptionParser()
    { }

    public virtual OrderByClause Parse(OrderByToken orderBy, QueryOptionParserContext context)
    {
        OrderByClause head = null;
        OrderByClause previous = null;

        OrderByToken token = orderBy;

        // Go through the orderby tokens starting from the last one
        while (token != null)
        {
            OrderByClause orderByClause = ProcessSingleOrderBy(token, context);

            if (head == null)
            {
                head = orderByClause;
            }
            else
            {
                previous.ThenBy = orderByClause;
            }

            previous = orderByClause;

            token = token.ThenBy;
        }

        return head;
    }

    /// <summary>
    /// Processes the specified order-by token.
    /// </summary>
    /// <param name="state">State to use for binding.</param>
    /// <param name="thenBy"> The next OrderBy node, or null if there is no orderby after this.</param>
    /// <param name="orderByToken">The order-by token to bind.</param>
    /// <returns>Returns the combined entityCollection including the ordering.</returns>
    private OrderByClause ProcessSingleOrderBy(OrderByToken orderByToken, QueryOptionParserContext context)
    {
       // ExceptionUtils.CheckArgumentNotNull(state, "state");
       // ExceptionUtils.CheckArgumentNotNull(orderByToken, "orderByToken");

        QueryNode expressionNode = Bind(orderByToken.Expression, context);

        //// The order-by expressions need to be primitive / enumeration types
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
