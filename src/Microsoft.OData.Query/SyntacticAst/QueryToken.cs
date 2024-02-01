//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Base class for all lexical tokens of OData query.
/// </summary>
public abstract class QueryToken
{
    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public abstract QueryTokenKind Kind { get; }
}

public class SkipTokenToken : QueryToken
{
    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.SkipToken;
}

public class DeltaTokenToken : QueryToken
{
    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.DeltaToken;
}

public class QueryTree
{
    public QueryToken Filter { get; set; }

    public OrderByToken OrderBy { get; set; }

    public SelectToken Select { get; set; }

    public ExpandToken Expand { get; set; }

    public ComputeToken Compute { get; set; }

    public ApplyToken Apply { get; set; }

    public QueryToken Search { get; set; }

    public SkipTokenToken SkipToken { get; set; }

    public DeltaTokenToken DeltaToken { get; set; }

    public long? Index { get; set; }

    public bool? Count { get; set; }

    public long? Top { get; set; }
    public long? Skip { get; set; }
}
