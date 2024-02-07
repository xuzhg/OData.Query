//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Parser which consumes the $apply query expression and produces the lexical object model.
/// </summary>
public interface IApplyOptionParser
{
    /// <summary>
    /// Parses the $apply expression.
    /// </summary>
    /// <param name="apply">The $apply expression string to parse.</param>
    /// <returns>The apply token parsed.</returns>
    ApplyToken ParseApply(string apply, QueryTokenizerContext context);
}
