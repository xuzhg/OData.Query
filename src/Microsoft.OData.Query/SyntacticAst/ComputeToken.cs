//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Query token representing a Compute token.
/// </summary>
public sealed class ComputeToken : ApplyTransformationToken
{
    private readonly IEnumerable<ComputeItemToken> _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComputeToken" /> class.
    /// </summary>
    /// <param name="items">The list of ComputeItemToken.</param>
    public ComputeToken(IEnumerable<ComputeItemToken> items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
    }

    /// <summary>
    /// Gets the kind of this token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.Compute;

    /// <summary>
    /// Gets the list of ComputeItemToken.
    /// </summary>
    public IEnumerable<ComputeItemToken> Items => _items;
}
