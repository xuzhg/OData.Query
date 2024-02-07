//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// A RangeVariable, which represents an iterator variable either over a collection, either of entities or not.
/// Exists outside of the main SemanticAST, but hooked in via a RangeVariableReferenceNode (either Non-Entity or Entity).
/// </summary>
public class RangeVariable
{
    public RangeVariable(string name, Type type)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    /// <summary>
    /// Gets the name of the associated rangeVariable.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of entity referenced by this rangeVariable
    /// </summary>
    public Type Type { get; }
}