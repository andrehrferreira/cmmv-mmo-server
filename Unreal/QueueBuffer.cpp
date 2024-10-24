#include "QueueBuffer.h"
#include "ByteBuffer.h"

UQueueBuffer* UQueueBufferFunctionLibary::CreateInstance(UWebSocket* Socket, uint8 QueuePacketType, const FString& Key)
{
    UQueueBuffer* WrapperQueueBuffer = NewObject<UQueueBuffer>();
    WrapperQueueBuffer->Socket = Socket;
    WrapperQueueBuffer->QueuePacketType = QueuePacketType;
    WrapperQueueBuffer->Key = Key;

    return WrapperQueueBuffer;
}

void UQueueBuffer::AddBuffer(uint8 PacketType, UByteBuffer* Buffer) {
    if (!IsDuplicatePacket(Buffer))
    {
        if (PacketType != QueuePacketType) {
            FQueueItem NewItem;
            NewItem.PacketType = PacketType;
            NewItem.Buffer = Buffer;

            Queues.Add(NewItem);
        }
    }
}

bool UQueueBuffer::IsDuplicatePacket(UByteBuffer* Buffer)
{
    FString BufferHash = Buffer->ToString();

    for (const auto& RecentBuffer : Queues)
    {
        if (RecentBuffer.Buffer->ToString() == BufferHash)
            return true;
    }

    return false;
}

void UQueueBuffer::CheckAndSend()
{
    auto& Buffers = Queues;

    int32 TotalSize = 0;

    for (const auto& Buffer : Buffers)
        TotalSize += Buffer.Buffer->Length();

    if (TotalSize >= MaxBufferSize)
        SendBuffers();
}

void UQueueBuffer::SendBuffers()
{
    if (Queues.Num() == 0 || !Socket) return;

    if (Queues.Num() > 1)
    {
        UByteBuffer* CombinedBuffer = CombineBuffers(Queues);
        TArray<uint8> FinalBuffer = CombinedBuffer->GetBuffer();

        FString HexString = UByteBuffer::ByteArrayToBinaryString(FinalBuffer);
        Socket->SendEncryptedMessage(QueuePacketType, CombinedBuffer, Key);
    }
    else
    {
        FString HexString = UByteBuffer::ByteArrayToBinaryString(Queues[0].Buffer->GetBuffer());
        Socket->SendEncryptedMessage(Queues[0].PacketType, Queues[0].Buffer, Key);
    }

    Queues.Empty();
}

UByteBuffer* UQueueBuffer::CombineBuffers(const TArray<FQueueItem>& Buffers)
{
    UByteBuffer* CombinedBuffer = UByteBuffer::CreateEmptyByteBuffer();

    for (const auto& QueueItem : Buffers)
    {
        TArray<uint8> Buf = QueueItem.Buffer->GetBuffer();
        CombinedBuffer->PutByte(QueueItem.PacketType);
        CombinedBuffer->GetBuffer().Append(Buf);

        for (int32 i = 0; i < EndRepeatByte; ++i)
            CombinedBuffer->PutByte(EndOfPacketByte);
    }

    return CombinedBuffer;
}

void UQueueBuffer::Tick() {
    if (Queues.Num() == 0 || !Socket) return;

    SendBuffers();
}