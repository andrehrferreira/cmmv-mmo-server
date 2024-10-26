public enum ReplyType : byte
{
    OwnerOnly = 0,
    AreaOfInterest = 1,
    Broadcast = 2
}

public enum ReplyDataType: byte
{
    Index = 0,
    Immutable = 1,
    Variable = 2
}

public enum ReplyTransformData: byte
{
    Literal = 0,
    Base36 = 1,
    Base64 = 2,
    JSON = 3,
    Byte = 4,
    Flag = 5
}