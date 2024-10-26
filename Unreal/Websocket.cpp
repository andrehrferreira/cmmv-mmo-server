/*
 * Websocket
 *
 * Author: Chris 
 * Modified by: Andre Ferreira
 * 
 * @see https://github.com/minimpoun/BlueprintWebsockets
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

#include "Websocket.h"
#include "IWebSocket.h"
#include "ByteBuffer.h"
#include "Encryption.h"
#include "WebSocketsModule.h"

#define LOCTEXT_NAMESPACE "FToSWebsocketsModule"

void LogByteArray(const TArray<uint8>& ByteArray)
{
	FString HexString;

	for (uint8 Byte : ByteArray)
		HexString += FString::Printf(TEXT("%02X "), Byte);

	UE_LOG(LogTemp, Log, TEXT("Byte Array: %s"), *HexString);
}

void UWebSocket::InitWebSocket(TSharedPtr<IWebSocket> InWebSocket)
{
	InternalWebSocket = InWebSocket;

	InternalWebSocket->OnConnected().AddUObject(this, &ThisClass::OnWebSocketConnected_Internal);
	InternalWebSocket->OnConnectionError().AddUObject(this, &ThisClass::OnWebSocketConnectionError_Internal);
	InternalWebSocket->OnClosed().AddUObject(this, &ThisClass::OnWebSocketClosed_Internal);
	InternalWebSocket->OnMessage().AddUObject(this, &ThisClass::OnWebSocketMessageReceived_Internal);
	InternalWebSocket->OnMessageSent().AddUObject(this, &ThisClass::OnWebSocketMessageSent_Internal);
	InternalWebSocket->OnBinaryMessage().AddUObject(this, &ThisClass::OnWebSocketBinaryMessageReceived_Internal);
}

void UWebSocket::Connect()
{
	InternalWebSocket->Connect();
}

void UWebSocket::Close(int32 StatusCode, const FString& Reason)
{
	InternalWebSocket->Close(StatusCode, Reason);
}

bool UWebSocket::IsConnected() const
{
	return InternalWebSocket->IsConnected();
}

void UWebSocket::SendMessage(uint8 PacketType, UByteBuffer* Message)
{
	//LogByteArray(Message->GetBuffer());

	const TArray<uint8>& OriginalData = Message->GetBuffer();
	TArray<uint8> NewData;

	NewData.Reserve(1 + OriginalData.Num());
	NewData.Add(PacketType);
	NewData.Append(OriginalData);

	//LogByteArray(NewData);

	InternalWebSocket->Send(NewData.GetData(), NewData.Num(), true);
}

void UWebSocket::SendEncryptedMessage(uint8 PacketType, UByteBuffer* Message, const FString& Key)
{
	const TArray<uint8>& OriginalData = Message->GetBuffer();
	TArray<uint8> NewData;

	NewData.Reserve(1 + OriginalData.Num());
	NewData.Add(PacketType);
	NewData.Append(OriginalData);

	const TArray<uint8>& EncryptedData = UEncryption::EncryptBuffer(NewData, Key);
	InternalWebSocket->Send(EncryptedData.GetData(), EncryptedData.Num(), true);
}

void UWebSocket::OnWebSocketConnected_Internal()
{
	OnWebSocketConnected.Broadcast();
}

void UWebSocket::OnWebSocketConnectionError_Internal(const FString& Error)
{
	OnWebSocketConnectionError.Broadcast(Error);
}

void UWebSocket::OnWebSocketClosed_Internal(int32 StatusCode, const FString& Reason, bool bWasClean)
{
	OnWebSocketClosed.Broadcast(StatusCode, Reason, bWasClean);
}

void UWebSocket::OnWebSocketMessageReceived_Internal(const FString& Message)
{
	OnWebSocketMessageReceived.Broadcast(Message);
}

void UWebSocket::OnWebSocketBinaryMessageReceived_Internal(const void* Data, SIZE_T Size, bool bIsBinary)
{
	TArray<uint8> ByteArray;
	const uint8* ByteData = static_cast<const uint8*>(Data);

	for (SIZE_T i = 0; i < Size; ++i)
		ByteArray.Add(ByteData[i]);

	//LogByteArray(ByteArray);

	UByteBuffer* Buffer = UByteBuffer::CreateByteBuffer(ByteArray);
	OnWebSocketBinaryMessageReceived.Broadcast(Buffer);
}

void UWebSocket::OnWebSocketMessageSent_Internal(const FString& Message)
{
	OnWebSocketMessageSent.Broadcast(Message);
}

UWebSocket* UWebSocketFunctionLibrary::CreateWebSocket(FString ServerUrl, FString ServerProtocol)
{
	return CreateWebSocketWithHeaders(ServerUrl, {}, ServerProtocol);
}

UWebSocket* UWebSocketFunctionLibrary::CreateWebSocketWithHeaders(FString ServerUrl, TMap<FString, FString> UpgradeHeaders, FString ServerProtocol /* = TEXT("ws") */)
{
	const TSharedPtr<IWebSocket> ActualSocket = FModuleManager::LoadModuleChecked<FWebSocketsModule>(TEXT("WebSockets")).CreateWebSocket(ServerUrl, ServerProtocol, UpgradeHeaders);
	UWebSocket* const WrapperSocket = NewObject<UWebSocket>();
	WrapperSocket->InitWebSocket(ActualSocket);
	return WrapperSocket;
}

int32 UWebSocketFunctionLibrary::GetTimeInMilliseconds()
{
	FDateTime Now = FDateTime::Now();
	FTimespan Timespan = Now.GetTimeOfDay();
	int64 Milliseconds = Timespan.GetTotalMilliseconds();
	return Milliseconds;
}