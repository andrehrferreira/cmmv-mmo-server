#pragma once

#include "CoreMinimal.h"
#include "ItemStates.generated.h"

UENUM(BlueprintType)
enum class EItemStates : uint8
{
    None UMETA(DisplayName = "None"),
    Blessed UMETA(DisplayName = "Blessed"),
    Insured UMETA(DisplayName = "Insured"),
    Exceptional UMETA(DisplayName = "Exceptional"),
    SpellChanneling UMETA(DisplayName = "Spell Channeling"),
    Broken UMETA(DisplayName = "Broken")
};

UCLASS()
class CLIENT_API UItemStateHelper : public UBlueprintFunctionLibrary
{
    GENERATED_BODY()

public:
    UFUNCTION(BlueprintCallable, Category = "Item States")
    static void AddItemState(EItemStates& CurrentState, EItemStates NewState);

    UFUNCTION(BlueprintCallable, Category = "Item States")
    static void RemoveItemState(EItemStates& CurrentState, EItemStates StateToRemove);

    UFUNCTION(BlueprintCallable, Category = "Item States")
    static bool HasItemState(EItemStates CurrentState, EItemStates StateToCheck);
};