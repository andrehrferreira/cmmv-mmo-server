// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct SelectTargetPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(SelectTargetDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(Base36.ToInt(data.Target));
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectTargetDTO Deserialize(ByteBuffer buffer)
    {
        var data = new SelectTargetDTO();
        data.Id = buffer.ReadId();
        data.Target = buffer.ReadId();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, SelectTargetDTO data)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.SelectTarget, buffer, true);
    }

}

public partial class Server
{
    public static NetworkEvents<SelectTargetDTO> OnSelectTarget = new NetworkEvents<SelectTargetDTO>();

    [Subscribe(ClientPacket.SelectTarget)]
    public static void OnSelectTargetHandler(SelectTargetDTO data, Socket socket)
    {
        var packet = SelectTargetPacket.Serialize(data);
        socket.Entity.Reply(ServerPacket.SelectTarget, packet, true);
    }
}
