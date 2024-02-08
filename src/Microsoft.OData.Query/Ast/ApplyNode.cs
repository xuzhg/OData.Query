//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Ast;

public class ApplyNode : QueryNode
{
    public override QueryNodeKind Kind => throw new NotImplementedException();

    public override Type NodeType => throw new NotImplementedException();
}

public class ApplyClause
{
    /// <summary>
    /// Create a ApplyClause.
    /// </summary>
    /// <param name="transformations">The <see cref="TransformationNode"/>s.</param>
    public ApplyClause(IList<TransformationNode> transformations)
    {
        Transformations = transformations;
    }

    /// <summary>
    /// The collection of transformations to perform.
    /// </summary>
    public IEnumerable<TransformationNode> Transformations { get; }
}