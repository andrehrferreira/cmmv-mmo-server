namespace Tests
{
    public class ConcurrentByteBufferPoolTests : AbstractTest
    {
        public ConcurrentByteBufferPoolTests()
        {
            Describe("ConcurrentByteBufferPool", () =>
            {
                It("should acquire a new buffer when the pool is empty", () =>
                {
                    var buffer = ConcurrentByteBufferPool.Acquire();
                    Expect(buffer).NotToBeNull();
                    Expect(buffer.GetBuffer().Length).ToBe(0);
                });

                It("should acquire a buffer from the global pool when the local pool is empty", () =>
                {
                    var buffer1 = ConcurrentByteBufferPool.Acquire();
                    buffer1.Write(42);
                    ConcurrentByteBufferPool.Release(buffer1);

                    var buffer2 = ConcurrentByteBufferPool.Acquire();
                    Expect(buffer2).NotToBeNull();
                    Expect(buffer2).ToBe(buffer1);
                    Expect(buffer2.Read<int>()).ToBe(42);
                });

                It("should return a new buffer if both local and global pools are empty", () =>
                {
                    ConcurrentByteBufferPool.Merge();
                    ConcurrentByteBufferPool.Clear();

                    var buffer = ConcurrentByteBufferPool.Acquire();
                    Expect(buffer).NotToBeNull();
                });

                It("should correctly merge local pool into the global pool", () =>
                {
                    var buffer1 = ConcurrentByteBufferPool.Acquire();
                    buffer1.Write(123);
                    ConcurrentByteBufferPool.Release(buffer1);
                    ConcurrentByteBufferPool.Merge();

                    var buffer2 = ConcurrentByteBufferPool.Acquire();
                    Expect(buffer2).NotToBeNull();
                    Expect(buffer2.Read<int>()).ToBe(123);
                });

                It("should reset and reuse buffers after releasing", () =>
                {
                    var buffer1 = ConcurrentByteBufferPool.Acquire();
                    buffer1.Write(3.14f);
                    ConcurrentByteBufferPool.Release(buffer1);

                    var buffer2 = ConcurrentByteBufferPool.Acquire();
                    Expect(buffer2).ToBe(buffer1);

                    Expect(buffer2.Position).ToBe(0);
                    buffer2.Write(1.23f);

                    buffer2 = new ByteBuffer(buffer2.GetBuffer());
                    var value = buffer2.Read<float>();
                    Expect(value).ToBe(1.23f);
                });

                It("should clear the global pool and return all buffers", () =>
                {
                    var buffer1 = ConcurrentByteBufferPool.Acquire();
                    buffer1.Write(10);
                    ConcurrentByteBufferPool.Release(buffer1);
                    ConcurrentByteBufferPool.Merge();

                    var clearedBuffer = ConcurrentByteBufferPool.Clear();
                    Expect(clearedBuffer).NotToBeNull();
                    Expect(clearedBuffer.GetBuffer().Length).ToBeGreaterThan(0);

                    var bufferAfterClear = ConcurrentByteBufferPool.Acquire();
                    Expect(bufferAfterClear).NotToBe(clearedBuffer);
                });

                It("should acquire a new buffer when all buffers have been taken", () =>
                {
                    var buffer1 = ConcurrentByteBufferPool.Acquire();
                    var buffer2 = ConcurrentByteBufferPool.Acquire(); 

                    Expect(buffer2).NotToBe(buffer1);
                });
            });
        }
    }
}
