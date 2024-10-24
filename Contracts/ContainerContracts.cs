[Contract("OpenContainer", PacketType.Server)]
public struct OpenContainerDTO
{
    [ContractField("byte")]
    public byte ContainerType;

    [ContractField("id")]
    public string EntityId;

    [ContractField("string")]
    public string ContainerId;
}

[Contract("CloseContainer", PacketType.Server)]
public struct CloseContainerDTO
{
    [ContractField("byte")]
    public byte ContainerType;
}

[Contract("AddItemContainer", PacketType.Server)]
public struct AddItemContainerDTO
{
    [ContractField("string")]
    public string ContainerId;

    [ContractField("int")]
    public int SlotId;

    [ContractField("string")]
    public string ItemRef;

    [ContractField("string")]
    public string ItemName;

    [ContractField("int")]
    public int Amount;

    [ContractField("byte")]
    public byte ItemRarity;

    [ContractField("int")]
    public int GoldCost;

    [ContractField("int")]
    public int Weight;

    [ContractField("bool")]
    public bool ShowHint;
}

[Contract("RemoveItemContainer", PacketType.Server)]
public struct RemoveItemContainerDTO
{
    [ContractField("string")]
    public string ContainerId;

    [ContractField("int")]
    public int SlotId;

    [ContractField("string")]
    public string ItemRef;
}

[Contract("ChangeAmountItemContainer", PacketType.Server)]
public struct ChangeAmountItemContainerDTO
{
    [ContractField("string")]
    public string ContainerId;

    [ContractField("int")]
    public int SlotId;

    [ContractField("int")]
    public int Amount;
}