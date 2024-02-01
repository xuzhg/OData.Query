//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Base node representing an Any/All query.
/// </summary>
public abstract class LambdaNode : SingleValueNode
{
    /// <summary>
    /// The collection of rangeVariables in scope for this lambda node.
    /// </summary>
    private readonly Collection<RangeVariable> _rangeVariables;

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaNode" /> class.
    /// </summary>
    /// <param name="source">The source of this node.</param>
    /// <param name="rangeVariables">The collection of rangeVariables in scope for this lambda node.</param>
    /// <param name="currentRangeVariable">The newest range variable added for by this lambda node.</param>
    /// <param name="body">The associated boolean expression.</param>
    protected LambdaNode(CollectionValueNode source, Collection<RangeVariable> rangeVariables, RangeVariable currentRangeVariable, SingleValueNode body)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        _rangeVariables = rangeVariables ?? throw new ArgumentNullException(nameof(rangeVariables));
        CurrentRangeVariable = currentRangeVariable ?? throw new ArgumentNullException(nameof(currentRangeVariable));
        Body = body ?? throw new ArgumentNullException(nameof(body));
    }

    /// <summary>
    /// Gets the collection of rangeVariables in scope for this lambda node.
    /// </summary>
    public IEnumerable<RangeVariable> RangeVariables => _rangeVariables;

    /// <summary>
    /// Gets the newest range variable added for by this lambda node.
    /// </summary>
    public RangeVariable CurrentRangeVariable { get; }

    /// <summary>
    /// Gets the associated boolean expression.
    /// </summary>
    public SingleValueNode Body { get; }

    /// <summary>
    /// Gets the source of this node.
    /// </summary>
    public CollectionValueNode Source { get; }
}
