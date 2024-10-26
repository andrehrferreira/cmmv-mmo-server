// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct ActionAreaPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(ActionAreaDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(data.Index);
        buffer.Write(data.Position);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ActionAreaDTO Deserialize(ByteBuffer buffer)
    {
        var data = new ActionAreaDTO();
        data.Id = buffer.ReadId();
        data.Index = buffer.ReadByte();
        data.Position = buffer.ReadVector3();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, ActionAreaDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.ActionArea, buffer, true, true);
    }

}

public partial class Server
{
    public static NetworkEvents<ActionAreaDTO> OnActionArea = new NetworkEvents<ActionAreaDTO>();

    [Subscribe(ClientPacket.ActionArea)]
    public static void OnActionAreaHandler(ActionAreaDTO data, Connection conn)
    {
        var packet = ActionAreaPacket.Serialize(data);
        conn.Entity.Reply(ServerPacket.ActionArea, packet, true);
    }
}
