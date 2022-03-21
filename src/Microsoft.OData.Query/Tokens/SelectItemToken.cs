//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a select item.
    /// </summary>
    public sealed class SelectItemToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectItemToken" /> class.
        /// </summary>
        /// <param name="path">The path to the property for this select item.</param>
        public SelectItemToken(PathSegmentToken path)
        {
            PathToProperty = path ?? throw new ArgumentNullException(nameof(path));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectItemToken" /> class.
        /// </summary>
        /// <param name="path">the path to the property for this select item.</param>
        /// <param name="select">the nested $select option for this select item.</param>
        /// <param name="filter">the nested $filter option for this select item.</param>
        /// <param name="orderBy">the nested $orderby options for this select item.</param>
        /// <param name="top">the nested $top option for this select item.</param>
        /// <param name="skip">the nested $skip option for this select item.</param>
        /// <param name="count">the nested $count option for this select item.</param>
        /// <param name="search">the nested $search option for this select item.</param>
        /// <param name="compute">the nested $compute option for this select item.</param>
        public SelectItemToken(PathSegmentToken path,
            SelectToken select,
            FilterToken filter,
            OrderByToken orderBy,
            TopToken top,
            SkipToken skip,
            CountToken count,
            SearchToken search,
            ComputeToken compute)
        {
            PathToProperty = path ?? throw new ArgumentNullException(nameof(path));

            Select = select;
            Filter = filter;
            OrderBy = orderBy;
            Top = top;
            Skip = skip;
            Count = count;
            Search = search;
            Compute = compute;
        }

        /// <summary>
        /// Gets the property for this select or expand term.
        /// </summary>
        public PathSegmentToken PathToProperty { get; }

        /// <summary>
        /// Gets the select option for this select or expand term.
        /// </summary>
        public SelectToken Select { get; }

        /// <summary>
        /// Gets the filter option for this select or expand term.
        /// </summary>
        public FilterToken Filter { get; }

        /// <summary>
        /// Gets the orderby options for this select or expand term.
        /// </summary>
        public OrderByToken OrderBy { get; }

        /// <summary>
        /// Gets the search option for this select or expand term.
        /// </summary>
        public SearchToken Search { get; }

        /// <summary>
        /// Gets the top option for this select or expand term.
        /// </summary>
        public TopToken Top { get; }

        /// <summary>
        /// Gets the skip option for this select or expand term.
        /// </summary>
        public SkipToken Skip { get; }

        /// <summary>
        /// Gets the query count option for this select or expand term.
        /// </summary>
        public CountToken Count { get; }

        /// <summary>
        /// Gets the compute option for this select or expand term.
        /// </summary>
        public ComputeToken Compute { get; }
    }
}
