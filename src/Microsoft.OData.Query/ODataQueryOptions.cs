//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.Tokens;

namespace Microsoft.OData.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class ODataQueryOptions
    {
        public virtual IQueryable ApplyTo(IQueryable query, string rawQuery, ODataQueryOptions options = null)
        {
            // Lexer the raw string and generate the QueryToken
            QueryToken token = Tokenize(rawQuery, null);

            // Bind the query token with the metadata (type, etc) to generate the QueryNode
            QueryNode node = Bind(token);

            // $apply
            IQueryable result = BindApply(query, node);

            // $filter
            result = BindFilter(result, node);

            // $search
            result = BindSearch(result, node);

            // $count ?

            // $orderby
            result = BindOrderby(result, node);

            // $skiptoken
            result = BindSkipToken(result, node);

            // $deltatoken?

            // $select and $expand
            result = BindSelectExpand(result, node);

            // $skip
            result = BindSkip(result, node);

            // $top
            result = BindTop(result, node);

            // paging
            result = BindPaging(result);

            return result;
        }

        protected virtual QueryToken Tokenize(string rawQuery, TokenizationSettings settings)
        {
            return null;
        }

        protected virtual QueryNode Bind(QueryToken token)
        {
            return null;
        }

        protected virtual IQueryable BindApply(IQueryable query, QueryNode node)
        {
            return null;
        }

        protected virtual IQueryable BindFilter(IQueryable query, QueryNode node)
        {
            return null;
        }

        protected virtual IQueryable BindSearch(IQueryable query, QueryNode node)
        {
            return null;
        }

        protected virtual IQueryable BindOrderby(IQueryable query, QueryNode node)
        {
            return null;
        }

        protected virtual IQueryable BindSkipToken(IQueryable query, QueryNode node)
        {
            return null;
        }

        protected virtual IQueryable BindSelectExpand(IQueryable query, QueryNode node)
        {
            return null;
        }

        protected virtual IQueryable BindSkip(IQueryable query, QueryNode node)
        {
            return null;
        }

        protected virtual IQueryable BindTop(IQueryable query, QueryNode node)
        {
            return null;
        }

        protected virtual IQueryable BindPaging(IQueryable query)
        {
            return null;
        }
    }
}
