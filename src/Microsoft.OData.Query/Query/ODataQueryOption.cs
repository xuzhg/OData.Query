//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;

namespace Microsoft.OData.Query;

public class ODataQueryOption
{
    public ApplyClause Apply { get; set; }

    public FilterClause Filter { get; set; }

    public SearchClause Search { get; set; }

    public SelectClause Select { get; set; }

    public ExpandClause Expand { get; set; }

    public OrderByClause OrderBy { get; set; }
}