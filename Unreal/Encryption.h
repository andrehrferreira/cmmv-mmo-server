#pragma once

#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "Encryption.generated.h"

UCLASS()
class UEncryption : public UBlueprintFunctionLibrary
{
    GENERATED_BODY()

public:
    UFUNCTION(BlueprintCallable, Category = "CustomEncryption")
    static FString Encrypt(const FString& Text, const FString& Key);

    UFUNCTION(BlueprintCallable, Category = "CustomEncryption")
    static TArray<uint8> EncryptBuffer(const TArray<uint8>& TextBytes, const FString& Key);

    UFUNCTION(BlueprintCallable, Category = "CustomEncryption")
    static FString Decrypt(const FString& Text, const FString& Key);

private:
    static FString ShiftBytes(const FString& Text, const FString& Key, bool bEncrypt);
};