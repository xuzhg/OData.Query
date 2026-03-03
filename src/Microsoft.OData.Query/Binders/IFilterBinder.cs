//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Parser;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.OData.Query.Binders;

/// <summary>
/// Exposes the ability to translate an OData $filter represented by <see cref="FilterClause"/> to the <see cref="Expression"/>.
/// </summary>
public interface IFilterBinder
{
    /// <summary>
    /// Translates an OData $filter represented by <see cref="FilterClause"/> to <see cref="Expression"/>.
    /// $filter=Name eq 'Sam'
    ///    |--  $it => $it.Name == "Sam"
    /// </summary>
    /// <param name="filter">The filter clause.</param>
    /// <param name="context">The query binder context.</param>
    /// <returns>The filter binder result.</returns>
    /// <remarks>reconsider to return "LambdaExpression"? </remarks>
    ValueTask<Expression> BindAsync(FilterClause filter, QueryBinderContext context);
}


public interface IQueryBinder
{
    ValueTask<Expression> BindAsync(ReadOnlyMemory<char> query, QueryBinderContext context);
}



public class QueryApplierContext
{
    public QueryApplierContext(IQueryable source)
    {
        Source = source;
    }

    public IQueryable Source { get; }
}

public class QueryCompositerContext
{
    /// <summary>
    ///  Gets and sets the optional-$-sign-prefix for OData system query option.
    /// </summary>
    public bool EnableNoDollarQueryOptions { get; set; }

    /// <summary>
    /// Gets and sets the value indicating whether to enable case insensitive for the identifier.
    /// </summary>
    public virtual bool EnableCaseInsensitive { get; set; }

    public IQueryParser Parser { get; }
}

public class QueryCompositerSettings
{
    private HandleNullPropagationOption _handleNullPropagationOption = HandleNullPropagationOption.Default;
    private int? _pageSize;
    private int? _modelBoundPageSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryComposerSettings" /> class.
    /// </summary>
    public QueryCompositerSettings()
    {
        EnsureStableOrdering = true;
        EnableConstantParameterization = true;
    }

    /// <summary>
    /// Gets or sets the <see cref="TimeZoneInfo"/>.
    /// </summary>
    public TimeZoneInfo TimeZone { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of query results to return based on the type or property.
    /// </summary>
    /// <value>
    /// The maximum number of query results to return based on the type or property,
    /// or <c>null</c> if there is no limit.
    /// </value>
    internal int? ModelBoundPageSize
    {
        get
        {
            return _modelBoundPageSize;
        }
        set
        {
            if (value.HasValue && value <= 0)
            {
                //throw Error.ArgumentMustBeGreaterThanOrEqualTo("value", value, 1);
                throw new ArgumentOutOfRangeException(nameof(value), value, "ModelBoundPageSize must be greater than or equal to 1.");
            }

            _modelBoundPageSize = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether query composition should
    /// alter the original query when necessary to ensure a stable sort order.
    /// </summary>
    /// <value>A <c>true</c> value indicates the original query should
    /// be modified when necessary to guarantee a stable sort order.
    /// A <c>false</c> value indicates the sort order can be considered
    /// stable without modifying the query.  Query providers that ensure
    /// a stable sort order should set this value to <c>false</c>.
    /// The default value is <c>true</c>.</value>
    public bool EnsureStableOrdering { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how null propagation should
    /// be handled during query composition.
    /// </summary>
    /// <value>
    /// The default is <see cref="HandleNullPropagationOption.Default"/>.
    /// </value>
    public HandleNullPropagationOption HandleNullPropagation
    {
        get
        {
            return _handleNullPropagationOption;
        }
        set
        {
          //  HandleNullPropagationOptionHelper.Validate(value, "value");
            _handleNullPropagationOption = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether constants should be parameterized. Parameterizing constants
    /// would result in better performance with Entity framework.
    /// </summary>
    /// <value>The default value is <c>true</c>.</value>
    public bool EnableConstantParameterization { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether queries with expanded navigations should be formulated
    /// to encourage correlated sub-query results to be buffered.
    /// Buffering correlated sub-query results can reduce the number of queries from N + 1 to 2
    /// by buffering results from the sub-query.
    /// </summary>
    /// <value>The default value is <c>false</c>.</value>
    public bool EnableCorrelatedSubqueryBuffering { get; set; }

    /// <summary>
    /// Gets or sets a value indicating which query options should be ignored when applying queries.
    /// </summary>
    public AllowedQueryOptions IgnoredQueryOptions { get; set; } = AllowedQueryOptions.None;

    /// <summary>
    /// Gets or sets a value indicating which nested query options should be ignored typically within select and expand.
    /// </summary>
    public AllowedQueryOptions IgnoredNestedQueryOptions { get; set; } = AllowedQueryOptions.None;

    /// <summary>
    /// Gets or sets the maximum number of query results to return.
    /// </summary>
    /// <value>
    /// The maximum number of query results to return, or <c>null</c> if there is no limit.
    /// </value>
    public int? PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            if (value.HasValue && value <= 0)
            {
                //throw Error.ArgumentMustBeGreaterThanOrEqualTo("value", value, 1);
                throw new ArgumentOutOfRangeException(nameof(value), value, "PageSize must be greater than or equal to 1.");
            }

            _pageSize = value;
        }
    }

    /// <summary>
    /// Honor $filter inside $expand of non-collection navigation property.
    /// The expanded property is only populated when the filter evaluates to true.
    /// This setting is false by default.
    /// </summary>
    public bool HandleReferenceNavigationPropertyExpandFilter { get; set; }

    internal void CopyFrom(QueryCompositerSettings settings)
    {
        TimeZone = settings.TimeZone;
        EnsureStableOrdering = settings.EnsureStableOrdering;
        EnableConstantParameterization = settings.EnableConstantParameterization;
        HandleNullPropagation = settings.HandleNullPropagation;
        PageSize = settings.PageSize;
        ModelBoundPageSize = settings.ModelBoundPageSize;
        HandleReferenceNavigationPropertyExpandFilter = settings.HandleReferenceNavigationPropertyExpandFilter;
        EnableCorrelatedSubqueryBuffering = settings.EnableCorrelatedSubqueryBuffering;
        IgnoredQueryOptions = settings.IgnoredQueryOptions;
        IgnoredNestedQueryOptions = settings.IgnoredNestedQueryOptions;
    }
}

/// <summary>
/// This enum defines how to handle null propagation in queryable support.
/// </summary>
public enum HandleNullPropagationOption
{
    /// <summary>
    /// Determine how to handle null propagation based on the
    /// query provider during query composition.  This is the
    /// default value used in <see cref="QueryComposerSettings"/>
    /// </summary>
    Default = 0,

    /// <summary>
    /// Handle null propagation during query composition.
    /// </summary>
    True = 1,

    /// <summary>
    /// Do not handle null propagation during query composition.
    /// </summary>
    False = 2
}


/// <summary>
/// OData query options to allow for querying.
/// </summary>
[Flags]
public enum AllowedQueryOptions
{
    /// <summary>
    /// A value that corresponds to allowing no query options.
    /// </summary>
    None = 0x0,

    /// <summary>
    /// A value that corresponds to allowing the $filter query option.
    /// </summary>
    Filter = 0x1,

    /// <summary>
    /// A value that corresponds to allowing the $expand query option.
    /// </summary>
    Expand = 0x2,

    /// <summary>
    /// A value that corresponds to allowing the $select query option.
    /// </summary>
    Select = 0x4,

    /// <summary>
    /// A value that corresponds to allowing the $orderby query option.
    /// </summary>
    OrderBy = 0x8,

    /// <summary>
    /// A value that corresponds to allowing the $top query option.
    /// </summary>
    Top = 0x10,

    /// <summary>
    /// A value that corresponds to allowing the $skip query option.
    /// </summary>
    Skip = 0x20,

    /// <summary>
    /// A value that corresponds to allowing the $count query option.
    /// </summary>
    Count = 0x40,

    /// <summary>
    /// A value that corresponds to allowing the $format query option.
    /// </summary>
    Format = 0x80,

    /// <summary>
    /// A value that corresponds to allowing the $skiptoken query option.
    /// </summary>
    SkipToken = 0x100,

    /// <summary>
    /// A value that corresponds to allowing the $deltatoken query option.
    /// </summary>
    DeltaToken = 0x200,

    /// <summary>
    /// A value that corresponds to allowing the $apply query option.
    /// </summary>
    Apply = 0x400,

    /// <summary>
    /// A value that corresponds to allowing the $compute query option.
    /// </summary>
    Compute = 0x800,

    /// <summary>
    /// A value that corresponds to allowing the $search query option.
    /// </summary>
    Search = 0x1000,

    /// <summary>
    /// A value that corresponds to the default query options supported.
    /// </summary>
    Supported = Filter | OrderBy | Top | Skip | SkipToken | Count | Select | Expand | Format | Apply | Compute | Search,

    /// <summary>
    /// A value that corresponds to allowing all query options.
    /// </summary>
    All = Filter | Expand | Select | OrderBy | Top | Skip | Count | Format | SkipToken | DeltaToken | Apply | Compute | Search
}