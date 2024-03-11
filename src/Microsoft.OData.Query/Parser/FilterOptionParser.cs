//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Metadata;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a filter clause.
/// </summary>
public class FilterOptionParser : QueryOptionParser, IFilterOptionParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterOptionParser" /> class.
    /// </summary>
    public FilterOptionParser()
        : this(FilterOptionTokenizer.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterOptionParser" /> class.
    /// </summary>
    /// <param name="tokenizer">The filter option tokenizer.</param>
    public FilterOptionParser(IFilterOptionTokenizer tokenizer)
    {
        Tokenizer = tokenizer;
    }

    /// <summary>
    /// Gets the tokenizer.
    /// </summary>
    public IFilterOptionTokenizer Tokenizer { get; }

    public virtual FilterClause Parse(IQueryToken filter, QueryParserContext context)
    {
        QueryNode expressionNode = Bind(filter, context);

        SingleValueNode expressionResultNode = expressionNode as SingleValueNode;
        if (expressionResultNode == null ||
            (expressionResultNode.NodeType != null && !expressionResultNode.NodeType.IsPrimitiveTypeKind()))
        {
            throw new Exception("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        PrimitiveTypeKind kind = expressionResultNode.NodeType.GetPrimitiveTypeKind();
        if (kind != PrimitiveTypeKind.Boolean)
        {
            throw new Exception("ODataErrorStrings.MetadataBinder_FilterExpressionNotBooleanType");
        }

        FilterClause filterNode = new FilterClause(expressionResultNode, context.ImplicitRangeVariable);

        return filterNode;
    }

    /// <summary>
    /// Parses the $filter expression like "Name eq 'Sam'" to a search tree.
    /// </summary>
    /// <param name="filter">The $filter expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The filter token.</returns>
    public virtual async ValueTask<FilterClause> ParseAsync(string filter, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(filter))
        {
            throw new ArgumentNullException(nameof(filter));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IQueryToken token = await Tokenizer.TokenizeAsync(filter, context.TokenizerContext);
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
