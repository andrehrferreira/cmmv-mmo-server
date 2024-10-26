namespace Tests
{
    public class ByteBufferPoolTests : AbstractTest
    {
        public ByteBufferPoolTests()
        {
            Describe("ByteBufferPool", () =>
            {
                It("should add a buffer to the pool and increase its length", () =>
                {
                    var pool = new ByteBufferPool();
                    var buffer = new ByteBuffer(10);
                    pool.Add(buffer);

                    Expect(pool.Length).ToBe(1);
                });

                It("should take a buffer from the pool and reduce its length", () =>
                {
                    var pool = new ByteBufferPool();
                    var buffer = new ByteBuffer(10);
                    pool.Add(buffer);

                    var takenBuffer = pool.Take();
                    Expect(takenBuffer).NotToBeNull();
                    Expect(pool.Length).ToBe(0);
                });

                It("should return null when taking from an empty pool", () =>
                {
                    var pool = new ByteBufferPool();
                    var result = pool.Take();

                    Expect(result).ToBeNull();
                });

                It("should clear the pool and return the head buffer", () =>
                {
                    var pool = new ByteBufferPool();
                    var buffer1 = new ByteBuffer(10);
                    var buffer2 = new ByteBuffer(20);

                    pool.Add(buffer1);
                    pool.Add(buffer2);

                    var clearedBuffer = pool.Clear();

                    Expect(clearedBuffer).ToBe(buffer2); 
                    Expect(pool.Length).ToBe(0);
                });

                It("should merge two pools correctly", () =>
                {
                    var pool1 = new ByteBufferPool();
                    var pool2 = new ByteBufferPool();
                    var buffer1 = new ByteBuffer(10);
                    var buffer2 = new ByteBuffer(20);
                    var buffer3 = new ByteBuffer(30);

                    pool1.Add(buffer1);
                    pool2.Add(buffer2);
                    pool2.Add(buffer3);

                    pool1.Merge(pool2);

                    Expect(pool1.Length).ToBe(3);
                    Expect(pool2.Length).ToBe(0);
                });

                It("should keep track of buffers correctly when taking multiple times", () =>
                {
                    var pool = new ByteBufferPool();
                    var buffer1 = new ByteBuffer(10);
                    var buffer2 = new ByteBuffer(20);

                    pool.Add(buffer1);
                    pool.Add(buffer2);

                    var firstTaken = pool.Take();
                    Expect(firstTaken).ToBe(buffer2);
                    Expect(pool.Length).ToBe(1);

                    var secondTaken = pool.Take();
                    Expect(secondTaken).ToBe(buffer1);
                    Expect(pool.Length).ToBe(0);
                });

                It("should correctly handle an empty pool when merging", () =>
                {
                    var pool1 = new ByteBufferPool();
                    var pool2 = new ByteBufferPool();
                    var buffer = new ByteBuffer(10);

                    pool1.Add(buffer);
                    pool1.Merge(pool2);

                    Expect(pool1.Length).ToBe(1);
                    Expect(pool2.Length).ToBe(0);
                });

                It("should correctly set Head and Tail when merging non-empty pools", () =>
                {
                    var pool1 = new ByteBufferPool();
                    var pool2 = new ByteBufferPool();
                    var buffer1 = new ByteBuffer(10);
                    var buffer2 = new ByteBuffer(20);

                    pool1.Add(buffer1);
                    pool2.Add(buffer2);

                    pool1.Merge(pool2);

                    Expect(pool1.Head).ToBe(buffer1);
                    Expect(pool1.Tail).ToBe(buffer2);
                });

                It("should return the length correctly when buffers are added and removed", () =>
                {
                    var pool = new ByteBufferPool();
                    var buffer1 = new ByteBuffer(10);
                    var buffer2 = new ByteBuffer(20);

                    Expect(pool.Length).ToBe(0);

                    pool.Add(buffer1);
                    Expect(pool.Length).ToBe(1);

                    pool.Add(buffer2);
                    Expect(pool.Length).ToBe(2);

                    pool.Take();
                    Expect(pool.Length).ToBe(1);

                    pool.Take();
                    Expect(pool.Length).ToBe(0);
                });

                It("should return the same buffer instance when taken from the pool", () =>
                {
                    var pool = new ByteBufferPool();
                    var buffer = new ByteBuffer(10);
                    pool.Add(buffer);

                    var takenBuffer = pool.Take();
                    Expect(takenBuffer).ToBe(buffer);
                });
            });
        }
    }
}
