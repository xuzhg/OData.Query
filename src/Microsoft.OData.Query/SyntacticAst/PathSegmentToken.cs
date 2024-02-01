//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing a segment in a path.
/// </summary>
public abstract class PathSegmentToken
{
    /// <summary>
    /// build this segment token using the next token
    /// </summary>
    /// <param name="nextToken">the next token in the path</param>
    protected PathSegmentToken(PathSegmentToken nextToken)
    {
        NextToken = nextToken;
    }

    /// <summary>
    /// Get the NextToken in the path
    /// </summary>
    public PathSegmentToken NextToken { get; }

    /// <summary>
    /// The name of the property to access.
    /// </summary>
    public abstract string Identifier { get; }

    /// <summary>
    /// Is this a structural property
    /// </summary>
    public bool IsStructuralProperty { get; set; }

    /// <summary>
    /// Is this token namespace or container qualified.
    /// </summary>
    /// <returns>true if this token is namespace or container qualified.</returns>
    public abstract bool IsNamespaceOrContainerQualified();
}