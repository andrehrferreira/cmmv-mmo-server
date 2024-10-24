// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct RemoveEntityPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(RemoveEntity data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Id);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RemoveEntity Deserialize(ByteBuffer buffer)
    {
        var data = new RemoveEntity();
        data.Id = buffer.ReadInt();
        return data;
    }
}

