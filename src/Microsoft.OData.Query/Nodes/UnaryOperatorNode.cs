﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Nodes;

public class UnaryOperatorNode : SingleValueNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnaryOperatorNode" /> class.
    /// </summary>
    /// <param name="operatorKind"></param>
    /// <param name="operand"></param>
    public UnaryOperatorNode(UnaryOperatorKind operatorKind, SingleValueNode operand)
    {
        OperatorKind = operatorKind;
        Operand = operand;
    }

    /// <summary>
    /// Gets the operator kind.
    /// </summary>
    public UnaryOperatorKind OperatorKind { get; }

    /// <summary>
    /// Gets the operand of the unary operator.
    /// </summary>
    public SingleValueNode Operand { get; }

    /// <summary>
    /// Gets the kind of the query node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.UnaryOperator;

    public override Type NodeType => throw new NotImplementedException();
}
