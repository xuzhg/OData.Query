//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query;

/// <summary>
/// Constants string for query option.
/// </summary>
internal static class QueryStringConstants
{
    /// <summary>A filter query option name.</summary>
    public const string Filter = "$filter";

    /// <summary>An apply query option name.</summary>
    public const string Apply = "$apply";

    /// <summary>An order by query option name.</summary>
    public const string OrderBy = "$orderby";

    /// <summary>A select query option name.</summary>
    public const string Select = "$select";

    /// <summary>An expand query option name.</summary>
    public const string Expand = "$expand";

    /// <summary>An expand query option name.</summary>
    public const string Search = "$search";

    /// <summary>An compute query option name.</summary>
    public const string Compute = "$compute";

    /// <summary>A skip query option name.</summary>
    public const string Skip = "$skip";

    /// <summary>A skip token query option name.</summary>
    public const string SkipToken = "$skiptoken";

    /// <summary>A delta token query option name.</summary>
    public const string DeltaToken = "$deltatoken";

    /// <summary>A top query option name.</summary>
    public const string Top = "$top";

    /// <summary>A count query option name.</summary>
    public const string Count = "$count";

    /// <summary>An index query option name.</summary>
    public const string Index = "$index";

    /// <summary>A format query option name.</summary>
    public const string Format = "$format";
}
