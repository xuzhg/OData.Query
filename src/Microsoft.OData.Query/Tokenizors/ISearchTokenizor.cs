//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Tokens;

namespace Microsoft.OData.Query.Tokenizors
{
    public interface ISearchTokenizor
    {
        QueryToken TokenizeSearch(string search);
    }
}
