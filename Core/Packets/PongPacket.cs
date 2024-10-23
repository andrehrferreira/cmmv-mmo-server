// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct PongPacket
{
    public ServerPacket Type = ServerPacket.Pong;

    public PongPacket() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(Pong data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Timestamp);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Pong Deserialize(ByteBuffer buffer)
    {
        var data = new Pong();
        data.Timestamp = buffer.ReadInt();
        return data;
    }
}

