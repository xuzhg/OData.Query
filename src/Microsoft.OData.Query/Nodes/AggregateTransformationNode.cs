//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Enumeration of the possible types of aggregations.
/// </summary>
public enum AggregateExpressionKind
{
    /// <summary>Value used to treat non recognized aggregations.</summary>
    None = 0,

    /// <summary>Aggregation of a single value property.</summary>
    PropertyAggregate = 1,

    /// <summary>Aggregation of a entity set property.</summary>
    EntitySetAggregate = 2
}

/// <summary>
/// A aggregate expression representing a aggregation transformation.
/// </summary>
public abstract class AggregateExpressionBase
{
    /// <summary>Base constructor for concrete subclasses use for convenience.</summary>
    /// <param name="kind">The <see cref="AggregateExpressionKind"/> of the expression.</param>
    /// <param name="alias">Alias of the resulting aggregated value.</param>
    protected AggregateExpressionBase(AggregateExpressionKind kind, string alias)
    {
        AggregateKind = kind;
        Alias = alias;
    }

    /// <summary>Returns the <see cref="AggregateExpressionKind"/> of the expression.</summary>
    public AggregateExpressionKind AggregateKind { get; private set; }

    /// <summary>Returns the alias of the expression.</summary>
    public string Alias { get; private set; }
}

/// <summary>
/// Node representing a aggregate transformation.
/// </summary>
public sealed class AggregateTransformationNode : TransformationNode
{
    /// <summary>
    /// Create a AggregateTransformationNode.
    /// </summary>
    /// <param name="expressions">A list of <see cref="AggregateExpressionBase"/>.</param>
    public AggregateTransformationNode(IEnumerable<AggregateExpressionBase> expressions)
    {
        //ExceptionUtils.CheckArgumentNotNull(expressions, "expressions");

        AggregateExpressions = expressions;
    }

    /// <summary>
    /// Property that returns a list of all <see cref="AggregateExpressionBase"/>s of this transformation node.
    /// </summary>
    public IEnumerable<AggregateExpressionBase> AggregateExpressions { get; }

    /// <summary>
    /// Gets the kind of the transformation node.
    /// </summary>
    public override TransformationNodeKind TransformKind => TransformationNodeKind.Aggregate;

    public override QueryNodeKind Kind => throw new NotImplementedException();

   // public override Type NodeType => throw new NotImplementedException();
}
