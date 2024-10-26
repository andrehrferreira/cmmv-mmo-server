﻿/*
 * Byte Buffer Pool
 * 
 * Author: Diego Guedes
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
/// Represents a pool of ByteBuffer objects to optimize memory usage and reduce allocations.
/// </summary>
public class ByteBufferPool
{
    /// <summary>
    /// The head of the linked list in the pool.
    /// </summary>
    public ByteBuffer Head;

    /// <summary>
    /// The tail of the linked list in the pool.
    /// </summary>
    public ByteBuffer Tail;

    /// <summary>
    /// A global instance of the ByteBufferPool for shared use across threads.
    /// </summary>
    public static readonly ByteBufferPool Global = new ByteBufferPool();

    /// <summary>
    /// A thread-local instance of the ByteBufferPool for isolated use within a single thread.
    /// </summary>
    [ThreadStatic]
    public static ByteBufferPool Local;

    /// <summary>
    /// Adds a ByteBuffer to the pool. The buffer is added at the head of the list.
    /// </summary>
    /// <param name="buffer">The ByteBuffer to add to the pool.</param>
    public void Add(ByteBuffer buffer)
    {
        buffer.Next = Head;

        if (Tail == null)
            Tail = buffer;

        Head = buffer;
    }

    /// <summary>
    /// Clears the pool, returning the current head buffer and setting the pool to empty.
    /// </summary>
    /// <returns>The head ByteBuffer that was in the pool before it was cleared.</returns>
    public ByteBuffer Clear()
    {
        ByteBuffer result = Head;

        Head = null;
        Tail = null;

        return result;
    }

    /// <summary>
    /// Takes a ByteBuffer from the pool. If the pool is empty, returns null.
    /// </summary>
    /// <returns>The ByteBuffer from the head of the pool, or null if the pool is empty.</returns>
    public ByteBuffer Take()
    {
        if (Head == null)
        {
            return null;
        }
        else
        {
            ByteBuffer result = Head;

            if (Head == Tail)
            {
                Head = null;
                Tail = null;
            }
            else
            {
                Head = Head.Next;
            }

            return result;
        }
    }

    /// <summary>
    /// Gets the current length of the pool, representing the number of ByteBuffer objects it contains.
    /// </summary>
    public int Length
    {
        get
        {
            int val = 0;

            ByteBuffer current = Head;

            while (current != null)
            {
                current = current.Next;
                ++val;
            }

            return val;
        }
    }

    /// <summary>
    /// Merges another ByteBufferPool into the current pool. The other pool is emptied after the merge.
    /// </summary>
    /// <param name="other">The other ByteBufferPool to merge into this pool.</param>
    public void Merge(ByteBufferPool other)
    {
        if (Head == null)
        {
            Head = other.Head;
            Tail = other.Tail;
        }
        else if (other.Head != null)
        {
            Tail.Next = other.Head;

            Tail = other.Tail;
        }

        other.Head = null;
        other.Tail = null;
    }
}
