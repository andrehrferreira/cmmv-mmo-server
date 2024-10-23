#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "Engine/GameInstance.h"

UCLASS(DisplayName = "ServerSubsystem")
class CLIENT_API ServerSubsystem : public UGameInstanceSubsystem 
{
	GENERATED_BODY()

public:

	virtual void Initialize(FSubsystemCollectionBase& Collection) override;
	virtual void Deinitialize() override;

	ServerSubsystem();
private:

protected:
	FString Host = "127.0.0.1";
	int Port = 5000;

};