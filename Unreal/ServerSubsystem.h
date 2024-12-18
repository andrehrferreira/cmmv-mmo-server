#pragma once

#include "CoreMinimal.h"
#include "Enums/ClientPacket.h"
#include "Enums/ServerPacket.h"
#include "Engine/GameInstance.h"
#include "Websocket.h"
#include "UDP.h"
//%INCLUDES%
#include "ServerSubsystem.generated.h"

UCLASS(DisplayName = "ServerSubsystem")
class CLIENT_API UServerSubsystem : public UGameInstance
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintCallable, BlueprintPure, Category = "Game Data", meta = (WorldContext = "WorldContextObject"))
	static UServerSubsystem* GetServerSubsystem(UObject* WorldContextObject);

	UFUNCTION(BlueprintCallable, Category = "ServerSubsystem")
	void Start();

	DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FConnected, FString, ClientID);
	DECLARE_DYNAMIC_MULTICAST_DELEGATE(FDisconnected);
//%DELEGATES%

	UPROPERTY(BlueprintAssignable, meta = (DisplayName = "OnConnect", Keywords = ""), Category = "ServerSubsystem")
	FConnected OnConnect;

	UPROPERTY(BlueprintAssignable, meta = (DisplayName = "OnDisconnect", Keywords = ""), Category = "ServerSubsystem")
	FDisconnected OnDisconnect;

//%EVENTS%

	virtual void Init() override;

	virtual void Shutdown() override;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UDP Server Configuration")
	FString UDPServerHost = TEXT("127.0.0.1");

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "UDP Server Configuration")
	int32 UDPServerPort = 3020;

	UPROPERTY(BlueprintReadOnly, Category = "UDP Server")
	UUDP* UDPInstance;

private:
	FString ECCPublicKey;
	FTimerHandle PingTimerHandle;

	void StartPingTimer();
	void SendPeriodicPing();
	void HandleReceivedData(UByteBuffer* ReceivedBuffer);
}; 