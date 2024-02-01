//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

public interface IComputeOptionParser
{
    /// <summary>
    /// Parses $compute query option.
    /// </summary>
    /// <param name="compute">The $compute expression string to parse.</param>
    /// <returns>The compute token.</returns>
    ComputeToken ParseCompute(string compute, QueryOptionParserContext context);
}