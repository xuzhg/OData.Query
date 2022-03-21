//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query
{

    /// <summary>
    /// Constant values related to the URI query syntax.
    /// </summary>
    public class ODataSearchMiddleware
    {
        private readonly QueryDelegate _next;

        public ODataSearchMiddleware(QueryDelegate next)
        {
            _next = next;
        }

        public async Task ApplyAsync(QueryContext context)
        {
            // do something for $select before move to next middleware



            await _next(context).ConfigureAwait(false);
        }
    }
}
