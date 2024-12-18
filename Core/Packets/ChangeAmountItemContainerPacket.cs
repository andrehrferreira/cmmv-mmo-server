// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct ChangeAmountItemContainerPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(ChangeAmountItemContainerDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.ChangeAmountItemContainer);
        buffer.Write(data.ContainerId);
        buffer.Write(data.SlotId);
        buffer.Write(data.Amount);
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, ChangeAmountItemContainerDTO data)
    {
        var buffer = Serialize(data);
        owner.Conn.Send(ServerPacket.ChangeAmountItemContainer, buffer, true);
    }
}

