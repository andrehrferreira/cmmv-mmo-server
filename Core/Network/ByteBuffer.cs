/*
 * ByteBuffer
 * 
 * Author: Diego Guedes
 * Modified by: Andre Ferreira
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

using System.Text;
using System.Runtime.CompilerServices;

public class ByteBuffer : IDisposable
{
    private int _referenceCount = 1;

    private byte[] Buffer;
    public int Position;
    private bool Disposed = false;

    public volatile bool IsDestroyed;

    public ByteBuffer Next;

    public ByteBuffer(int initialSize = 0)
    {
        Buffer = new byte[initialSize];
        Position = 0;
    }

    public ByteBuffer(byte[] data)
    {
        Buffer = data ?? throw new ArgumentNullException(nameof(data));
        Position = 0;
    }

    public ByteBuffer(ByteBuffer other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Buffer = other.Buffer;
        Position = 0;
        other.Retain();
    }

    public void Reset()
    {
        Position = 0;
        Next = null;
    }

    ~ByteBuffer()
    {
        if(Buffer != null)
        {
            Buffer = null;
            IsDestroyed = true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Retain()
    {
        _referenceCount++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetType(byte type)
    {
        EnsureCapacity(1);

        if (Position > 0)
            Array.Copy(Buffer, 0, Buffer, 1, Position);

        Buffer[0] = type;
        Position++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCapacity(int requiredBytes)
    {
        int requiredCapacity = Position + requiredBytes;

        if (Buffer == null || requiredCapacity > Buffer.Length)
        {
            int newSize = Buffer == null ? requiredCapacity : Math.Max(Buffer.Length * 2, requiredCapacity);
            Buffer = Buffer == null ? new byte[newSize] : ResizeBuffer(Buffer, newSize);
        }
    }

    private byte[] ResizeBuffer(byte[] buffer, int newSize)
    {
        byte[] newBuffer = new byte[newSize];

        if (buffer != null)        
            Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
        
        return newBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetPosition()
    {
        Position = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer CreateEmptyBuffer()
    {
        return ConcurrentByteBufferPool.Acquire();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] GetBuffer()
    {
        if (Disposed)
            throw new ObjectDisposedException("ByteBuffer");

        return Buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (!Disposed)
        {
            _referenceCount--;
            if (_referenceCount <= 0)
            {
                Disposed = true;
                Buffer = null;
                ConcurrentByteBufferPool.Release(this);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToHex()
    {
        if (Disposed)
            throw new ObjectDisposedException("ByteBuffer");

        uint hash = 0;

        for (int i = 0; i < Position; i++)        
            hash = (hash << 5) + hash + Buffer[i]; 
        
        return hash.ToString("X8");
    }

    // Write methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(byte value)
    {
        EnsureCapacity(1);
        Buffer[Position++] = value;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(int value)
    {
        EnsureCapacity(4);
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, Buffer, Position, bytes.Length);
        Position += bytes.Length;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(string value)
    {
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(value);
        Write(utf8Bytes.Length);
        EnsureCapacity(utf8Bytes.Length);
        Array.Copy(utf8Bytes, 0, Buffer, Position, utf8Bytes.Length);
        Position += utf8Bytes.Length;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(bool value)
    {
        return Write((byte)(value ? 1 : 0));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(float value)
    {
        EnsureCapacity(4);
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, Buffer, Position, bytes.Length);
        Position += bytes.Length;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(Vector3 value)
    {
        Write((int)value.X);
        Write((int)value.Y);
        Write((int)value.Z);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(Rotator value)
    {
        Write((int)value.Roll);
        Write((int)value.Pitch);
        Write((int)value.Yaw);
        return this;
    }

    // Read methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>()
    {
        if (typeof(T) == typeof(byte))
            return (T)(object)ReadByte();
        else if (typeof(T) == typeof(int))
            return (T)(object)ReadInt();
        else if (typeof(T) == typeof(float))
            return (T)(object)ReadFloat();
        else if (typeof(T) == typeof(bool))
            return (T)(object)ReadBool();
        else if (typeof(T) == typeof(string))
            return (T)(object)ReadString();
        else if (typeof(T) == typeof(Vector3))
            return (T)(object)ReadVector3();
        else if (typeof(T) == typeof(Rotator))
            return (T)(object)ReadRotator();
        else if (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(byte))
            return (T)(object)ReadByte();
        else
            throw new NotSupportedException($"Type '{typeof(T)}' is not supported.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        if (Position + 1 > Buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        return Buffer[Position++];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool()
    {
        return ReadByte() != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt()
    {
        if (Position + 4 > Buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        int value = BitConverter.ToInt32(Buffer, Position);
        Position += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {
        int length = ReadInt();

        if (Position + length > Buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        string value = Encoding.UTF8.GetString(Buffer, Position, length);
        Position += length;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat()
    {
        if (Position + 4 > Buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        float value = BitConverter.ToSingle(Buffer, Position);
        Position += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ReadVector3()
    {
        float x = (float)ReadInt();
        float y = (float)ReadInt();
        float z = (float)ReadInt();
        return new Vector3(x, y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadId()
    {
        int id = ReadInt();
        return Base36.ToString(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rotator ReadRotator()
    {
        float roll = (float)ReadInt();
        float pitch = (float)ReadInt();
        float yaw = (float)ReadInt();
        return new Rotator(roll, pitch, yaw);
    }
}
