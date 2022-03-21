//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query
{
    /// <summary>
    /// Constant values related to the URI query syntax.
    /// </summary>
    public static partial class UseQueryExtensions
    {
        /// <summary>
        /// Adds a tokenization middleware.
        /// </summary>
        /// <param name="builder">The <see cref="IODataQueryBuilder"/> instance.</param>
        /// <returns>The <see cref="IODataQueryBuilder"/> instance.</returns>
        public static IODataQueryBuilder UseTokenizor(this IODataQueryBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseQuery<TokenizeMiddleware>();
        }

        /// <summary>
        /// Adds a $filter middleware.
        /// </summary>
        /// <param name="builder">The <see cref="IODataQueryBuilder"/> instance.</param>
        /// <returns>The <see cref="IODataQueryBuilder"/> instance.</returns>
        public static IODataQueryBuilder UseFilter(this IODataQueryBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseQuery<ODataFilterMiddleware>();
        }

        /// <summary>
        /// Adds a $select middleware.
        /// </summary>
        /// <param name="builder">The <see cref="IODataQueryBuilder"/> instance.</param>
        /// <returns>The <see cref="IODataQueryBuilder"/> instance.</returns>
        public static IODataQueryBuilder UseSelect(this IODataQueryBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseQuery<ODataSelectMiddleware>();
        }

        /// <summary>
        /// Adds a $search middleware.
        /// </summary>
        /// <param name="builder">The <see cref="IODataQueryBuilder"/> instance.</param>
        /// <returns>The <see cref="IODataQueryBuilder"/> instance.</returns>
        public static IODataQueryBuilder UseSearch(this IODataQueryBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseQuery<ODataSearchMiddleware>();
        }
    }
}
