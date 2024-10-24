// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct PongPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(PongDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Timestamp);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PongDTO Deserialize(ByteBuffer buffer)
    {
        var data = new PongDTO();
        data.Timestamp = buffer.ReadInt();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, PongDTO data)
    {
        var buffer = Serialize(data);
        owner.Socket.Send(ServerPacket.Pong, buffer);
    }

}

