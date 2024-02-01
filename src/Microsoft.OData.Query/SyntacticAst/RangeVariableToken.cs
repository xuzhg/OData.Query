//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing the parameter for an Any/All query.
/// </summary>
public sealed class RangeVariableToken : QueryToken
{
    /// <summary>
    /// Create a new RangeVariableToken
    /// </summary>
    /// <param name="name">The name of the visitor for the Any/All query.</param>
    public RangeVariableToken(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.RangeVariable;

    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name { get; }

    /// <summary>Indicates the Equals overload.</summary>
    /// <returns>True if equal.</returns>
    /// <param name="obj">The other RangeVariableToken.</param>
    public override bool Equals(object obj)
    {
        var otherPath = obj as RangeVariableToken;
        if (otherPath == null)
        {
            return false;
        }

        return Name.Equals(otherPath.Name, StringComparison.Ordinal);
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}