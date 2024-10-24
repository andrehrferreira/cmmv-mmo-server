// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct SyncEventPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(SyncEventDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(data.EventId);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyncEventDTO Deserialize(ByteBuffer buffer)
    {
        var data = new SyncEventDTO();
        data.Id = buffer.ReadId();
        data.EventId = buffer.ReadByte();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, SyncEventDTO data)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.SyncEvent, buffer, true);
    }

}

public partial class Server
{
    public static NetworkEvents<SyncEventDTO> OnSyncEvent = new NetworkEvents<SyncEventDTO>();

    [Subscribe(ClientPacket.SyncEvent)]
    public static void OnSyncEventHandler(SyncEventDTO data, Socket socket)
    {
        var packet = SyncEventPacket.Serialize(data);
        socket.Entity.Reply(ServerPacket.SyncEvent, packet, true);
    }
}
