//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Linq.Expressions;

namespace Microsoft.OData.Query
{
    public delegate Task QueryDelegate(QueryContext context);

    public class QueryContext
    {
        /// <summary>
        /// Gets or sets the <see cref="IServiceProvider"/> that provides access to the request's service container.
        /// </summary>
        public IServiceProvider ContextServices { get; set; }


        public Expression Expression { get; set; }
    }

    /// <summary>
    /// Constant values related to the URI query syntax.
    /// </summary>
    public interface IODataQueryBuilder
    {
        /// <summary>
        /// Gets or sets the <see cref="IServiceProvider"/> that provides access to the application's service container.
        /// </summary>
        IServiceProvider ApplicationServices { get; set; }

        IODataQueryBuilder Use(Func<QueryDelegate, QueryDelegate> middleware);

        QueryDelegate Build();
    }
}
