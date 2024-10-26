/*
 * UDP
 *
 * Author: Andre Ferreira
 *
 * Copyright (c) Uzmi Games. Licensed under the MIT License.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#include "UDP.h"
#include "Async/Async.h"
#include "SocketSubsystem.h"
#include "Kismet/KismetSystemLibrary.h"
#include "ByteBuffer.h"
#include "Encryption.h"

void UUDP::LogByteArray(const TArray<uint8>& ByteArray)
{
    FString HexString;

    for (uint8 Byte : ByteArray)
        HexString += FString::Printf(TEXT("%02X "), Byte);

    UE_LOG(LogTemp, Warning, TEXT("Byte Array: %s"), *HexString);
}

void UUDP::InitUDP(FString host, int32 port) {
    Host = host;
    Port = port;
}

void UUDP::Connect()
{
    RemoteAdress = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM)->CreateInternetAddr();

    if (!RemoteAdress) {
        OnConnectionError.Broadcast(FString(TEXT("Failed to create RemoteAddress")));
        return;
    }

    bool bIsValid;
    RemoteAdress->SetIp(*Host, bIsValid);
    RemoteAdress->SetPort(Port);

    if (!bIsValid) {
        OnConnectionError.Broadcast(FString(TEXT("UDP address is invalid")));
        return;
    }

    int BufferSize = 512 * 1024;
    Socket = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM)->CreateSocket(NAME_DGram, TEXT("UDPClient"), false);

    if (!Socket) {
        OnConnectionError.Broadcast(FString(TEXT("Failed to create UDP socket.")));
        return;
    }

    Socket->SetReuseAddr(true);
    Socket->SetNonBlocking(true);
    Socket->SetBroadcast(true);
    Socket->SetReceiveBufferSize(BufferSize, BufferSize);
    Socket->SetSendBufferSize(BufferSize, BufferSize);

    FTimespan ThreadWaitTime = FTimespan::FromMilliseconds(100);
    FString ThreadName = FString::Printf(TEXT("UDP RECEIVER"));
    UE_LOG(LogTemp, Warning, TEXT("Create UDPReceiver"));

    UDPReceiver = new FUdpSocketReceiver(Socket, ThreadWaitTime, *ThreadName);

    UDPReceiver->OnDataReceived().BindLambda([this](const FArrayReaderPtr& DataPtr, const FIPv4Endpoint& Endpoint)
        {
            TArray<uint8> Data;
            Data.AddUninitialized(DataPtr->TotalSize());
            DataPtr->Serialize(Data.GetData(), DataPtr->TotalSize());

            FString SenderIp = Endpoint.Address.ToString();
            int32 SenderPort = Endpoint.Port;

            AsyncTask(ENamedThreads::GameThread, [this, Data, SenderIp, SenderPort]() {
                //LogByteArray(Data);
                UByteBuffer* Buffer = UByteBuffer::CreateByteBuffer(Data);
                OnMessageReceived.Broadcast(Buffer);
                });

            LastPacketTime = FDateTime::UtcNow();
        });

    UDPReceiver->Start();
    UE_LOG(LogTemp, Warning, TEXT("Connected to %s:%d"), *Host, Port);
    OnConnected.Broadcast();
}

void UUDP::Disconnect()
{
    if (Socket)
    {
        Socket->Close();

        if (UDPReceiver)
        {
            UDPReceiver->Stop();
            delete UDPReceiver;
            UDPReceiver = nullptr;
        }

        ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM)->DestroySocket(Socket);
        Socket = nullptr;

        UE_LOG(LogTemp, Warning, TEXT("Disconnected from UDP socket."));
    }

    OnDisconnected.Broadcast();
}

bool UUDP::ValidateConnection()
{
    return Socket && Socket->GetConnectionState() == SCS_Connected && ((FDateTime::UtcNow() - LastPacketTime).GetTotalSeconds() < 3);
}

void UUDP::SendMessage(uint8 PacketType, UByteBuffer* Message)
{
    if (!Socket || !IsConnected()) return;

    const TArray<uint8>& OriginalData = Message->GetBuffer();
    TArray<uint8> NewData;

    NewData.Reserve(1 + OriginalData.Num());
    NewData.Add(PacketType);
    NewData.Append(OriginalData);

    int32 BytesSent = 0;
    Socket->SendTo(NewData.GetData(), NewData.Num(), BytesSent, *RemoteAdress);
}

void UUDP::SendEncryptedMessage(uint8 PacketType, UByteBuffer* Message, const FString& Key)
{
    if (!Socket || !IsConnected()) return;

    const TArray<uint8>& OriginalData = Message->GetBuffer();
    TArray<uint8> NewData;

    NewData.Reserve(1 + OriginalData.Num());
    NewData.Add(PacketType);
    NewData.Append(OriginalData);

    const TArray<uint8>& EncryptedData = UEncryption::EncryptBuffer(NewData, Key);
    int32 BytesSent = 0;
    Socket->SendTo(EncryptedData.GetData(), EncryptedData.Num(), BytesSent, *RemoteAdress);
}

bool UUDP::IsConnected() const
{
    return Socket && Socket->GetConnectionState() == SCS_Connected;
}

UUDP* UUDPFunctionLibrary::CreateSocket(FString Host, int32 Port)
{
    UUDP* const WrapperSocket = NewObject<UUDP>();
    WrapperSocket->InitUDP(Host, Port);
    return WrapperSocket;
}

int32 UUDPFunctionLibrary::GetTimeInMilliseconds()
{
    FDateTime Now = FDateTime::Now();
    FTimespan Timespan = Now.GetTimeOfDay();
    int64 Milliseconds = Timespan.GetTotalMilliseconds();
    return Milliseconds;
}
