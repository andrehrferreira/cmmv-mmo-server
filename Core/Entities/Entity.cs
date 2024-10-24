

public partial class Entity 
{
    public Socket Socket { get; set; }

    [Reply(ReplyType.AreaOfInterest, ReplyDataType.Index, ReplyTransformData.Base36)]
    public string Id { get; set; }

    [Reply(ReplyType.AreaOfInterest, ReplyDataType.Immutable)]
    public string Name { get; set; }

    // Character info
    public List<Entity> AreaOfInterest = new List<Entity>();


    //Network
    public void Reply(ServerPacket packetType, ByteBuffer data, bool Queue = false)
    {
        data.SetType((byte)packetType);

        AreaOfInterest.ForEach((entity) =>
        {
            if(entity.Socket != null)
            {
                if (Queue)
                    QueueBuffer.AddBuffer(entity.Socket.Id, data);
                else
                    entity.Socket.Send(data.GetBuffer());
            }
        });
    }
}