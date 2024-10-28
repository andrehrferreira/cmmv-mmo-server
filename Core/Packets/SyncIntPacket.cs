// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct SyncIntPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(SyncInt data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.SyncInt);
        buffer.Write(data.Model);
        buffer.Write(data.Index);
        buffer.Write(data.Value);
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, SyncInt data)
    {
        var buffer = Serialize(data);
        QueueBuffer.AddBuffer(ServerPacket.SyncInt, owner.Conn.Id, buffer);
    }
}

