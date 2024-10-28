/*
 * LinkedList
 * 
 * Author: Andre Ferreira
 * 
 * Copyright (c) Uzmi Games. Licensed under the MIT License.
 *    
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/// <summary>
/// Represents a node in the linked list.
/// </summary>
/// <typeparam name="T">The type of data stored in the node.</typeparam>
public class ListNode<T>
{
    /// <summary>
    /// Gets or sets the data stored in the node.
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Gets or sets the reference to the next node in the list.
    /// </summary>
    public ListNode<T> Next { get; set; } = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListNode{T}"/> class with the specified data.
    /// </summary>
    /// <param name="data">The data to store in the node.</param>
    public ListNode(T data)
    {
        Data = data;
    }
}

/// <summary>
/// Represents a singly linked list.
/// </summary>
/// <typeparam name="T">The type of elements stored in the list.</typeparam>
public class LinkedList<T>
{
    private ListNode<T> Head = null;

    /// <summary>
    /// Appends a new element to the end of the linked list.
    /// </summary>
    /// <param name="data">The data to add to the list.</param>
    public void Append(T data)
    {
        var newNode = new ListNode<T>(data);

        if (Head == null)
        {
            Head = newNode;
        }
        else
        {
            var current = Head;

            while (current.Next != null)
                current = current.Next;

            current.Next = newNode;
        }
    }

    /// <summary>
    /// Prepends a new element to the beginning of the linked list.
    /// </summary>
    /// <param name="data">The data to add to the list.</param>
    public void Prepend(T data)
    {
        var newNode = new ListNode<T>(data);
        newNode.Next = Head;
        Head = newNode;
    }

    /// <summary>
    /// Removes the first occurrence of the specified element from the linked list.
    /// </summary>
    /// <param name="data">The data to remove from the list.</param>
    public void Remove(T data)
    {
        if (Head == null)
            return;

        if (Head.Data.Equals(data))
        {
            Head = Head.Next;
            return;
        }

        var current = Head;

        while (current.Next != null && !current.Next.Data.Equals(data))
            current = current.Next;

        if (current.Next != null && current.Next.Data.Equals(data))
            current.Next = current.Next.Next;
    }

    /// <summary>
    /// Gets the head node of the linked list.
    /// </summary>
    /// <returns>The head <see cref="ListNode{T}"/> of the list, or <c>null</c> if the list is empty.</returns>
    public ListNode<T> GetHead()
    {
        return Head;
    }
}