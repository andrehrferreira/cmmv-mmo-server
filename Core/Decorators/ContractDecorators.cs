
[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public class ContractAttribute: Attribute
{
    public string Name {  get; }
    public PacketType Type { get; }
    public PacketAction Action { get; }
    public bool Queue { get; set; }

    public ContractAttribute(string name, PacketType type, PacketAction action = PacketAction.None, bool queue = false)
    {
        Name = name;
        Type = type;
        Action = action;
        Queue = queue;
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public class ContractFieldAttribute : Attribute
{
    public string Type { get; }

    public ContractFieldAttribute(string type)
    {
        Type = type;
    }
}