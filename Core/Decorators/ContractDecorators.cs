
[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public class ContractAttribute: Attribute
{
    public string Name {  get; }
    public PacketType Type { get; }

    public ContractAttribute(string name, PacketType type)
    {
        Name = name;
        Type = type;
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