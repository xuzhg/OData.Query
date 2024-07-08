//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query;




public class ODataQueryOption
{
    public ApplyClause Apply { get; set; }

    public FilterClause Filter { get; set; }

    public SearchClause Search { get; set; }

    public SelectClause Select { get; set; }

    public ExpandClause Expand { get; set; }

    public ComputeClause Compute { get; set; }

    public OrderByClause OrderBy { get; set; }

    public bool? Count { get; set; }

    // public int? Top { get; set; }

    public ConstantNode<long> Top { get; set; }

    public int? Skip { get; set; }
    public int? Index { get; set; }

    public string SkipToken { get; set; }
    public string DeltaToken { get; set; }

    public QueryNode Customized { get; set; }
}