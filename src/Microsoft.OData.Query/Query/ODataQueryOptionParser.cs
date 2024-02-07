//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query;

public class ODataQueryOptionParser : IODataQueryOptionParser
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ODataQueryOptionParser" /> class.
    /// </summary>
    /// <param name="serviceProvider">The required tokenizer.</param>
    public ODataQueryOptionParser(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query">The odata query string, it should be escaped query string.</param>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual ValueTask<ODataQueryOption> ParseQueryAsync(string query, QueryOptionParserContext context)
    {
        IDictionary<string, ReadOnlyMemory<char>> queryOptionsDict = QueryStringHelpers.SplitQuery(query);

        ODataQueryOption queryOption = new ODataQueryOption();

        // We need parse the 'apply', 'compute' first.
        // $apply
        if (queryOptionsDict.TryGetQueryOption(QueryStringConstants.Apply, context, out ReadOnlyMemory<char> apply))
        {
            queryOption.Apply = ParseApply(apply, context);
        }

        // $compute
        if (queryOptionsDict.TryGetQueryOption(QueryStringConstants.Compute, context, out ReadOnlyMemory<char> compute))
        {
            queryOption.Compute = ParseCompute(compute, context);
        }

        // $filter
        if (queryOptionsDict.TryGetQueryOption(QueryStringConstants.Filter, context, out ReadOnlyMemory<char> filter))
        {
            queryOption.Filter = ParseFilter(filter, context);
        }

        // $orderBy
        if (queryOptionsDict.TryGetQueryOption(QueryStringConstants.OrderBy, context, out ReadOnlyMemory<char> orderBy))
        {
            queryOption.OrderBy = ParseOrderBy(orderBy, context);
        }

        return ValueTask.FromResult(queryOption);
    }

    protected virtual ApplyClause ParseApply(ReadOnlyMemory<char> apply, QueryOptionParserContext context)
    {
        // Need add the aggregate properties into context
        throw new NotImplementedException();
    }

    protected virtual ComputeClause ParseCompute(ReadOnlyMemory<char> compute, QueryOptionParserContext context)
    {
        // Need add the aggregate properties into context
        throw new NotImplementedException();
    }

    protected virtual SearchClause ParseSearch(ReadOnlyMemory<char> search, QueryOptionParserContext context)
    {
        throw new NotImplementedException();
    }

    protected virtual FilterClause ParseFilter(ReadOnlyMemory<char> filter, QueryOptionParserContext context)
    {
        throw new NotImplementedException();
    }

    protected virtual OrderByClause ParseOrderBy(ReadOnlyMemory<char> orderBy, QueryOptionParserContext context)
    {
        IOrderByOptionTokenizer tokenizer = _serviceProvider?.GetService<IOrderByOptionTokenizer>()
            ?? new OrderByOptionTokenizer(ExpressionLexerFactory.Default);

        OrderByToken token = tokenizer.Tokenize(orderBy.Span.ToString(), context.TokenizerContext);

        IOrderByOptionParser parser = _serviceProvider?.GetService<IOrderByOptionParser>()
            ?? new OrderByOptionParser();

        return parser.ParseOrderBy(token, context);
    }
}
