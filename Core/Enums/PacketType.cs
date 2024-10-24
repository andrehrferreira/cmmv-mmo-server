﻿public enum PacketType: byte
{
    Server = 0,
    Client = 1,
    Multiplex = 2
}

public enum PacketAction: byte
{
    None = 0,
    RunOnServer = 0,
    AreaOfInterest = 1,
    Broadcast = 2
}