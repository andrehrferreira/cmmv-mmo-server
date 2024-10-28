// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct SyncBytePacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(SyncByte data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.SyncByte);
        buffer.Write(data.Model);
        buffer.Write(data.Index);
        buffer.Write(data.Value);
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, SyncByte data)
    {
        var buffer = Serialize(data);
        QueueBuffer.AddBuffer(ServerPacket.SyncByte, owner.Conn.Id, buffer);
    }
}

