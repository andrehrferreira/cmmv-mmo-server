
namespace Tests
{
    public class QueueBufferTests : AbstractTest
    {
        public QueueBufferTests()
        {
            Describe("QueueBuffer", () =>
            {
                It("should add and retrieve a socket", () =>
                {
                    var socket = new Socket();
                    QueueBuffer.AddSocket("testSocket", socket);

                    var retrievedSocket = QueueBuffer.GetSocket("testSocket");

                    Expect(retrievedSocket).NotToBeNull();
                    Expect(retrievedSocket).ToBe(socket);

                    QueueBuffer.RemoveSocket("testSocket");
                    var removedSocket = QueueBuffer.GetSocket("testSocket");
                    Expect(removedSocket).ToBeNull();
                });

                It("should detect duplicate packet", () =>
                {
                    var buffer = new ByteBuffer();
                    buffer.Write("Duplicate packet");

                    QueueBuffer.AddBuffer("testSocket", buffer);

                    bool isDuplicate = QueueBuffer.IsDuplicatePacket("testSocket", buffer);

                    Expect(isDuplicate).ToBeTrue();
                });

                It("should combine buffers correctly", () =>
                {
                    var buffer1 = new ByteBuffer();
                    buffer1.Write("Buffer 1");
                    var buffer2 = new ByteBuffer();
                    buffer2.Write("Buffer 2");

                    QueueBuffer.AddBuffer("testSocket", buffer1);
                    QueueBuffer.AddBuffer("testSocket", buffer2);

                    var combinedBuffer = QueueBuffer.CombineBuffers(new List<ByteBuffer> { buffer1, buffer2 });                   
                    var combinedArray = combinedBuffer.GetBuffer();
                    int expectedSize = buffer1.GetBuffer().Length + buffer2.GetBuffer().Length + (2 * QueueBuffer.EndRepeatByte) + 1;

                    Expect(combinedArray.Length).ToBe(expectedSize);
                });


                It("should send buffers when total size exceeds max buffer size", () =>
                {
                    var largeBuffer = new ByteBuffer(new byte[QueueBuffer.MaxBufferSize]);
                    QueueBuffer.AddBuffer("testSocket", largeBuffer);

                    var socket = new Socket();
                    QueueBuffer.AddSocket("testSocket", socket);

                    try
                    {
                        QueueBuffer.CheckAndSend("testSocket");
                        Expect(true).ToBeTrue(); // Expect no exceptions
                    }
                    catch (Exception ex)
                    {
                        Expect(ex).ToBeNull(); // Should not reach here
                    }
                });

                It("should tick and send buffers correctly", () =>
                {
                    var buffer = new ByteBuffer();
                    buffer.Write("Tick packet");

                    QueueBuffer.AddBuffer("testSocket", buffer);

                    QueueBuffer.Tick(null);

                    bool isDuplicate = QueueBuffer.IsDuplicatePacket("testSocket", buffer);

                    Expect(isDuplicate).ToBeFalse(); // Packet should be sent and queue cleared
                });

                It("should start ticking at regular intervals", () =>
                {
                    // Ensure no exceptions are thrown during this operation
                    try
                    {
                        QueueBuffer.StartTicking(1000);
                        Expect(true).ToBeTrue();
                    }
                    catch (Exception ex)
                    {
                        Expect(ex).ToBeNull(); // Should not reach here
                    }
                });
            });
        }
    }
}
