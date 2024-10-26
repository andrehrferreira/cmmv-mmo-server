/*
 * Encryption
 *
 * Author: Andre Ferreira
 *
 * Copyright (c) Uzmi Games. Licensed under the MIT License.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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