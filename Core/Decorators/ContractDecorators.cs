
[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public class ContractAttribute: Attribute
{
    public string Name {  get; }
    public PacketType Type { get; }
    public PacketAction Action { get; }
    public bool Queue { get; set; }
    public PacketSendType SendType { get; set; }
    public bool EncryptedData { get; set; }

    public ContractAttribute(
        string name, PacketType type, PacketAction action = PacketAction.None, 
        bool queue = false, PacketSendType sendType = PacketSendType.Self,
        bool encryptedData = false
    )
    {
        Name = name;
        Type = type;
        Action = action;
        Queue = queue;
        SendType = sendType;
        EncryptedData = encryptedData;
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public class ContractFieldAttribute : Attribute
{
    public string Type { get; }

    public FieldReplyType ReplyType { get; }

    public ContractFieldAttribute(
        string type, FieldReplyType replyType = FieldReplyType.Mutiplex
    )
    {
        Type = type;
        ReplyType = replyType;
    }
}