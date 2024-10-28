// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct SyncStringPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(SyncString data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.SyncString);
        buffer.Write(data.Model);
        buffer.Write(data.Index);
        buffer.Write(data.Value);
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, SyncString data)
    {
        var buffer = Serialize(data);
        QueueBuffer.AddBuffer(ServerPacket.SyncString, owner.Conn.Id, buffer);
    }
}

