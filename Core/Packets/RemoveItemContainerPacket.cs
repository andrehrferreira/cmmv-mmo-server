// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct RemoveItemContainerPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(RemoveItemContainerDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.RemoveItemContainer);
        buffer.Write(data.ContainerId);
        buffer.Write(data.SlotId);
        buffer.Write(data.ItemRef);
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, RemoveItemContainerDTO data)
    {
        var buffer = Serialize(data);
        owner.Conn.Send(ServerPacket.RemoveItemContainer, buffer, true);
    }
}

