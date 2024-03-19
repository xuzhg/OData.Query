//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Parser;

namespace Microsoft.OData.Query;

public class ODataQueryOptionParser<T> : ODataQueryOptionParser
{
    public ODataQueryOptionParser(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}

/// <summary>
/// Engine Pipeline
/// 1) 
/// </summary>
public class ODataQueryOptionParser : IODataQueryOptionParser
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ODataQueryOptionParser" /> class.
    /// </summary>
    /// <param name="serviceProvider">The required tokenizer.</param>
    public ODataQueryOptionParser(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Parses the input query string to OData query nodes.
    /// </summary>
    /// <param name="query">The odata query string, it should be escaped query string.</param>
    /// <param name="context">The parser context.</param>
    /// <returns>The OData query option parsed.</returns>
    public virtual async ValueTask<ODataQueryOption> ParseAsync(string query, QueryParserContext context)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentNullException(nameof(query));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IDictionary<string, ReadOnlyMemory<char>> queryOptionsDict = QueryStringHelpers.SplitQuery(query);

        ODataQueryOption result = new ODataQueryOption();
        foreach (var queryOptionItem in queryOptionsDict)
        {
            string normalizedQueryOptionName = NormalizeQueryOption(queryOptionItem.Key, context);

            switch (normalizedQueryOptionName)
            {
                case QueryStringConstants.Apply: // $apply
                    await ParseApply(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.Compute: // $compute
                    await ParseCompute(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.Filter: // $filter
                    await ParseFilter(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.OrderBy: // $orderby
                    await ParseOrderBy(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.Select: // $select
                    await ParseSelect(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.Expand: // $select
                    await ParseExpand(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.Search: // $search
                    await ParseSearch(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.Count: // $count
                    await ParseCount(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.Index: // $index
                    await ParseIndex(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.Top: // $top
                    await ParseTop(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.Skip: // $skip
                    await ParseSkip(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.SkipToken: // $skiptoken
                    await ParseSkipToken(queryOptionItem.Value, result, context);
                    break;

                case QueryStringConstants.DeltaToken: // $deltatoken
                    await ParseDeltaToken(queryOptionItem.Value, result, context);
                    break;

                default:
                    await ParseCustomized(queryOptionItem.Key, queryOptionItem.Value, result, context);
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apply"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual async Task ParseApply(ReadOnlyMemory<char> apply, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Apply != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Apply, context);
        }

        IApplyOptionParser applyParser = _serviceProvider?.GetService<IApplyOptionParser>() ?? new ApplyOptionParser();
        queryOption.Apply = await applyParser.ParseAsync(apply.Span.ToString(), context);
    }

    protected virtual async Task ParseCompute(ReadOnlyMemory<char> compute, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Compute != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Compute, context);
        }

        IComputeOptionParser computeParser = _serviceProvider?.GetService<IComputeOptionParser>() ?? new ComputeOptionParser();
        queryOption.Compute = await computeParser.ParseAsync(compute.Span.ToString(), context);
    }

    protected virtual async Task ParseFilter(ReadOnlyMemory<char> filter, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Filter != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Filter, context);
        }

        IFilterOptionParser filterParser = _serviceProvider?.GetService<IFilterOptionParser>() ?? new FilterOptionParser();
        queryOption.Filter = await filterParser.ParseAsync(filter.Span.ToString(), context);
    }

    protected virtual async Task ParseOrderBy(ReadOnlyMemory<char> orderBy, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.OrderBy != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.OrderBy, context);
        }

        IOrderByOptionParser orderByParser = _serviceProvider?.GetService<IOrderByOptionParser>() ?? new OrderByOptionParser();
        queryOption.OrderBy = await orderByParser.ParseAsync(orderBy.Span.ToString(), context);
    }

    protected virtual async Task ParseSelect(ReadOnlyMemory<char> select, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Select != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Select, context);
        }

        ISelectOptionParser selectParser = _serviceProvider?.GetService<ISelectOptionParser>() ?? new SelectOptionParser();
        queryOption.Select = await selectParser.ParseAsync(select.Span.ToString(), context);
    }

    protected virtual async Task ParseExpand(ReadOnlyMemory<char> expand, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Expand != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Expand, context);
        }

        IExpandOptionParser expandParser = _serviceProvider?.GetService<IExpandOptionParser>() ?? new ExpandOptionParser();
        queryOption.Expand = await expandParser.ParseAsync(expand.Span.ToString(), context);
    }

    protected virtual async Task ParseCount(ReadOnlyMemory<char> count, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Count != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Count, context);
        }

        StringComparison comparison = context.EnableCaseInsensitive ?
            StringComparison.OrdinalIgnoreCase :
            StringComparison.Ordinal;

        if (count.Span.Equals("true", comparison))
        {
            queryOption.Count = true;
        }
        else if (count.Span.Equals("false", comparison))
        {
            queryOption.Count = false;
        }
        else
        {
            throw new QueryParserException(Error.Format(SRResources.QueryOptionParser_InvalidDollarCount, count.Span.ToString()));
        }

        await Task.CompletedTask;
    }

    protected virtual async Task ParseTop(ReadOnlyMemory<char> top, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Top != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Top, context);
        }

        long topValue;
        if (!long.TryParse(top.Span, out topValue) || topValue < 0)
        {
            throw new QueryParserException(Error.Format(SRResources.QueryOptionParser_InvalidIntegerOptionValue, top.Span.ToString(), "$top"));
        }

        await Task.CompletedTask;
    }

    protected virtual async Task ParseSkip(ReadOnlyMemory<char> skip, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Skip != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Skip, context);
        }

        long skipValue;
        if (!long.TryParse(skip.Span, out skipValue) || skipValue < 0)
        {
            throw new QueryParserException(Error.Format(SRResources.QueryOptionParser_InvalidIntegerOptionValue, skip.Span.ToString(), "$skip"));
        }

        await Task.CompletedTask;
    }

    protected virtual async Task ParseIndex(ReadOnlyMemory<char> index, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Index != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Index, context);
        }

        long indexValue;
        if (!long.TryParse(index.Span, out indexValue) || indexValue < 0)
        {
            throw new QueryParserException(Error.Format(SRResources.QueryOptionParser_InvalidIntegerOptionValue, index.Span.ToString(), "$index"));
        }

        await Task.CompletedTask;
    }

    protected virtual async Task ParseSearch(ReadOnlyMemory<char> search, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.Search != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Search, context);
        }

        ISearchOptionParser searchParser = _serviceProvider?.GetService<ISearchOptionParser>() ?? new SearchOptionParser();
        queryOption.Search = await searchParser.ParseAsync(search.Span.ToString(), context);
    }

    protected virtual async Task ParseSkipToken(ReadOnlyMemory<char> skipToken, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.SkipToken != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.SkipToken, context);
        }

        queryOption.SkipToken = skipToken.Span.ToString();
        await Task.CompletedTask;
    }

    protected virtual async Task ParseDeltaToken(ReadOnlyMemory<char> deltaToken, ODataQueryOption queryOption, QueryParserContext context)
    {
        if (queryOption.DeltaToken != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.DeltaToken, context);
        }

        queryOption.DeltaToken = deltaToken.Span.ToString();
        await Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual async Task ParseCustomized(string key, ReadOnlyMemory<char> value, ODataQueryOption result, QueryParserContext context)
    {
        // by default, nothing here
        await Task.CompletedTask;
    }

    private static void ThrowQueryParameterMoreThanOnce(string queryName, QueryParserContext context)
    {
        throw new QueryParserException(Error.Format(SRResources.QueryOptionParser_QueryParameterMustBeSpecifiedOnce,
            queryName,
            context.EnableCaseInsensitive ? "Enabled" : "Disabled",
            context.EnableNoDollarPrefix ? "Enabled" : "Disabled"));
    }

    /// <summary>
    /// Gets query options according to case sensitivity and whether no dollar query options is enabled.
    /// </summary>
    /// <param name="queryOptionName">The query option key from request.</param>
    /// <param name="buildInName">The built-in system query option key name, it should start with "$", for example: $select.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>true/false.</returns>
    private static bool IsSystemQueryOption(string queryOptionName, string buildInName, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(queryOptionName))
        {
            return false;
        }

        string changedQueryOptionName = queryOptionName;
        if (context.EnableCaseInsensitive)
        {
            changedQueryOptionName = queryOptionName.ToLowerInvariant();
        }

        // Comparing like: "queryName == $select"
        if (changedQueryOptionName.Equals(buildInName, StringComparison.Ordinal))
        {
            return true;
        }

        // Comparing like: "queryName == select"
        if (context.EnableNoDollarPrefix &&
            changedQueryOptionName.AsSpan().Equals(buildInName.AsSpan().Slice(1), StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static string NormalizeQueryOption(string queryOptionName, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(queryOptionName))
        {
            return queryOptionName;
        }

        string changedQueryOptionName = queryOptionName;
        if (context.EnableNoDollarPrefix && !queryOptionName.StartsWith("$", StringComparison.Ordinal))
        {
            changedQueryOptionName = $"${changedQueryOptionName}";
        }

        return context.EnableCaseInsensitive ? changedQueryOptionName.ToLowerInvariant() : changedQueryOptionName;
    }
}

#if false
/// <summary>
/// Parser for query options
/// </summary>
public class ODataQueryOptionParser2
{
    #region private fields
    /// <summary>Target Edm type. </summary>
    private readonly Type _targetType;

    /// <summary> Dictionary of query options </summary>
    private readonly IDictionary<string, string> _queryOptions;

    private readonly IServiceProvider _serviceProvider;

    /// <summary>Filter clause.</summary>
    private FilterClause filterClause;

    /// <summary>SelectAndExpand clause.</summary>
    private SelectExpandClause selectExpandClause;

    /// <summary>Orderby clause.</summary>
    private OrderByClause orderByClause;

    /// <summary>Search clause.</summary>
    private SearchClause searchClause;

    /// <summary>
    /// Apply clause for aggregation queries
    /// </summary>
    private ApplyClause applyClause;

    /// <summary>
    /// Compute clause for computation queries
    /// </summary>
    private ComputeClause computeClause;

    #endregion private fields

    #region constructor
    /// <summary>
    /// Constructor for ODataQueryOptionParser
    /// </summary>
    /// <param name="model">Model to use for metadata binding.</param>
    /// <param name="targetEdmType">The target EdmType to apply the query option on.</param>
    /// <param name="targetNavigationSource">The target navigation source to apply the query option on.</param>
    /// <param name="queryOptions">The dictionary storing query option key-value pairs.</param>
    public ODataQueryOptionParser(Type targetType, IDictionary<string, string> queryOptions)
        : this(model, targetEdmType, targetNavigationSource, queryOptions, null)
    {
    }

    /// <summary>
    /// Constructor for ODataQueryOptionParser
    /// </summary>
    /// <param name="model">Model to use for metadata binding.</param>
    /// <param name="targetEdmType">The target EdmType to apply the query option on.</param>
    /// <param name="targetNavigationSource">The target navigation source to apply the query option on.</param>
    /// <param name="queryOptions">The dictionary storing query option key-value pairs.</param>
    /// <param name="container">The optional dependency injection container to get related services for URI parsing.</param>
    public ODataQueryOptionParser(IEdmModel model, IEdmType targetEdmType, IDictionary<string, string> queryOptions, IServiceProvider container)
    {
        ExceptionUtils.CheckArgumentNotNull(queryOptions, "queryOptions");

        this.odataPathInfo = new ODataPathInfo(targetEdmType, targetNavigationSource);
        this.targetEdmType = this.odataPathInfo.TargetEdmType;
        this.queryOptions = queryOptions;
        this.Configuration = new ODataUriParserConfiguration(model, container)
        {
            ParameterAliasValueAccessor = new ParameterAliasValueAccessor(queryOptions.Where(_ => _.Key.StartsWith("@", StringComparison.Ordinal)).ToDictionary(_ => _.Key, _ => _.Value))
        };
    }

    /// <summary>
    /// Constructor for ODataQueryOptionParser
    /// </summary>
    /// <param name="model">Model to use for metadata binding.</param>
    /// <param name="odataPath">The odata path to apply the query option on.</param>
    /// <param name="queryOptions">The dictionary storing query option key-value pairs.</param>
    public ODataQueryOptionParser(Type targetType, string queryOption, IServiceProvider serviceProvider)
        : this(model, odataPath, queryOptions, null)
    {
    }

    /// <summary>
    /// Constructor for ODataQueryOptionParser
    /// </summary>
    /// <param name="model">Model to use for metadata binding.</param>
    /// <param name="odataPath">The odata path to apply the query option on.</param>
    /// <param name="queryOptions">The dictionary storing query option key-value pairs.</param>
    /// <param name="container">The optional dependency injection container to get related services for URI parsing.</param>
    public ODataQueryOptionParser(Type targetType, IDictionary<string, string> queryOptions, IServiceProvider serviceProvider)
    {
        _targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        _queryOptions = queryOptions ?? throw new ArgumentNullException(nameof(queryOptions));
        _serviceProvider = serviceProvider;

        this.Configuration = new ODataUriParserConfiguration(model, container)
        {
            ParameterAliasValueAccessor = new ParameterAliasValueAccessor(queryOptions.Where(_ => _.Key.StartsWith("@", StringComparison.Ordinal)).ToDictionary(_ => _.Key, _ => _.Value))
        };
    }

    #endregion constructor

    #region properties
    /// <summary>
    /// The settings for this instance of <see cref="ODataQueryOptionParser"/>. Refer to the documentation for the individual properties of <see cref="ODataUriParserSettings"/> for more information.
    /// </summary>
    public ODataUriParserSettings Settings
    {
        get { return this.Configuration.Settings; }
    }

    /// <summary>
    /// Get the parameter alias nodes info.
    /// </summary>
    public IDictionary<string, SingleValueNode> ParameterAliasNodes
    {
        get { return this.Configuration.ParameterAliasValueAccessor.ParameterAliasValueNodesCached; }
    }

    /// <summary>
    /// Gets or sets the <see cref="ODataUriResolver"/> for <see cref="ODataUriParser"/>.
    /// </summary>
    public ODataUriResolver Resolver
    {
        get { return this.Configuration.Resolver; }
        set { this.Configuration.Resolver = value; }
    }

    /// <summary>The parser's configuration. </summary>
    internal ODataUriParserConfiguration Configuration { get; set; }
    #endregion properties

    #region public methods
    /// <summary>
    /// Parses a filter clause on the given full Uri, binding
    /// the text into semantic nodes using the constructed mode.
    /// </summary>
    /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
    public virtual FilterClause ParseFilter()
    {
        if (this.filterClause != null)
        {
            return this.filterClause;
        }

        string filterQuery;

        if (!this.TryGetQueryOption(QueryStringConstants.Filter, out filterQuery)
            || string.IsNullOrEmpty(filterQuery)
            || this.targetEdmType == null)
        {
            return null;
        }

        this.filterClause = ParseFilterImplementation(filterQuery, this.Configuration, this.odataPathInfo);
        return this.filterClause;
    }

    /// <summary>
    /// Parses a apply clause on the given full Uri, binding
    /// the text into semantic nodes using the constructed mode.
    /// </summary>
    /// <returns>A <see cref="ApplyClause"/> representing the aggregation query.</returns>
    public ApplyClause ParseApply()
    {
        if (this.applyClause != null)
        {
            return this.applyClause;
        }

        string applyQuery;

        if (!this.TryGetQueryOption(UriQueryConstants.ApplyQueryOption, out applyQuery)
            || string.IsNullOrEmpty(applyQuery)
            || this.targetEdmType == null)
        {
            return null;
        }

        this.applyClause = ParseApplyImplementation(applyQuery, this.Configuration, this.odataPathInfo);
        return this.applyClause;
    }

    /// <summary>
    /// ParseSelectAndExpand from an instantiated class
    /// </summary>
    /// <returns>A SelectExpandClause with the semantic representation of select and expand terms</returns>
    public SelectExpandClause ParseSelectAndExpand()
    {
        if (this.selectExpandClause != null)
        {
            return this.selectExpandClause;
        }

        string selectQuery, expandQuery;

        // Intended to use bitwise AND & instead of logic AND && here, prevent short-circuiting.
        if ((!this.TryGetQueryOption(UriQueryConstants.SelectQueryOption, out selectQuery) || selectQuery == null)
            & (!this.TryGetQueryOption(UriQueryConstants.ExpandQueryOption, out expandQuery) || expandQuery == null)
            || this.targetEdmType == null)
        {
            return null;
        }

        IEdmStructuredType structuredType = this.targetEdmType as IEdmStructuredType;
        if (structuredType == null)
        {
            throw new ODataException(Strings.UriParser_TypeInvalidForSelectExpand(this.targetEdmType));
        }

        this.selectExpandClause = ParseSelectAndExpandImplementation(selectQuery, expandQuery, this.Configuration, this.odataPathInfo);
        return this.selectExpandClause;
    }

    /// <summary>
    /// Parses an orderBy clause on the given full Uri, binding
    /// the text into semantic nodes using the constructed mode.
    /// </summary>
    /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
    public OrderByClause ParseOrderBy()
    {
        if (this.orderByClause != null)
        {
            return this.orderByClause;
        }

        string orderByQuery;
        if (!this.TryGetQueryOption(UriQueryConstants.OrderByQueryOption, out orderByQuery)
            || string.IsNullOrEmpty(orderByQuery)
            || this.targetEdmType == null)
        {
            return null;
        }

        this.orderByClause = ParseOrderByImplementation(orderByQuery, this.Configuration, this.odataPathInfo);
        return this.orderByClause;
    }

    /// <summary>
    /// Parses a $top query option
    /// </summary>
    /// <returns>A value representing that top option, null if $top query does not exist.</returns>
    public long? ParseTop()
    {
        string topQuery;
        return this.TryGetQueryOption(UriQueryConstants.TopQueryOption, out topQuery) ? ParseTop(topQuery) : null;
    }

    /// <summary>
    /// Parses a $skip query option
    /// </summary>
    /// <returns>A value representing that skip option, null if $skip query does not exist.</returns>
    public long? ParseSkip()
    {
        string skipQuery;
        return this.TryGetQueryOption(UriQueryConstants.SkipQueryOption, out skipQuery) ? ParseSkip(skipQuery) : null;
    }

    /// <summary>
    /// Parses a $index query option
    /// </summary>
    /// <returns>A value representing that index option, null if $index query does not exist.</returns>
    public long? ParseIndex()
    {
        string indexQuery;
        return this.TryGetQueryOption(UriQueryConstants.IndexQueryOption, out indexQuery) ? ParseIndex(indexQuery) : null;
    }

    /// <summary>
    /// Parses a $count query option
    /// </summary>
    /// <returns>A count representing that count option, null if $count query does not exist.</returns>
    public bool? ParseCount()
    {
        string countQuery;
        return this.TryGetQueryOption(UriQueryConstants.CountQueryOption, out countQuery) ? ParseCount(countQuery) : null;
    }

    /// <summary>
    /// Parses the $search.
    /// </summary>
    /// <returns>SearchClause representing $search.</returns>
    public SearchClause ParseSearch()
    {
        if (this.searchClause != null)
        {
            return this.searchClause;
        }

        string searchQuery;
        if (!this.TryGetQueryOption(UriQueryConstants.SearchQueryOption, out searchQuery)
            || searchQuery == null)
        {
            return null;
        }

        this.searchClause = ParseSearchImplementation(searchQuery, this.Configuration);
        return searchClause;
    }

    /// <summary>
    /// Parses a $skiptoken query option
    /// </summary>
    /// <returns>A value representing that skip token option, null if $skiptoken query does not exist.</returns>
    public string ParseSkipToken()
    {
        string skipTokenQuery;
        return this.TryGetQueryOption(UriQueryConstants.SkipTokenQueryOption, out skipTokenQuery) ? skipTokenQuery : null;
    }

    /// <summary>
    /// Parses a $deltatoken query option
    /// </summary>
    /// <returns>A value representing that delta token option, null if $deltatoken query does not exist.</returns>
    public string ParseDeltaToken()
    {
        string deltaTokenQuery;
        return this.TryGetQueryOption(UriQueryConstants.DeltaTokenQueryOption, out deltaTokenQuery) ? deltaTokenQuery : null;
    }

    /// <summary>
    /// Parses a compute clause on the given full Uri, binding
    /// the text into semantic nodes using the constructed mode.
    /// </summary>
    /// <returns>A <see cref="ComputeClause"/> representing the computed properties.</returns>
    public ComputeClause ParseCompute()
    {
        if (this.computeClause != null)
        {
            return this.computeClause;
        }

        string computeQuery;

        if (!this.TryGetQueryOption(UriQueryConstants.ComputeQueryOption, out computeQuery)
            || string.IsNullOrEmpty(computeQuery)
            || this.targetEdmType == null)
        {
            return null;
        }

        this.computeClause = ParseComputeImplementation(computeQuery, this.Configuration, this.odataPathInfo);
        return this.computeClause;
    }
    #endregion public methods

    #region private methods
    /// <summary>
    /// Parses a <paramref name="filter"/> clause, binding
    /// the text into semantic nodes using the provided model.
    /// </summary>
    /// <param name="filter">String representation of the filter expression.</param>
    /// <param name="configuration">The configuration used for binding.</param>
    /// <param name="odataPathInfo">The path info from Uri path.</param>
    /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
    private FilterClause ParseFilterImplementation(string filter, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
    {
        ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
        ExceptionUtils.CheckArgumentNotNull(odataPathInfo, "odataPathInfo");
        ExceptionUtils.CheckArgumentNotNull(filter, "filter");

        // Get the syntactic representation of the filter expression
        UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier, configuration.Resolver.EnableNoDollarQueryOptions);
        QueryToken filterToken = expressionParser.ParseFilter(filter);

        // Bind it to metadata
        BindingState state = CreateBindingState(configuration, odataPathInfo);

        MetadataBinder binder = new MetadataBinder(state);
        FilterBinder filterBinder = new FilterBinder(binder.Bind, state);
        FilterClause boundNode = filterBinder.BindFilter(filterToken);

        return boundNode;
    }

    /// <summary>
    /// Parses an <paramref name="apply"/> clause, binding
    /// the text into a metadata-bound or dynamic properties to be applied using the provided model.
    /// </summary>
    /// <param name="apply">String representation of the apply expression.</param>
    /// <param name="configuration">The configuration used for binding.</param>
    /// <param name="odataPathInfo">The path info from Uri path.</param>
    /// <returns>A <see cref="ApplyClause"/> representing the metadata bound apply expression.</returns>
    private static ApplyClause ParseApplyImplementation(string apply, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
    {
        ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
        ExceptionUtils.CheckArgumentNotNull(apply, "apply");

        // Get the syntactic representation of the apply expression
        UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier);
        var applyTokens = expressionParser.ParseApply(apply);

        // Bind it to metadata
        BindingState state = new BindingState(configuration, odataPathInfo.Segments.ToList());
        state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(odataPathInfo.TargetEdmType.ToTypeReference(), odataPathInfo.TargetNavigationSource);
        state.RangeVariables.Push(state.ImplicitRangeVariable);
        MetadataBinder binder = new MetadataBinder(state);
        ApplyBinder applyBinder = new ApplyBinder(binder.Bind, state, configuration, odataPathInfo);
        ApplyClause boundNode = applyBinder.BindApply(applyTokens);

        return boundNode;
    }

    /// <summary>
    /// Parses the <paramref name="select"/> and <paramref name="expand"/> clauses, binding
    /// the text into a metadata-bound list of properties to be selected using the provided model.
    /// </summary>
    /// <param name="select">String representation of the select expression from the URI.</param>
    /// <param name="expand">String representation of the expand expression from the URI.</param>
    /// <param name="configuration">The configuration used for binding.</param>
    /// <param name="odataPathInfo">The path info from Uri path.</param>
    /// <returns>A <see cref="SelectExpandClause"/> representing the metadata bound select and expand expression.</returns>
    private SelectExpandClause ParseSelectAndExpandImplementation(string select, string expand, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
    {
        ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
        ExceptionUtils.CheckArgumentNotNull(configuration.Model, "model");

        ExpandToken expandTree;
        SelectToken selectTree;

        // syntactic pass , pass in the expand parent entity type name, in case expand option contains star, will get all the parent entity navigation properties (both declared and dynamical).
        SelectExpandSyntacticParser.Parse(select, expand, odataPathInfo.TargetStructuredType, configuration, out expandTree, out selectTree);

        // semantic pass
        BindingState state = CreateBindingState(configuration, odataPathInfo);
        return SelectExpandSemanticBinder.Bind(odataPathInfo, expandTree, selectTree, configuration, state);
    }

    /// <summary>
    /// Parses an <paramref name="orderBy "/> clause, binding
    /// the text into semantic nodes using the provided model.
    /// </summary>
    /// <param name="orderBy">String representation of the orderby expression.</param>
    /// <param name="configuration">The configuration used for binding.</param>
    /// <param name="odataPathInfo">The path info from Uri path.</param>
    /// <returns>An <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
    private OrderByClause ParseOrderByImplementation(string orderBy, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
    {
        ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
        ExceptionUtils.CheckArgumentNotNull(configuration.Model, "model");
        ExceptionUtils.CheckArgumentNotNull(orderBy, "orderBy");

        // Get the syntactic representation of the orderby expression
        UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(configuration.Settings.OrderByLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier);
        var orderByQueryTokens = expressionParser.ParseOrderBy(orderBy);

        // Bind it to metadata
        BindingState state = CreateBindingState(configuration, odataPathInfo);

        MetadataBinder binder = new MetadataBinder(state);
        OrderByBinder orderByBinder = new OrderByBinder(binder.Bind);
        OrderByClause orderByClause = orderByBinder.BindOrderBy(state, orderByQueryTokens);

        return orderByClause;
    }

    private BindingState CreateBindingState(ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
    {
        BindingState state = new BindingState(configuration, odataPathInfo.Segments.ToList());
        state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(odataPathInfo.TargetEdmType.ToTypeReference(), odataPathInfo.TargetNavigationSource);
        state.RangeVariables.Push(state.ImplicitRangeVariable);
        state.ResourcePathNavigationSource = odataPathInfo.TargetNavigationSource;
        if (applyClause != null)
        {
            state.AggregatedPropertyNames = applyClause.GetLastAggregatedPropertyNames();
            if (applyClause.Transformations.Any(x => x.Kind == TransformationNodeKind.GroupBy || x.Kind == TransformationNodeKind.Aggregate))
            {
                state.IsCollapsed = true;
            }
        }

        if (computeClause != null)
        {
            var computedProperties = new HashSet<EndPathToken>(computeClause.ComputedItems.Select(i => new EndPathToken(i.Alias, null)));
            if (state.AggregatedPropertyNames == null)
            {
                state.AggregatedPropertyNames = computedProperties;
            }
            else
            {
                state.AggregatedPropertyNames.UnionWith(computedProperties);
            }
        }

        return state;
    }

    /// <summary>
    /// Parses a $top query option
    /// </summary>
    /// <param name="topQuery">The topQuery from the query</param>
    /// <returns>A value representing that top option, null if $top query does not exist.</returns>
    /// <exception cref="ODataException">Throws if the input count is not a valid $top value.</exception>
    private static long? ParseTop(string topQuery)
    {
        if (topQuery == null)
        {
            return null;
        }

        long topValue;
        if (!long.TryParse(topQuery, out topValue) || topValue < 0)
        {
            throw new ODataException(Strings.SyntacticTree_InvalidTopQueryOptionValue(topQuery));
        }

        return topValue;
    }

    /// <summary>
    /// Parses a $skip query option
    /// </summary>
    /// <param name="skipQuery">The count skipQuery from the query</param>
    /// <returns>A value representing that skip option, null if $skip query does not exist.</returns>
    /// <exception cref="ODataException">Throws if the input count is not a valid $skip value.</exception>
    private static long? ParseSkip(string skipQuery)
    {
        if (skipQuery == null)
        {
            return null;
        }

        long skipValue;
        if (!long.TryParse(skipQuery, out skipValue) || skipValue < 0)
        {
            throw new ODataException(Strings.SyntacticTree_InvalidSkipQueryOptionValue(skipQuery));
        }

        return skipValue;
    }

    /// <summary>
    /// Parses a $index query option
    /// </summary>
    /// <param name="indexQuery">The value of $index from the query</param>
    /// <returns>A value representing that index option, null if $index query does not exist.</returns>
    /// <exception cref="ODataException">Throws if the input value is not a valid $index value.</exception>
    private static long? ParseIndex(string indexQuery)
    {
        if (indexQuery == null)
        {
            return null;
        }

        // A negative ordinal number indexes from the end of the collection,
        // with -1 representing an insert as the last item in the collection.
        long indexValue;
        if (!long.TryParse(indexQuery, out indexValue))
        {
            throw new ODataException(Strings.SyntacticTree_InvalidIndexQueryOptionValue(indexQuery));
        }

        return indexValue;
    }

    /// <summary>
    /// Parses a query count option
    /// Valid Samples: $count=true; $count=false
    /// Invalid Samples: $count=True; $count=true
    /// </summary>
    /// <param name="count">The count string from the query</param>
    /// <returns>query count true of false</returns>
    /// <exception cref="ODataException">Throws if the input count is not a valid $count value.</exception>
    private static bool? ParseCount(string count)
    {
        if (count == null)
        {
            return null;
        }

        switch (count.Trim())
        {
            case ExpressionConstants.KeywordTrue:
                return true;
            case ExpressionConstants.KeywordFalse:
                return false;
            default:
                throw new ODataException(Strings.ODataUriParser_InvalidCount(count));
        }
    }

    /// <summary>
    /// Parses the <paramref name="search"/> clause, binding
    /// the text into a metadata-bound list of properties to be selected using the provided model.
    /// </summary>
    /// <param name="search">String representation of the search expression from the URI.</param>
    /// <param name="configuration">The configuration used for binding.</param>
    /// <returns>A <see cref="SearchClause"/> representing the metadata bound search expression.</returns>
    private static SearchClause ParseSearchImplementation(string search, ODataUriParserConfiguration configuration)
    {
        ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
        ExceptionUtils.CheckArgumentNotNull(search, "search");

        SearchParser searchParser = new SearchParser(configuration.Settings.SearchLimit);
        QueryToken queryToken = searchParser.ParseSearch(search);

        // Bind it to metadata
        BindingState state = new BindingState(configuration);
        MetadataBinder binder = new MetadataBinder(state);
        SearchBinder searchBinder = new SearchBinder(binder.Bind);

        return searchBinder.BindSearch(queryToken);
    }

    /// <summary>
    /// Parses the <paramref name="compute"/> clause, binding
    /// the text into a metadata-bound list of computations using the provided model.
    /// </summary>
    /// <param name="compute">String representation of the compute expression from the URI.</param>
    /// <param name="configuration">The configuration used for binding.</param>
    /// <param name="odataPathInfo">The path info from Uri path.</param>
    /// <returns>A <see cref="ComputeClause"/> representing the metadata bound compute expression.</returns>
    private ComputeClause ParseComputeImplementation(string compute, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
    {
        ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
        ExceptionUtils.CheckArgumentNotNull(compute, "compute");

        // Get the syntactic representation of the apply expression
        UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier);
        ComputeToken computeToken = expressionParser.ParseCompute(compute);

        // Bind it to metadata
        BindingState state = CreateBindingState(configuration, odataPathInfo);
        MetadataBinder binder = new MetadataBinder(state);
        ComputeBinder computeBinder = new ComputeBinder(binder.Bind);
        ComputeClause boundNode = computeBinder.BindCompute(computeToken);

        return boundNode;
    }

    #endregion private methods
}
#endif