// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct SyncEventPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(SyncEvent data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Id);
        buffer.Write(data.EventId);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyncEvent Deserialize(ByteBuffer buffer)
    {
        var data = new SyncEvent();
        data.Id = buffer.ReadInt();
        data.EventId = buffer.ReadByte();
        return data;
    }
}

public partial class Server
{
    public static NetworkEvents<SyncEvent> OnSyncEvent = new NetworkEvents<SyncEvent>();

    [Subscribe(ClientPacket.SyncEvent)]
    public static void OnSyncEventHandler(SyncEvent data, Socket socket)
    {
        var packet = SyncEventPacket.Serialize(data);
        socket.Entity.Reply(ServerPacket.SyncEvent, packet, true);
    }
}
