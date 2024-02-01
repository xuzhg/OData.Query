//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing an expand operation.
/// </summary>
public sealed class ExpandToken : QueryToken
{
    /// <summary>
    /// The properties according to which to expand in the results.
    /// </summary>
    private readonly IEnumerable<ExpandItemToken> expandItems;

    /// <summary>
    /// Creates a new instance of <see cref="ExpandToken"/> given the property-accesses of the expand query.
    /// </summary>
    /// <param name="expandTerms">The properties according to which to expand the results.</param>
    public ExpandToken(params ExpandItemToken[] expandTerms)
        : this((IEnumerable<ExpandItemToken>)expandTerms)
    {
    }

    /// <summary>
    /// Create a new instance of <see cref="ExpandToken"/> given the property-accesses of the expand query.
    /// </summary>
    /// <param name="expandTerms">The properties according to which to expand the results.</param>
    public ExpandToken(IEnumerable<ExpandItemToken> expandTerms)
    {
      //  this.expandItems = new ReadOnlyEnumerableForUriParser<ExpandTermToken>(expandTerms ?? Enumerable.Empty<ExpandTermToken>());
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.Expand;

    /// <summary>
    /// The properties according to which to expand in the results.
    /// </summary>
    public IEnumerable<ExpandItemToken> ExpandItems
    {
        get { return this.expandItems; }
    }
}