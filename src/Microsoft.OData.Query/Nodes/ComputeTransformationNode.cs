//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Node representing a compute expression.
/// </summary>
public sealed class ComputeTransformationNode : TransformationNode
{
    private readonly IEnumerable<ComputeExpression> expressions;

    /// <summary>
    /// Create a ComputeTransformationNode.
    /// </summary>
    /// <param name="expressions">A list of <see cref="ComputeExpression"/>.</param>
    public ComputeTransformationNode(IEnumerable<ComputeExpression> expressions)
    {
      //  ExceptionUtils.CheckArgumentNotNull(expressions, "expressions");

        this.expressions = expressions;
    }

    /// <summary>
    /// Gets the list of <see cref="ComputeExpression"/>.
    /// </summary>
    public IEnumerable<ComputeExpression> Expressions
    {
        get
        {
            return expressions;
        }
    }

    /// <summary>
    /// Gets the kind of the transformation node.
    /// </summary>
    public override TransformationNodeKind TransformKind => TransformationNodeKind.Compute;

    public override Type NodeType => throw new NotImplementedException();

    public override QueryNodeKind Kind => throw new NotImplementedException();
}
