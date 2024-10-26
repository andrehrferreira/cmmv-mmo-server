// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct CloseContainerPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(CloseContainerDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.ContainerType);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CloseContainerDTO Deserialize(ByteBuffer buffer)
    {
        var data = new CloseContainerDTO();
        data.ContainerType = buffer.ReadByte();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, CloseContainerDTO data)
    {
        var buffer = Serialize(data);
        owner.Conn.Send(ServerPacket.CloseContainer, buffer, true);
    }

}

