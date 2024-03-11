//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Represents all lexical tokens of OData query.
/// </summary>
public interface IQueryToken
{
    /// <summary>
    /// The kind of the query token.
    /// </summary>
    QueryTokenKind Kind { get; }
}

public class TopToken : IQueryToken
{
    public TopToken(long top)
    {
        Top = top;
    }

    public long Top { get; }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public QueryTokenKind Kind => QueryTokenKind.Top;
}

public class SkipTokenToken : IQueryToken
{
    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public QueryTokenKind Kind => QueryTokenKind.SkipToken;
}

public class DeltaTokenToken : IQueryToken
{
    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public QueryTokenKind Kind => QueryTokenKind.DeltaToken;
}

public class QueryTree
{
    public IQueryToken Filter { get; set; }

    public OrderByToken OrderBy { get; set; }

    public SelectToken Select { get; set; }

    public ExpandToken Expand { get; set; }

    public ComputeToken Compute { get; set; }

    public ApplyToken Apply { get; set; }

    public IQueryToken Search { get; set; }

    public SkipTokenToken SkipToken { get; set; }

    public DeltaTokenToken DeltaToken { get; set; }

    public long? Index { get; set; }

    public bool? Count { get; set; }

    public long? Top { get; set; }
    public long? Skip { get; set; }
}
