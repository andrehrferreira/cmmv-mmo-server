// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct CancelTargetPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(CancelTargetDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.CancelTarget);
        buffer.Write(Base36.ToInt(data.Id));
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CancelTargetDTO Deserialize(ByteBuffer buffer)
    {
        var data = new CancelTargetDTO();
        data.Id = buffer.ReadId();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, CancelTargetDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.CancelTarget, buffer, true, true);
    }
}

public partial class Server
{
    public static NetworkEvents<CancelTargetDTO> OnCancelTarget = new NetworkEvents<CancelTargetDTO>();

    [Subscribe(ClientPacket.CancelTarget)]
    public static void OnCancelTargetHandler(CancelTargetDTO data, Connection conn)
    {
        var packet = CancelTargetPacket.Serialize(data);
        conn.Entity.Reply(ServerPacket.CancelTarget, packet, true);
    }
}
