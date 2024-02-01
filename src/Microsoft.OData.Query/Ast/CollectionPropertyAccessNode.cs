//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------


using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Ast;

public class CollectionPropertyAccessNode : CollectionValueNode
{
    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.CollectionValuePropertyAccess;
}
