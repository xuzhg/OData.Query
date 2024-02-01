//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Query token representing a GroupBy token.
/// </summary>
public sealed class GroupByToken : ApplyTransformationToken
{
    private readonly IEnumerable<EndPathToken> properties;

    private readonly ApplyTransformationToken child;

    /// <summary>
    /// Create a GroupByToken.
    /// </summary>
    /// <param name="properties">The list of group by properties.</param>
    /// <param name="child">The child of this token.</param>
    public GroupByToken(IEnumerable<EndPathToken> properties, ApplyTransformationToken child)
    {
        //ExceptionUtils.CheckArgumentNotNull(properties, "properties");

        this.properties = properties;
        this.child = child;
    }

    /// <summary>
    /// Gets the kind of this token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.AggregateGroupBy;

    /// <summary>
    /// Gets the list of group by properties.
    /// </summary>
    public IEnumerable<EndPathToken> Properties
    {
        get { return this.properties; }
    }

    /// <summary>
    /// Gets the child of this token.
    /// </summary>
    public ApplyTransformationToken Child
    {
        get { return this.child; }
    }
}
