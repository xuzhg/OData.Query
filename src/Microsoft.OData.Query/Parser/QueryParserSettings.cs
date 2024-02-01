//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

public class QueryParserSettings
{
    /// <summary>
    /// Default recursive call limit for Filter
    /// </summary>
    internal const int DefaultFilterLimit = 800;

    /// <summary>
    /// Default recursive call limit for OrderBy
    /// </summary>
    internal const int DefaultOrderByLimit = 800;

    /// <summary>
    /// Default tree depth for Select and Expand
    /// </summary>
    internal const int DefaultSelectExpandLimit = 800;

    /// <summary>
    /// Default limit for the path parser.
    /// </summary>
    internal const int DefaultPathLimit = 100;

    /// <summary>
    /// Default limit for the search parser.
    /// </summary>
    internal const int DefaultSearchLimit = 100;

    /// <summary>
    /// The maximum depth of the tree that results from parsing $expand.
    /// </summary>
    private int _maxExpandDepth;

    /// <summary>
    /// The maximum number of <see cref="ExpandedNavigationSelectItem"/> instances that can appear in the tree that results from parsing $expand.
    /// </summary>
    private int _maxExpandCount;

    /// <summary>
    /// the maximum number of segments in a path
    /// </summary>
    private int _pathLimit;

    /// <summary>
    /// the recursive depth of the Syntactic tree for a filter clause
    /// </summary>
    private int _filterLimit;

    /// <summary>
    /// the maximum depth of the syntactic tree for an orderby clause
    /// </summary>
    private int _orderByLimit;

    /// <summary>
    /// the maximum depth of the Syntactic or Semantic tree for a Select.
    /// </summary>
    private int _selectLimit;

    /// <summary>
    /// the maximum depth of the Syntactic or Semantic tree for a Expand.
    /// </summary>
    private int _expandLimit;

    /// <summary>
    /// the maximum depth of the syntactic tree for an search clause
    /// </summary>
    private int _searchLimit;

    /// <summary>
    /// Initializes a new instance of <see cref="QueryParserSettings"/> with default values.
    /// </summary>
    public QueryParserSettings()
    {
        FilterLimit = DefaultFilterLimit;
        OrderByLimit = DefaultOrderByLimit;
        PathLimit = DefaultPathLimit;
        SelectLimit = DefaultSelectExpandLimit;
        ExpandLimit = DefaultSelectExpandLimit;
        SearchLimit = DefaultSearchLimit;

        MaximumExpansionDepth = int.MaxValue;
        MaximumExpansionCount = int.MaxValue;
    }

    /// <summary>
    /// Gets or Sets the limit on the maximum number of segments that can be parsed by the
    /// syntactic parser. This guarantees a set level of performance.
    /// </summary>
    public int PathLimit
    {
        get => _pathLimit;

        set
        {
            if (value < 0)
            {
               // throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
            }

            _pathLimit = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum depth of the tree that results from parsing $expand.
    /// </summary>
    /// <remarks>
    /// This will be validated after parsing completes, and so should not be used to prevent the instantiation of large trees.
    /// Further, redundant expansions will be pruned before validation and will not count towards the maximum.
    /// </remarks>
    public int MaximumExpansionDepth
    {
        get => _maxExpandDepth;
        set
        {
            if (value < 0)
            {
               // throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
            }

            _maxExpandDepth = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum number of <see cref="ExpandedNavigationSelectItem"/> instances that can appear in the tree that results from parsing $expand.
    /// </summary>
    /// <remarks>
    /// This will be validated after parsing completes, and so should not be used to prevent the instantiation of large trees.
    /// Further, redundant expansions will be pruned before validation and will not count towards the maximum.
    /// </remarks>
    public int MaximumExpansionCount
    {
        get => _maxExpandCount;

        set
        {
            if (value < 0)
            {
               // throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
            }

            _maxExpandCount = value;
        }
    }

    /// <summary>
    /// Gets or Sets the maximum recursive depth for a select and expand clause, which limits the maximum depth of the tree that can be parsed by the
    /// syntactic parser. This guarantees a set level of performance.
    /// </summary>
    /// <exception cref="ODataException">Throws if the input value is negative.</exception>
    public int SelectLimit
    {
        get => _selectLimit;
        set
        {
            if (value < 0)
            {
              //  throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
            }

            _selectLimit = value;
        }
    }

    /// <summary>
    /// Gets or Sets the maximum recursive depth for an expand clause, which limits the maximum depth of the tree that can be parsed by the
    /// syntactic parser. This guarantees a set level of performance.
    /// </summary>
    public int ExpandLimit
    {
        get => _expandLimit;
        set
        {
            if (value < 0)
            {
               // throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
            }

            _expandLimit = value;
        }
    }

    /// <summary>
    /// Gets or Sets the limit on the maximum depth of the filter tree that can be parsed by the
    /// syntactic parser. This guarantees a set level of performance.
    /// </summary>
    public int FilterLimit
    {
        get => _filterLimit;
        set
        {
            if (value < 0)
            {
             //   throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
            }

            _filterLimit = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum recursive depth for an orderby clause, which limits the maximum depth of the tree that can be parsed by the
    /// syntactic parser. This guarantees a set level of performance.
    /// </summary>
    public int OrderByLimit
    {
        get => _orderByLimit;
        set
        {
            if (value < 0)
            {
             //   throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
            }

            _orderByLimit = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum recursive depth for an search clause, which limits the maximum depth of the tree that can be parsed by the
    /// syntactic parser. This guarantees a set level of performance.
    /// </summary>
    public int SearchLimit
    {
        get => _searchLimit;
        set
        {
            if (value < 0)
            {
            //    throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
            }

            _searchLimit = value;
        }
    }
}
