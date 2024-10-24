#pragma once

#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "ByteBuffer.h"
#include "Modules/ModuleManager.h"

#include "Websocket.generated.h"

class IWebSocket;

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnWebSocketConnected);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnWebSocketConnectionError, const FString&, Error);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_ThreeParams(FOnWebSocketClosed, int32, StatusCode, const FString&, Reason, bool, bWasClean);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnWebSocketMessageReceived, const FString&, Message);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnWebSocketBinaryMessageReceived, UByteBuffer*, Data);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnWebSocketMessageSent, const FString&, Message);

UCLASS(MinimalAPI, BlueprintType)
class UWebSocket final : public UObject
{
	GENERATED_BODY()

public:
	UPROPERTY(BlueprintAssignable)
	FOnWebSocketConnected OnWebSocketConnected;

	UPROPERTY(BlueprintAssignable)
	FOnWebSocketConnectionError OnWebSocketConnectionError;

	UPROPERTY(BlueprintAssignable)
	FOnWebSocketClosed OnWebSocketClosed;

	UPROPERTY(BlueprintAssignable)
	FOnWebSocketMessageReceived OnWebSocketMessageReceived;

	UPROPERTY(BlueprintAssignable)
	FOnWebSocketBinaryMessageReceived OnWebSocketBinaryMessageReceived;

	UPROPERTY(BlueprintAssignable)
	FOnWebSocketMessageSent OnWebSocketMessageSent;

	void InitWebSocket(TSharedPtr<IWebSocket> InWebSocket);

	UFUNCTION(BlueprintCallable, Category = "WebSockets")
	void Connect();

	UFUNCTION(BlueprintCallable, Category = "WebSockets")
	void Close(int32 StatusCode = 1000, const FString& Reason = TEXT(""));

	UFUNCTION(BlueprintPure, Category = "WebSockets")
	bool IsConnected() const;

	UFUNCTION(BlueprintCallable, Category = "WebSockets")
	void SendMessage(uint8 PacketType, UByteBuffer* Message);

	UFUNCTION(BlueprintCallable, Category = "WebSockets")
	void SendEncryptedMessage(uint8 PacketType, UByteBuffer* Message, const FString& Key);

private:

	UFUNCTION()
	void OnWebSocketConnected_Internal();

	UFUNCTION()
	void OnWebSocketConnectionError_Internal(const FString& Error);

	UFUNCTION()
	void OnWebSocketClosed_Internal(int32 StatusCode, const FString& Reason, bool bWasClean);

	UFUNCTION()
	void OnWebSocketMessageReceived_Internal(const FString& Message);

	void OnWebSocketBinaryMessageReceived_Internal(const void* Data, SIZE_T Size, bool bIsBinary);

	UFUNCTION()
	void OnWebSocketMessageSent_Internal(const FString& Message);

	TSharedPtr<IWebSocket> InternalWebSocket;
};


UCLASS(MinimalAPI)
class UWebSocketFunctionLibrary final : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintCallable, Category = "WebSockets")
	static UWebSocket* CreateWebSocket(FString ServerUrl, FString ServerProtocol = TEXT("ws"));

	UFUNCTION(BlueprintCallable, Category = "WebSockets")
	static UWebSocket* CreateWebSocketWithHeaders(FString ServerUrl, TMap<FString, FString> UpgradeHeaders, FString ServerProtocol = TEXT("ws"));

	UFUNCTION(BlueprintCallable, Category = "WebSockets")
	static int32 GetTimeInMilliseconds();
};