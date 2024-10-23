

using System.Text;

public struct ByteBuffer
{
    private byte[] buffer;
    private int position;

    public ByteBuffer(byte[] data = null)
    {
        buffer = data ?? new byte[0];
        position = 0;
    }

    private void EnsureCapacity(int requiredBytes)
    {
        int requiredCapacity = position + requiredBytes;

        if (requiredCapacity > buffer.Length)
            Array.Resize(ref buffer, requiredCapacity);
    }

    public static ByteBuffer CreateEmptyBuffer()
    {
        return new ByteBuffer();
    }

    public byte[] GetBuffer()
    {
        return buffer;
    }

    // Write methods
    public ByteBuffer Write(byte value)
    {
        EnsureCapacity(1);
        buffer[position++] = value;
        return this;
    }

    public ByteBuffer Write<T>(T value)
    {
        if (typeof(T) == typeof(byte))
            Write(Convert.ToByte(value));        
        else if (typeof(T) == typeof(int))
            Write(Convert.ToInt32(value));       
        else if (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(byte))        
            Write(Convert.ToByte(value));        
        else        
            throw new NotSupportedException($"Type '{typeof(T)}' is not supported.");
        
        return this;
    }

    public ByteBuffer Write(int value)
    {
        EnsureCapacity(4);
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, buffer, position, bytes.Length);
        position += bytes.Length;
        return this;
    }

    public ByteBuffer Write(string value)
    {
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(value);
        Write(utf8Bytes.Length);
        EnsureCapacity(utf8Bytes.Length);
        Array.Copy(utf8Bytes, 0, buffer, position, utf8Bytes.Length);
        position += utf8Bytes.Length;
        return this;
    }

    public ByteBuffer Write(bool value)
    {
        return Write((byte)(value ? 1 : 0));
    }

    public ByteBuffer Write(float value)
    {
        EnsureCapacity(4);
        BitConverter.GetBytes(value).CopyTo(buffer, position);
        position += 4;
        return this;
    }

    //Read methods

    public T Read<T>()
    {
        if (typeof(T) == typeof(byte))
            return (T)(object)ReadByte();
        else if (typeof(T) == typeof(int))        
            return (T)(object)ReadInt();
        else if (typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == typeof(byte))        
            return (T)(object)ReadByte();        
        else
            throw new NotSupportedException($"Type '{typeof(T)}' is not supported.");
    }

    public byte ReadByte()
    {
        if (position + 1 > buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        return buffer[position++];
    }

    public bool ReadBool()
    {
        return ReadByte() != 0;
    }

    public int ReadInt()
    {
        if (position + 4 > buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        int value = BitConverter.ToInt32(buffer, position);
        position += 4;
        return value;
    }

    public string ReadString()
    {
        int length = ReadInt();

        if (position + length > buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        string value = Encoding.UTF8.GetString(buffer, position, length);
        position += length;
        return value;
    }

    public float ReadFloat()
    {
        if (position + 4 > buffer.Length)
            throw new InvalidOperationException("Buffer underflow");

        float value = BitConverter.ToSingle(buffer, position);
        position += 4;
        return value;
    }
}