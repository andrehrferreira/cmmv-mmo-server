// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct CloseContainerPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(CloseContainerDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.CloseContainer);
        buffer.Write(data.ContainerType);
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, CloseContainerDTO data)
    {
        var buffer = Serialize(data);
        owner.Conn.Send(ServerPacket.CloseContainer, buffer, true);
    }
}

