//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing a segment in a path.
/// </summary>
///
public abstract class PathToken : IQueryToken
{
    public abstract QueryTokenKind Kind { get; }

    /// <summary>
    /// The NextToken in the path(can either be the parent or the child depending on whether the tree has
    /// been normalized for expand or not.
    /// TODO: need to revisit this and the rest of the syntactic parser to make it ready for public consumption.
    /// </summary>
    public abstract IQueryToken NextToken { get; set; }

    /// <summary>
    /// The name of the property to access.
    /// </summary>
    public abstract string Identifier { get; }

    /// <summary>Indicates the Equals overload.</summary>
    /// <returns>True if equal.</returns>
    /// <param name="obj">The other PathToken.</param>
    public override bool Equals(object obj)
    {
        var otherPath = obj as PathToken;
        if (otherPath == null)
        {
            return false;
        }

        return this.Identifier.Equals(otherPath.Identifier, System.StringComparison.Ordinal)
            && (this.NextToken == null && otherPath.NextToken == null
                || this.NextToken.Equals(otherPath.NextToken));
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        int identifierHashCode = this.Identifier.GetHashCode();
        if (this.NextToken != null)
        {
            identifierHashCode = Combine(identifierHashCode, this.NextToken.GetHashCode());
        }

        return identifierHashCode;
    }

    private static int Combine(int h1, int h2)
    {
        // RyuJIT optimizes this to use the ROL instruction
        // Related GitHub pull request: dotnet/coreclr#1830
        // Based on https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Numerics/Hashing/HashHelpers.cs
        uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
        return ((int)rol5 + h1) ^ h2;
    }
}

/// <summary>
/// Lexical token representing a System token such as $count
/// </summary>
public class SegmentToken : IQueryToken
{
    /// <summary>
    /// Build a new System Token
    /// </summary>
    /// <param name="identifier">the identifier for this token.</param>
    /// <param name="next">the next token in the path</param>
    public SegmentToken(string identifier/*, SegmentToken next*/)
    {
        // ExceptionUtils.CheckArgumentNotNull(identifier, "identifier");
        Identifier = identifier;
       // Next = next;
    }

    /// <summary>
    /// Get/set the NextToken in the path
    /// </summary>
    public SegmentToken Next { get; set; }

    /// <summary>
    /// Get the identifier for this token
    /// </summary>
    public string Identifier { get; }

    public QueryTokenKind Kind => QueryTokenKind.PathSegment;

    /// <summary>
    /// Is this token namespace or container qualified.
    /// </summary>
    /// <returns>always false, since this is a system token.</returns>
    //public override bool IsNamespaceOrContainerQualified()
    //{
    //    return false;
    //}
}