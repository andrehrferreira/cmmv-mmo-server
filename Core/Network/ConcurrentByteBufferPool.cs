/*
 * Concurrent ByteBuffer Pool
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
/// Provides a concurrent, thread-safe pool for managing and reusing instances of ByteBuffer.
/// This pool uses a global pool accessible by all threads and a local pool specific to each thread
/// to reduce contention and improve performance.
/// </summary>
public static class ConcurrentByteBufferPool
{
    // Global pool shared across all threads.
    static ByteBufferPool Global = new ByteBufferPool();

    // Local pool specific to each thread.
    [ThreadStatic]
    static ByteBufferPool Local;

    /// <summary>
    /// Acquires a ByteBuffer from the pool. If a buffer is available in the local pool,
    /// it is returned. If not, the global pool is checked. If no buffers are available,
    /// a new ByteBuffer instance is created.
    /// </summary>
    /// <returns>An instance of ByteBuffer from the pool or a new one if the pool is empty.</returns>
    public static ByteBuffer Acquire()
    {
        ByteBuffer buffer;

        // If the local pool is not initialized, fallback to the global pool.
        if (Local == null)
        {
            lock (Global)
            {
                buffer = Global.Take();
            }
        }
        else
        {
            // Attempt to take a buffer from the local pool.
            buffer = Local.Take();

            // If the local pool is empty, fallback to the global pool.
            if (buffer == null)
            {
                lock (Global)
                {
                    buffer = Global.Take();
                }
            }
        }

        // If no buffer is available, create a new instance.
        if (buffer == null)
        {
            buffer = new ByteBuffer();
        }

        return buffer;
    }

    /// <summary>
    /// Releases a ByteBuffer back to the pool. If the local pool is not initialized,
    /// it will be created. The buffer is reset and added back to the local pool.
    /// </summary>
    /// <param name="buffer">The ByteBuffer instance to release.</param>
    public static void Release(ByteBuffer buffer)
    {
        // Initialize the local pool if it is not already set.
        if (Local == null)
        {
            Local = new ByteBufferPool();
        }

        // Reset the buffer before adding it back to the pool.
        buffer.Reset();

        Local.Add(buffer);
    }

    /// <summary>
    /// Merges the local pool of the current thread into the global pool. This method is thread-safe
    /// and allows for consolidation of buffers when the local pool is no longer needed.
    /// </summary>
    public static void Merge()
    {
        // Only merge if the local pool contains buffers.
        if (Local != null && Local.Head != null)
        {
            lock (Global)
            {
                Global.Merge(Local);
            }
        }
    }

    /// <summary>
    /// Clears all buffers from the global pool. This operation is thread-safe and ensures
    /// no other thread is accessing the pool during the operation.
    /// </summary>
    /// <returns>The head of the ByteBuffer list from the global pool before it was cleared.</returns>
    public static ByteBuffer Clear()
    {
        lock (Global)
        {
            return Global.Clear();
        }
    }
}
