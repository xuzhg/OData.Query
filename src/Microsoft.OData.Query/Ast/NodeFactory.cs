//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Ast;

public class NodeFactory
{
    /// <summary>
    /// Creates a ParameterQueryNode for an implicit parameter ($it).
    /// </summary>
    /// <returns>A new IParameterNode.</returns>
    internal static RangeVariable CreateImplicitRangeVariable(Type elementType)
    {
        return new RangeVariable(TokenConstants.It, elementType);
        //{
        //    Name = TokenConstants.It,
        //    Type = elementType
        //};
    }
}