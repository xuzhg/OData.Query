//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Linq.Expressions;
using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Parser;

namespace Microsoft.OData.Query.Binders;

public class FilterBinder : IFilterBinder
{
    public FilterBinder(IFilterOptionParser parser, IQueryNodeBinder nodeBinder)
    {
        Parser = parser;
        NodeBinder = nodeBinder;
    }

    public IFilterOptionParser Parser { get; }

    public IQueryNodeBinder NodeBinder { get; }

    public virtual Expression BindFilter(string filter, BinderContext context)
    {
        // parse $filter=Name eq 'John' to Abstract search tree.
        //FilterClause filterClause = Parser.ParseFilter(filter, new QueryOptionParserContext());

        //// Generate the Linq Expression
        //Expression body = NodeBinder.Bind(filterClause.Expression, context);

        //ParameterExpression filterParameter = context.CurrentParameter;

        //// TODO: nullable
        //LambdaExpression filterExpr = Expression.Lambda(body, filterParameter);

        //return filterExpr;
        throw new NotImplementedException();
    }
}
