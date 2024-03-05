//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Metadata;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

public class FilterOptionParser : QueryOptionParser, IFilterOptionParser
{
    public virtual FilterClause Parse(QueryToken filter, QueryParserContext context)
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
}
