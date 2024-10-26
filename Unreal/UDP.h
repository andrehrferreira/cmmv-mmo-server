#pragma once

#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "Modules/ModuleManager.h"
#include "IPAddress.h"
#include "SocketSubsystem.h"
#include "Common/UdpSocketBuilder.h"
#include "Common/UdpSocketReceiver.h"
#include "Common/UdpSocketSender.h"
#include "UDP.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnUDPConnected);
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnUDPDisconnected);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnUDPConnectionError, FString, Message);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnUDPMessageReceived, UByteBuffer*, Data);

UCLASS(MinimalAPI, BlueprintType)
class UUDP final : public UObject
{
	GENERATED_BODY()

public:
	UPROPERTY(BlueprintAssignable)
	FOnUDPConnected OnConnected;

	UPROPERTY(BlueprintAssignable)
	FOnUDPConnectionError OnConnectionError;

	UPROPERTY(BlueprintAssignable)
	FOnUDPDisconnected OnDisconnected;

	UPROPERTY(BlueprintAssignable)
	FOnUDPMessageReceived OnMessageReceived;

	void InitUDP(FString Host, int32 Port);

	UFUNCTION(BlueprintCallable, Category = "UDP")
	void Connect();

	UFUNCTION(BlueprintCallable, Category = "UDP")
	void Disconnect();

	UFUNCTION(BlueprintPure, Category = "UDP")
	bool IsConnected() const;

	UFUNCTION(BlueprintCallable, Category = "UDP")
	void SendMessage(uint8 PacketType, UByteBuffer* Message);

	UFUNCTION(BlueprintCallable, Category = "UDP")
	void SendEncryptedMessage(uint8 PacketType, UByteBuffer* Message, const FString& Key);

protected:
	FString Host;

	int32 Port;

	FTimerHandle ConnectionValidationTimer;

	FDateTime LastPacketTime;

	FSocket* Socket;

	FUdpSocketReceiver* UDPReceiver;

	TSharedPtr<FInternetAddr> RemoteAdress;

	ISocketSubsystem* SocketSubsystem;

	bool ValidateConnection();

	void LogByteArray(const TArray<uint8>& ByteArray);
};

UCLASS(MinimalAPI)
class UUDPFunctionLibrary final : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintCallable, Category = "UDP")
	static UUDP* CreateSocket(FString Host, int32 Port);

	UFUNCTION(BlueprintCallable, Category = "UDP")
	static int32 GetTimeInMilliseconds();
};