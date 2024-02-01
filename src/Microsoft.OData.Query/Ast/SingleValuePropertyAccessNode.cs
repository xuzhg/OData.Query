//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Ast;

public class SingleValuePropertyAccessNode : SingleValueNode
{
    public SingleValueNode Source { get; set; }

    public string Name { get; set; }

    public override Nodes.QueryNodeKind Kind => throw new NotImplementedException();
}
