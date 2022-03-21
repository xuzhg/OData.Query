//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query
{
    /// <summary>
    /// Constant values related to the URI query syntax.
    /// </summary>
    public static partial class QueryBinder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="rawQuery"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IQueryable BindTo(IQueryable query, string rawQuery, QueryBinderOptions options = null)
        {
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="rawQuery"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IQueryable BindTo<T>(IQueryable<T> query, string rawQuery, QueryBinderOptions options = null)
        {
            return query;
        }
    }
}
