// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct CreateEntityPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(CreateEntityDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.CreateEntity);
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(data.Name);
        buffer.Write(data.Visual);
        buffer.Write(data.States);
        buffer.Write(data.Buffs);
        buffer.Write(data.Position);
        buffer.Write(data.MaxLife);
        buffer.Write(data.Life);
        buffer.Write(data.Speed);
        buffer.Write(data.Guild);
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, CreateEntityDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        QueueBuffer.AddBuffer(ServerPacket.CreateEntity, entity.Conn.Id, buffer);
    }
}

