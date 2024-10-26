/*
 * Server Subsystem
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

#include "ServerSubsystem.h"
#include "Kismet/GameplayStatics.h"
//%INCLUDES%

UServerSubsystem* UServerSubsystem::GetServerSubsystem(UObject* WorldContextObject)
{
    return WorldContextObject ? Cast<UServerSubsystem>(UGameplayStatics::GetGameInstance(WorldContextObject)) : nullptr;
}

void UServerSubsystem::Init() {
	Super::Init();
	UE_LOG(LogTemp, Warning, TEXT("ServerSubsystem Init called."));
	Start();
}

void UServerSubsystem::Start()
{
	UE_LOG(LogTemp, Warning, TEXT("ServerSubsystem Start called. Connecting to UDP: %s:%d"), *UDPServerHost, UDPServerPort);

	UDPInstance = UUDPFunctionLibrary::CreateSocket(UDPServerHost, UDPServerPort);

	if (UDPInstance)
	{
		UDPInstance->OnMessageReceived.AddDynamic(this, &UServerSubsystem::HandleReceivedData);
		UDPInstance->Connect();
	}
	else {
		UE_LOG(LogTemp, Error, TEXT("Invalid UDP Instance"));
	}

	StartPingTimer();
}

void UServerSubsystem::StartPingTimer()
{
	GetWorld()->GetTimerManager().SetTimer(PingTimerHandle, this, &UServerSubsystem::SendPeriodicPing, 3.0f, true);
}

void UServerSubsystem::SendPeriodicPing()
{

	FDateTime Now = FDateTime::Now();
	FTimespan Timespan = Now.GetTimeOfDay();
	int64 Milliseconds = Timespan.GetTotalMilliseconds();

	FPing PingData = FPing();
	PingData.Timestamp = (int32)Milliseconds;

	SendPing(PingData);
}

void UServerSubsystem::Shutdown()
{
	if (UDPInstance)
	{
		UDPInstance->Disconnect();
		UDPInstance = nullptr;
	}

	Super::Shutdown();
}

void UServerSubsystem::HandleReceivedData(UByteBuffer* ReceivedBuffer)
{
	if (ReceivedBuffer)
	{
		uint8 PacketId = ReceivedBuffer->GetByte();
		EServerPacket PacketType = static_cast<EServerPacket>(PacketId);

		switch(PacketType) {
//%PARSEDDATASWITCH%
		}
	}
}

//%FUNCTIONS%