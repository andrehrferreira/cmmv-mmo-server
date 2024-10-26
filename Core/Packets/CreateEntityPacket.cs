// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct CreateEntityPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(CreateEntityDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
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
    public static CreateEntityDTO Deserialize(ByteBuffer buffer)
    {
        var data = new CreateEntityDTO();
        data.Id = buffer.ReadId();
        data.Name = buffer.ReadString();
        data.Visual = buffer.ReadString();
        data.States = buffer.ReadInt();
        data.Buffs = buffer.ReadInt();
        data.Position = buffer.ReadVector3();
        data.MaxLife = buffer.ReadInt();
        data.Life = buffer.ReadInt();
        data.Speed = buffer.ReadInt();
        data.Guild = buffer.ReadString();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, CreateEntityDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        QueueBuffer.AddBuffer(ServerPacket.CreateEntity, entity.Conn.Id, buffer);
    }

}

