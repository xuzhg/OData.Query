//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tests.Models;

public class UsAddress : Address
{
    public int ZipCode { get; set; }

    public int SubCode { get; set; }
}
