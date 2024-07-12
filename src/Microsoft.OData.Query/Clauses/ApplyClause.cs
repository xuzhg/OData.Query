//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Clauses;

public class ApplyClause
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplyClause" /> class.
    /// </summary>
    /// <param name="transformations">The <see cref="TransformationNode"/>s.</param>
    public ApplyClause(IList<TransformationNode> transformations)
    {
        Transformations = transformations;
    }

    /// <summary>
    /// The collection of transformations to perform.
    /// </summary>
    public IEnumerable<TransformationNode> Transformations { get; }
}
