using System.Text;
using System.Runtime.CompilerServices;

public class ByteBuffer : IDisposable
{
    private byte[] buffer;
    private int position;
    private bool disposed = false;

    public ByteBuffer(int initialSize = 0)
    {
        buffer = new byte[initialSize];
        position = 0;
    }

    public ByteBuffer(byte[] data)
    {
        buffer = data ?? throw new ArgumentNullException(nameof(data));
        position = 0;
    }

    public ByteBuffer(ByteBuffer other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        buffer = new byte[other.buffer.Length];
        Array.Copy(other.buffer, buffer, other.buffer.Length);
        position = other.position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCapacity(int requiredBytes)
    {
        int requiredCapacity = position + requiredBytes;

        if (requiredCapacity > buffer.Length)
        {
            int newSize = Math.Max(buffer.Length * 2, requiredCapacity);
            Array.Resize(ref buffer, newSize);
        }
    }

    public void ResetPosition()
    {
        position = 0;
    }

    public static ByteBuffer CreateEmptyBuffer()
    {
        return new ByteBuffer();
    }

    public byte[] GetBuffer()
    {
        if (disposed)
            throw new ObjectDisposedException("ByteBuffer");

        byte[] actualBuffer = new byte[position];
        Array.Copy(buffer, actualBuffer, position);
        return actualBuffer;
    }

    public void Dispose()
    {
        if (!disposed)
        {
            buffer = null;
            disposed = true;
        }
    }

    // Write methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(byte value)
    {
        EnsureCapacity(1);
        buffer[position++] = value;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(int value)
    {
        EnsureCapacity(4);
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, buffer, position, bytes.Length);
        position += bytes.Length;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer Write(string value)
    {
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(value);
        Write(utf8Bytes.Length);
        EnsureCapacity(utf8Bytes.Length);
        Array.Copy(utf8Bytes, 0, buffer, position, utf8Bytes.Length);
        position += utf8Bytes.Length;
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
        Array.Copy(bytes, 0, buffer, position, bytes.Length);
        position += bytes.Length;
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
        else if (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(byte))
            return (T)(object)ReadByte();
        else
            throw new NotSupportedException($"Type '{typeof(T)}' is not supported.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        if (position + 1 > buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        return buffer[position++];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool()
    {
        return ReadByte() != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt()
    {
        if (position + 4 > buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        int value = BitConverter.ToInt32(buffer, position);
        position += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {
        int length = ReadInt();

        if (position + length > buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        string value = Encoding.UTF8.GetString(buffer, position, length);
        position += length;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat()
    {
        if (position + 4 > buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        float value = BitConverter.ToSingle(buffer, position);
        position += 4;
        return value;
    }
}
