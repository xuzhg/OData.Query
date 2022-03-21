//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query
{
    /// <summary>
    /// Default implementation for <see cref="IODataQueryBuilder"/>.
    /// </summary>
    public class ODataQueryBuilder : IODataQueryBuilder
    {
        private readonly List<Func<QueryDelegate, QueryDelegate>> _components = new();

        public ODataQueryBuilder(IServiceProvider serviceProvider)
        {
            ApplicationServices = serviceProvider;
        }

        /// <summary>
        /// Gets or sets the <see cref="IServiceProvider"/> that provides access to the application's service container.
        /// </summary>
        public IServiceProvider ApplicationServices { get; set; }

        public IODataQueryBuilder Use(Func<QueryDelegate, QueryDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public QueryDelegate Build()
        {
            QueryDelegate app = context =>
            {
                // If we reach the end of the pipeline, but we have a result, then something unexpected has happened.
                if (context.Expression == null)
                {
                    var message = $"The query reached the end of the pipeline without set a Linq.Expression.";
                    throw new InvalidOperationException(message);
                }

                return Task.CompletedTask;
            };

            for (var c = _components.Count - 1; c >= 0; c--)
            {
                app = _components[c](app);
            }

            return app;
        }
    }
}
