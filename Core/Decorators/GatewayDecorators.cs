[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class SubscribeAttribute : Attribute
{
    public ClientPacket Type { get; }

    public SubscribeAttribute(ClientPacket type)
    {
        Type = type;
    }
}