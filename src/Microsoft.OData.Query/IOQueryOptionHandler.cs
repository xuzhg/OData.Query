//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query;

public interface IOQueryOptionHandler
{
    ValueTask<IQueryable> ApplyTo(IQueryable query, OQueryOptionSettings settings);

    ValueTask<object> ApplyTo(object entity, OQueryOptionSettings settings);
}
