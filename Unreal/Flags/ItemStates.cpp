#include "ItemStates.h"
#include "Kismet/KismetSystemLibrary.h"

void UItemStateHelper::AddItemState(EItemStates& CurrentState, EItemStates NewState)
{
    CurrentState = static_cast<EItemStates>(static_cast<uint8>(CurrentState) | static_cast<uint8>(NewState));
}

void UItemStateHelper::RemoveItemState(EItemStates& CurrentState, EItemStates StateToRemove)
{
    CurrentState = static_cast<EItemStates>(static_cast<uint8>(CurrentState) & ~static_cast<uint8>(StateToRemove));
}

bool UItemStateHelper::HasItemState(EItemStates CurrentState, EItemStates StateToCheck)
{
    return (static_cast<uint8>(CurrentState) & static_cast<uint8>(StateToCheck)) != 0;
}
