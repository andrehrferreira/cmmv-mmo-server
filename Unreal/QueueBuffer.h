#pragma once

#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "ByteBuffer.h"
#include "Websocket.h"
#include "Modules/ModuleManager.h"

#include "QueueBuffer.generated.h"

USTRUCT(BlueprintType)
struct FQueueItem
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadWrite)
	uint8 PacketType;

	UPROPERTY(BlueprintReadWrite)
	UByteBuffer* Buffer;
};

UCLASS(MinimalAPI, BlueprintType)
class UQueueBuffer final : public UObject
{
	GENERATED_BODY()

private:
	TArray<FQueueItem> Queues;

	static const int32 MaxBufferSize = 512 * 1024;
	static const uint8 EndOfPacketByte = 0xFE;
	static const int32 EndRepeatByte = 4;

	bool IsDuplicatePacket(UByteBuffer* Buffer);
	void CheckAndSend();
	void SendBuffers();
	UByteBuffer* CombineBuffers(const TArray<FQueueItem>& Buffers);

public:
	UWebSocket* Socket;
	uint8 QueuePacketType;
	FString Key;

	UFUNCTION(BlueprintCallable, Category = "QueueBuffer")
	void AddBuffer(uint8 PacketType, UByteBuffer* Buffer);

	UFUNCTION(BlueprintCallable, Category = "QueueBuffer")
	void Tick();
};

UCLASS(MinimalAPI)
class UQueueBufferFunctionLibary final : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:
	UFUNCTION(BlueprintCallable, Category = "QueueBuffer")
	static UQueueBuffer* CreateInstance(UWebSocket* Socket, uint8 QueuePacketType, const FString& Key);
};