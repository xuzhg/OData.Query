//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

public class RangeVariableReferenceNode : SingleValueNode
{
    public RangeVariableReferenceNode(RangeVariable variable)
    {
        Variable = variable;
    }

    public RangeVariable Variable { get; }

    public string Name => Variable.Name;

    public override QueryNodeKind Kind => QueryNodeKind.ResourceRangeVariableReference;

    public override Type NodeType => Variable.Type;
}
