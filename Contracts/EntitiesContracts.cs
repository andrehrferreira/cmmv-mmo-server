[Contract("CreateEntity", PacketType.Server, PacketAction.None, true, PacketSendType.ToEntity, true)]
public struct CreateEntityDTO
{
    [ContractField("id")]
    public string Id;

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

[Contract("UpdateEntity", PacketType.Multiplex, PacketAction.None, true, PacketSendType.ToEntity, true)]
public struct UpdateEntityDTO
{
    [ContractField("id")]
    public string Id;

    [ContractField("int", FieldReplyType.ServerOnly)]
    public int States;

    [ContractField("int", FieldReplyType.ServerOnly)]
    public int Buffs;

    [ContractField("vector3")]
    public Vector3 Position;

    [ContractField("int", FieldReplyType.ServerOnly)]
    public int Life;
}

[Contract("RemoveEntity", PacketType.Server, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct RemoveEntityDTO
{
    [ContractField("id")]
    public string Id;
}

[Contract("SyncEvent", PacketType.Multiplex, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct SyncEventDTO
{
    [ContractField("id")]
    public string Id;

    [ContractField("byte")]
    public byte EventId;
}

[Contract("PlayMontage", PacketType.Multiplex, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct PlayMontageDTO
{
    [ContractField("id")]
    public string Id;

    [ContractField("int")]
    public int Index;
}

[Contract("Action", PacketType.Multiplex, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct ActionDTO
{
    [ContractField("id")]
    public string Id;

    [ContractField("byte")]
    public byte Index;
}

[Contract("ActionArea", PacketType.Multiplex, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct ActionAreaDTO
{
    [ContractField("id")]
    public string Id;

    [ContractField("byte")]
    public byte Index;

    [ContractField("vector3")]
    public Vector3 Position;
}

[Contract("SelectTarget", PacketType.Multiplex, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct SelectTargetDTO
{
    [ContractField("id")]
    public string Id;

    [ContractField("id")]
    public string Target;
}

[Contract("CancelTarget", PacketType.Multiplex, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct CancelTargetDTO
{
    [ContractField("id")]
    public string Id;
}

[Contract("TakeDamage", PacketType.Server, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct TakeDamageDTO
{
    [ContractField("id")]
    public string Id;

    [ContractField("id")]
    public string CauserId;

    [ContractField("int")]
    public int Damage;

    [ContractField("byte")]
    public byte DamageType;
}

[Contract("TakeMiss", PacketType.Server, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct TakeMissDTO
{
    [ContractField("id")]
    public string Id;
}

[Contract("Heal", PacketType.Server, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct HealDTO
{
    [ContractField("id")]
    public string Id;

    [ContractField("byte")]
    public byte Type;

    [ContractField("int")]
    public int Value;

    [ContractField("id")]
    public string CasterId;
}

[Contract("Dissolve", PacketType.Server, PacketAction.AreaOfInterest, true, PacketSendType.ToEntity, true)]
public struct DissolveDTO
{
    [ContractField("id")]
    public string Id;
}