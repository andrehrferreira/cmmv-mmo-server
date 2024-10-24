#include "ServerSubsystem.h"
#include "Kismet/GameplayStatics.h"

UServerSubsystem* UServerSubsystem::GetServerSubsystem(UObject* WorldContextObject)
{
    if (WorldContextObject)
    {
        if (UGameInstance* GameInstance = UGameplayStatics::GetGameInstance(WorldContextObject))
        {
            return Cast<UServerSubsystem>(GameInstance);
        }
    }

    return nullptr;
}
