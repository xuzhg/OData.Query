//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Services
{
    /// <summary>
    /// A context for query tokenization.
    /// </summary>
    public class QueryTokenizationContext
    {
        public bool EnableCaseInsensitive { get; set; } = true;

        public bool EnableNoDollarQueryOptions { get; set; } = true;
    }
}
