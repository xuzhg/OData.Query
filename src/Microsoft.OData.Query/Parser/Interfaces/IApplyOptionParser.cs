//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the $apply query expression and produces the abstract search node.
/// </summary>
public interface IApplyOptionParser
{
    /// <summary>
    /// Parses the $apply expression.
    /// </summary>
    /// <param name="apply">The $apply expression string to parse.</param>
    /// <returns>The apply token parsed.</returns>
    ApplyClause Parse(ApplyToken apply, QueryParserContext context);

    ValueTask<ApplyClause> ParseAsync(string apply, QueryParserContext context);
}
