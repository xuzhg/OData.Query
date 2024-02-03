//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query;

public class ODataQueryOptions<T> : ODataQueryOptions
{
    public ODataQueryOptions(string query)
        : base(typeof(T), query)
    {
    }
}

public class ODataQueryOptions
{
    public ODataQueryOptions(Type elementType, string query)
    {
        
    }
}

public class OrderByQueryOption
{
    public OrderByQueryOption(Type elementType, string rawOrderBy)
    {
         
    }
}
