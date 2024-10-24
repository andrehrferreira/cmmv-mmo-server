// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct UpdateEntityPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(UpdateEntityDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(data.States);
        buffer.Write(data.Buffs);
        buffer.Write(data.Position);
        buffer.Write(data.Life);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UpdateEntityDTO Deserialize(ByteBuffer buffer)
    {
        var data = new UpdateEntityDTO();
        data.Id = buffer.ReadId();
        data.States = buffer.ReadInt();
        data.Buffs = buffer.ReadInt();
        data.Position = buffer.ReadVector3();
        data.Life = buffer.ReadInt();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, UpdateEntityDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        QueueBuffer.AddBuffer(ServerPacket.UpdateEntity, entity.Socket.Id, buffer);
    }

}

