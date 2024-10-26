// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct AddItemContainerPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(AddItemContainerDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.ContainerId);
        buffer.Write(data.SlotId);
        buffer.Write(data.ItemRef);
        buffer.Write(data.ItemName);
        buffer.Write(data.Amount);
        buffer.Write(data.ItemRarity);
        buffer.Write(data.GoldCost);
        buffer.Write(data.Weight);
        buffer.Write(data.ShowHint);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AddItemContainerDTO Deserialize(ByteBuffer buffer)
    {
        var data = new AddItemContainerDTO();
        data.ContainerId = buffer.ReadString();
        data.SlotId = buffer.ReadInt();
        data.ItemRef = buffer.ReadString();
        data.ItemName = buffer.ReadString();
        data.Amount = buffer.ReadInt();
        data.ItemRarity = buffer.ReadByte();
        data.GoldCost = buffer.ReadInt();
        data.Weight = buffer.ReadInt();
        data.ShowHint = buffer.ReadBool();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, AddItemContainerDTO data)
    {
        var buffer = Serialize(data);
        owner.Conn.Send(ServerPacket.AddItemContainer, buffer, true);
    }

}

