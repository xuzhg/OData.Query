//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing a select operation.
/// </summary>
public sealed class SelectToken : QueryToken
{
    /// <summary>
    /// The properties according to which to select in the results.
    /// </summary>
    private readonly IEnumerable<SelectItemToken> selectTerms;

    /// <summary>
    /// Create a <see cref="SelectToken"/> given the property-accesses of the select query.
    /// </summary>
    /// <param name="properties">The properties according to which to select the results.</param>
    public SelectToken(IEnumerable<PathSegmentToken> properties)
        : this(properties == null ? null : properties.Select(e => new SelectItemToken(e)))
    {
    }

    /// <summary>
    /// Create a <see cref="SelectToken"/> given the property-accesses of the select query.
    /// </summary>
    /// <param name="selectTerms">The select term tokes according to which to select the results.</param>
    public SelectToken(IEnumerable<SelectItemToken> selectTerms)
    {
        //this.selectTerms = selectTerms != null ?
        //    new ReadOnlyEnumerableForUriParser<SelectTermToken>(selectTerms) :
        //    new ReadOnlyEnumerableForUriParser<SelectTermToken>(Enumerable.Empty<SelectItemmToken>());
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.Select;

    /// <summary>
    /// The properties according to which to select the results.
    /// </summary>
    public IEnumerable<PathSegmentToken> Properties
    {
        get;
        //get
        //{
        //    return this.selectTerms.Select(e => e.PathToProperty);
        //}
    }

    /// <summary>
    /// The properties according to which to select in the results.
    /// </summary>
    public IEnumerable<SelectItemToken> SelectTerms
    {
        get { return this.selectTerms; }
    }
}