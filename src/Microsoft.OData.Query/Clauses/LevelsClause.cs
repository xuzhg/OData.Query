//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Clauses;

public class LevelsClause
{
    public static LevelsClause MaxLevels = new LevelsClause();

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelsClause" /> class.
    /// </summary>
    /// <param name="level">The level value for the LevelsClause.
    /// </param>
    public LevelsClause(long level)
    {
        IsMax = false;
        Level = level;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelsClause" /> class.
    /// </summary>
    private LevelsClause()
    {
        IsMax = true;
        Level = -1;
    }

    /// <summary>
    /// Get a flag indicating whether max level is specified.
    /// </summary>
    public bool IsMax { get; }

    /// <summary>
    /// The level value for current expand option.
    /// </summary>
    /// <remarks>This value is trivial when IsMax is True.</remarks>
    public long Level { get; }
}
