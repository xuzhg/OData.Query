//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the query $orderby expression and produces the lexical object model.
/// </summary>
public class OrderByOptionParser : QueryOptionParser, IOrderByOptionParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByOptionParser" /> class.
    /// </summary>
    public OrderByOptionParser()
        : this(OrderByOptionTokenizer.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByOptionParser" /> class.
    /// </summary>
    /// <param name="tokenizer">The orderby option tokenizer.</param>
    public OrderByOptionParser(IOrderByOptionTokenizer tokenizer)
    {
        Tokenizer = tokenizer;
    }

    /// <summary>
    /// Gets the tokenizer.
    /// </summary>
    public IOrderByOptionTokenizer Tokenizer { get; }

    /// <summary>
    /// Parses the $orderby expression.
    /// </summary>
    /// <param name="orderBy">The $orderby expression string to parse.</param>
    /// <returns>The order by clause parsed.</returns>
    public virtual async ValueTask<OrderByClause> ParseAsync(string orderBy, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(orderBy))
        {
            throw new ArgumentNullException(nameof(orderBy));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        OrderByToken orderByToken = await Tokenizer.TokenizeAsync(orderBy, context.TokenizerContext);
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
