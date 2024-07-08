//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections;

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Base class for all semantic metadata bound nodes.
/// It's node for abstract search tree (AST).
/// </summary>
public abstract class QueryNode
{
    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public abstract QueryNodeKind Kind { get; }

    /// <summary>
    /// Gets the type of this node represents.
    /// </summary>
    public abstract Type NodeType { get; }
}

public interface IQueryNode
{
    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    QueryNodeKind Kind { get; }

    /// <summary>
    /// Gets the type of this node represents.
    /// </summary>
    Type NodeType { get; }
}

public class KeyValueNode : QueryNode
{
    public string Key { get; set; }

    public QueryNode Value { get; set; }

    public override QueryNodeKind Kind => throw new NotImplementedException();

    public override Type NodeType => throw new NotImplementedException();
}

public class QueryOptionNode : QueryNode, IList<KeyValueNode>
{
    public KeyValueNode this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override QueryNodeKind Kind => throw new NotImplementedException();

    public override Type NodeType => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(KeyValueNode item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(KeyValueNode item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(KeyValueNode[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValueNode> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public int IndexOf(KeyValueNode item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, KeyValueNode item)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValueNode item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

//public class QueryOptionNode : Dictionary<string, IQueryNode>, IQueryNode
//{
//    public override QueryNodeKind Kind => throw new NotImplementedException();

//    public override Type NodeType => throw new NotImplementedException();
//}