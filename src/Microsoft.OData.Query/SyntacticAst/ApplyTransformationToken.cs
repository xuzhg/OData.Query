//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Base class for Apply transformation tokens
/// </summary>
public abstract class ApplyTransformationToken : IQueryToken
{
    /// <summary>
    /// Gets the kind of this token.
    /// </summary>
    public virtual QueryTokenKind Kind { get; }
}
