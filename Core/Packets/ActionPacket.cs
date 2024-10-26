// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct ActionPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(ActionDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(data.Index);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ActionDTO Deserialize(ByteBuffer buffer)
    {
        var data = new ActionDTO();
        data.Id = buffer.ReadId();
        data.Index = buffer.ReadByte();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, ActionDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.Action, buffer, true, true);
    }

}

public partial class Server
{
    public static NetworkEvents<ActionDTO> OnAction = new NetworkEvents<ActionDTO>();

    [Subscribe(ClientPacket.Action)]
    public static void OnActionHandler(ActionDTO data, Connection conn)
    {
        var packet = ActionPacket.Serialize(data);
        conn.Entity.Reply(ServerPacket.Action, packet, true);
    }
}
