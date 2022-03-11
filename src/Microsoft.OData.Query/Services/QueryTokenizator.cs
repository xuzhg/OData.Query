//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Query.Tokens;

namespace Microsoft.OData.Query.Services
{
    /// <summary>
    /// A default implementation of <see cref="IQueryTokenizator"/>
    /// </summary>
    public class QueryTokenizator : IQueryTokenizator
    {
        /// <summary>
        /// Tokenize the raw query string into tokens.
        /// The raw query string should be encoded, which means the char escaped.
        /// </summary>
        /// <param name="rawQuery">The encoded raw query string.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws if raw query is null or empty string.</exception>
        public virtual QueryToken[] Tokenize(string rawQuery, QueryTokenizationContext context)
        {
            if (string.IsNullOrEmpty(rawQuery))
            {
                throw new ArgumentNullException(nameof(rawQuery));
            }

            IList<QueryToken> tokens = new List<QueryToken>();
            (var querys, var others) = ParseAndSplitRawQuery(rawQuery, context);

            // Processing OData built-in query
            foreach (var query in querys)
            {
                QueryToken token = TokenizeODataToken(query, context);

                if (token != null)
                {
                    tokens.Add(token);
                }
            }

            // Process non-OData built-in query
            foreach (var other in others)
            {
                tokens.Add(TokenizeCustomized(other, context));
            }

            return tokens.ToArray();
        }

        private QueryToken TokenizeODataToken(KeyValuePair<string, string> query, QueryTokenizationContext context)
        {
            return query.Key switch
            {
                "$apply" => TokenizeApply(query.Value, context),

                "$compute" => TokenizeCompute(query.Value, context),

                "$count" => TokenizeCount(query.Value, context),

                "$deltatoken" => TokenizeDeltaToken(query.Value, context),

                "$expand" => TokenizeExpand(query.Value, context),

                "$filter" => TokenizeFilter(query.Value, context),

                "$format" => TokenizeFormat(query.Value, context),

                "$id" => TokenizeId(query.Value, context),

                "$orderby" => TokenizeOrderby(query.Value, context),

                "$select" => TokenizeSelect(query.Value, context),

                "$skip" => TokenizeSkip(query.Value, context),

                "$skiptoken" => TokenizeSkipToken(query.Value, context),

                "$top" => TokenizeTop(query.Value, context),

                "$search" => TokenizeSearch(query.Value, context),

                "$index" => TokenizeIndex(query.Value, context),

                _ => null
            };
        }

        protected virtual QueryToken TokenizeApply(string apply, QueryTokenizationContext context)
        {
            return null;
        }

        protected virtual QueryToken TokenizeCompute(string compute, QueryTokenizationContext context)
        {
            return null;
        }

        protected virtual QueryToken TokenizeCount(string apply, QueryTokenizationContext context)
        {
            return null;
        }

        protected virtual QueryToken TokenizeDeltaToken(string deltaToken, QueryTokenizationContext context)
        {
            return null;
        }

        protected virtual QueryToken TokenizeExpand(string expand, QueryTokenizationContext context)
        {
            return null;
        }

        protected virtual QueryToken TokenizeFilter(string filter, QueryTokenizationContext context)
        {
            return null;
        }

        protected virtual QueryToken TokenizeFormat(string format, QueryTokenizationContext context)
        {
            return null;
        }

        protected virtual QueryToken TokenizeId(string id, QueryTokenizationContext context)
        {
            return null;
        }

        protected virtual QueryToken TokenizeOrderby(string orderby, QueryTokenizationContext context)
        {
            return null;
        }
        protected virtual QueryToken TokenizeSelect(string select, QueryTokenizationContext context)
        {
            return null;
        }

        /// <summary>
        /// Parses a $skip query option
        /// </summary>
        /// <param name="skip">The skip from the query.</param>
        /// <param name="context">The context.</param>
        /// <returns>A skip token.</returns>
        /// <exception cref="OException">Throws if the input is not a valid $skip value.</exception>
        protected virtual SkipToken TokenizeSkip(string skip, QueryTokenizationContext context)
        {
            long skipValue;
            if (!long.TryParse(skip, out skipValue) || skipValue < 0)
            {
                throw new OException(Error.Format(OResource.QueryTokenizator_InvalidSkipQueryOptionValue, skip));
            }

            return new SkipToken(skipValue);
        }

        protected virtual QueryToken TokenizeSkipToken(string skiptoken, QueryTokenizationContext context)
        {
            return null;
        }

        /// <summary>
        /// Parses a $top query option
        /// </summary>
        /// <param name="top">The topQuery from the query.</param>
        /// <param name="context">The context.</param>
        /// <returns>A top token.</returns>
        /// <exception cref="OException">Throws if the input is not a valid $top value.</exception>
        protected virtual TopToken TokenizeTop(string top, QueryTokenizationContext context)
        {
            long topValue;
            if (!long.TryParse(top, out topValue) || topValue < 0)
            {
                throw new OException(Error.Format(OResource.QueryTokenizator_InvalidTopQueryOptionValue, top));
            }

            return new TopToken(topValue);
        }

        /// <summary>
        /// Parses a $search query option
        /// </summary>
        /// <param name="search">The search from the query.</param>
        /// <param name="context">The context.</param>
        /// <returns>A search token.</returns>
        protected virtual SearchToken TokenizeSearch(string search, QueryTokenizationContext context)
        {
            return null;
        }

        /// <summary>
        /// Parses a $index query option
        /// </summary>
        /// <param name="index">The value of $index from the query</param>
        /// <param name="context">The context.</param>
        /// <returns>An index token.</returns>
        /// <exception cref="OException">Throws if the input value is not a valid $index value.</exception>
        protected virtual IndexToken TokenizeIndex(string index, QueryTokenizationContext context)
        {
            long indexValue;
            if (!long.TryParse(index, out indexValue))
            {
                throw new OException(Error.Format(OResource.QueryTokenizator_InvalidIndexQueryOptionValue, index));
            }

            return new IndexToken(indexValue);
        }

        protected virtual QueryToken TokenizeCustomized(KeyValuePair<string, string> query, QueryTokenizationContext context)
            => new CustomQueryToken(query.Key, query.Value);

        /// <summary>
        /// Parse and split the raw query string into two parts, one is for OData built-in, the other is for cusomized.
        /// </summary>
        /// <param name="rawQuery">The raw query string.</param>
        /// <param name="context">The tokenization context</param>
        /// <returns>the OData built-in query and the others.</returns>
        internal static (IDictionary<string, string>, IList<KeyValuePair<string, string>>) ParseAndSplitRawQuery(string rawQuery, QueryTokenizationContext context)
        {
            IDictionary<string, string> queryOptionDic = new Dictionary<string, string>(StringComparer.Ordinal);
            IList<KeyValuePair<string, string>> others = new List<KeyValuePair<string, string>>();

            IList<KeyValuePair<string, string>> querys = QueryStringHelper.ParseRawQuery(rawQuery);
            if (querys != null)
            {
                foreach (var query in querys)
                {
                    // query.Key should never be null
                    Debug.Assert(query.Key != null);

                    bool shouldAddDollarPrefix = context.EnableNoDollarQueryOptions && !query.Key.StartsWith('$');
                    string adjustedName = shouldAddDollarPrefix ? $"${query.Key}" : query.Key;
                    adjustedName = context.EnableCaseInsensitive ? adjustedName.ToLowerInvariant() : adjustedName;

                    if (adjustedName.IsODataQueryOption())
                    {
                        if (queryOptionDic.ContainsKey(adjustedName))
                        {
                            string errorMessage = context.EnableNoDollarQueryOptions ?
                                $"{adjustedName}/{adjustedName.Substring(1)}" : // make a string like "$filter/filter"
                                adjustedName;

                            throw new OException(Error.Format(OResource.QueryTokenizator_QueryOptionMustBeSpecifiedOnce, errorMessage));
                        }

                        queryOptionDic.Add(adjustedName, query.Value);
                    }
                    else
                    {
                        others.Add(query);
                    }
                }
            }

            return (queryOptionDic, others);
        }
    }
}
