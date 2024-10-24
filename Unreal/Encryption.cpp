#include "Encryption.h"
#include "Misc/Base64.h"

TArray<uint8> FStringToByteArray(const FString& String)
{
    TArray<uint8> ByteArray;
    ByteArray.Reserve(String.Len());

    for (const TCHAR Char : String)
        ByteArray.Add(static_cast<uint8>(Char));

    return ByteArray;
}

FString ByteArrayToFString(const TArray<uint8>& ByteArray)
{
    FString String;
    String.Reserve(ByteArray.Num());

    for (const uint8 Byte : ByteArray)
        String += static_cast<TCHAR>(Byte);

    return String;
}

TArray<uint8> XorOperation(const TArray<uint8>& Input, const FString& Key)
{
    TArray<uint8> Result;
    TArray<uint8> KeyBytes = FStringToByteArray(Key);

    if (KeyBytes.Num() == 0)
    {
        UE_LOG(LogTemp, Error, TEXT("Key cannot be empty"));
        return Input;
    }

    Result.Reserve(Input.Num());

    for (int32 Index = 0; Index < Input.Num(); ++Index)
        Result.Add(Input[Index] ^ KeyBytes[Index % KeyBytes.Num()]);

    return Result;
}

FString UEncryption::Encrypt(const FString& Text, const FString& Key)
{
    TArray<uint8> TextBytes = FStringToByteArray(Text);
    TArray<uint8> EncryptedBytes = XorOperation(TextBytes, Key);
    return FBase64::Encode(EncryptedBytes.GetData(), EncryptedBytes.Num());
}

TArray<uint8> UEncryption::EncryptBuffer(const TArray<uint8>& TextBytes, const FString& Key)
{
    if (TextBytes.Num() > 0) {
        TArray<uint8> EncryptedBytes = XorOperation(TextBytes, Key);
        return EncryptedBytes;
    }
    else {
        return TextBytes;
    }
}

FString UEncryption::Decrypt(const FString& Text, const FString& Key)
{
    TArray<uint8> DecodedBytes;
    FBase64::Decode(Text, DecodedBytes);
    TArray<uint8> DecryptedBytes = XorOperation(DecodedBytes, Key);
    return ByteArrayToFString(DecryptedBytes);
}