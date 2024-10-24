#pragma once

#include "CoreMinimal.h"
#include "Engine/GameInstance.h"
#include "ServerSubsystem.generated.h"

UCLASS(DisplayName = "ServerSubsystem")
class CLIENT_API UServerSubsystem : public UGameInstance
{
	GENERATED_BODY()

public:

	UFUNCTION(BlueprintCallable, BlueprintPure, Category = "Game Data", meta = (WorldContext = "WorldContextObject"))
	static UServerSubsystem* GetServerSubsystem(UObject* WorldContextObject);

	DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FConnected, FString, ClientID);
	DECLARE_DYNAMIC_MULTICAST_DELEGATE(FDisconnected);

	UPROPERTY(BlueprintAssignable, meta = (DisplayName = "OnConnect", Keywords = ""), Category = "ServerSubsystem")
	FConnected OnConnect;

	UPROPERTY(BlueprintAssignable, meta = (DisplayName = "OnDisconnect", Keywords = ""), Category = "ServerSubsystem")
	FDisconnected OnDisconnect;

}; 