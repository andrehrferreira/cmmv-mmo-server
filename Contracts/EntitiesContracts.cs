

[Contract("CreateEntity", PacketType.Server, PacketAction.None, true)]
public struct CreateEntity
{
    [ContractField("int")]
    public int Id;

    [ContractField("string")]
    public string Name;

    [ContractField("string")]
    public string Visual;

    [ContractField("int")]
    public int States;

    [ContractField("int")]
    public int Buffs;

    [ContractField("vector3")]
    public Vector3 Position;

    [ContractField("int")]
    public int MaxLife;

    [ContractField("int")]
    public int Life;

    [ContractField("int")]
    public int Speed;

    [ContractField("string")]
    public string Guild;
}

[Contract("UpdateEntity", PacketType.Server, PacketAction.None, true)]
public struct UpdateEntity
{
    [ContractField("int")]
    public int Id;

    [ContractField("int")]
    public int States;

    [ContractField("int")]
    public int Buffs;

    [ContractField("vector3")]
    public Vector3 Position;

    [ContractField("int")]
    public int Life;
}

[Contract("RemoveEntity", PacketType.Server, PacketAction.None, true)]
public struct RemoveEntity
{
    [ContractField("int")]
    public int Id;
}

[Contract("SyncEvent", PacketType.Multiplex, PacketAction.AreaOfInterest, true)]
public struct SyncEvent
{
    [ContractField("int")]
    public int Id;

    [ContractField("byte")]
    public byte EventId;
}

[Contract("PlayMontage", PacketType.Multiplex, PacketAction.AreaOfInterest, true)]
public struct PlayMontage
{
    [ContractField("int")]
    public int Id;

    [ContractField("int")]
    public int Index;
}