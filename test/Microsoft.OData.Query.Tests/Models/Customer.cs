//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tests.Models;

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public double Salary { get; set; }

    public Address Address { get; set; }
}