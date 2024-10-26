// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct OpenContainerPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(OpenContainerDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.ContainerType);
        buffer.Write(Base36.ToInt(data.EntityId));
        buffer.Write(data.ContainerId);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OpenContainerDTO Deserialize(ByteBuffer buffer)
    {
        var data = new OpenContainerDTO();
        data.ContainerType = buffer.ReadByte();
        data.EntityId = buffer.ReadId();
        data.ContainerId = buffer.ReadString();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, OpenContainerDTO data)
    {
        var buffer = Serialize(data);
        owner.Conn.Send(ServerPacket.OpenContainer, buffer, true);
    }

}

