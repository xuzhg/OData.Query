//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// Represents the selection of all the actions and functions in a specified namespace.
/// $select=NS.NamespaceName
/// </summary>
public sealed class SelectedNamespaceQualifiedWildcardItem : SelectedItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectedNamespaceQualifiedWildcardItem" /> class.
    /// </summary>
    /// <param name="namespaceName">The namespace of the wildcard.</param>
    public SelectedNamespaceQualifiedWildcardItem(string namespaceName)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
        {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        Namespace = namespaceName;
    }

    /// <summary>
    /// Gets the namespace whose actions and functions should be selected.
    /// </summary>
    public string Namespace { get; }
}
