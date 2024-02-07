//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

public interface IExpandOptionParser
{
    /// <summary>
    /// Parses the $expand expression.
    /// </summary>
    /// <param name="expand">The $expand expression string to parse.</param>
    /// <returns>The expand token.</returns>
    ExpandToken ParseExpand(string expand, QueryTokenizerContext context);
}
