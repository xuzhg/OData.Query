//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Tokens;
using System.Linq.Expressions;

namespace Microsoft.OData.Query.Services
{
    /// <summary>
    /// An interface to parse the raw query to token
    /// </summary>
    public interface IQueryTokenizator
    {
        QueryToken[] Tokenize(string rawQuery, QueryTokenizationContext context);
    }
}
