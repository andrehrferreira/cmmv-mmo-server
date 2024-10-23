
namespace Tests
{
    public class ByteBufferTests : AbstractTest
    {
        public ByteBufferTests()
        {
            Describe("ByteBuffer", () =>
            {
                It("should write and read a byte value", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write((byte)42);

                    buffer = new ByteBuffer(buffer.GetBuffer());
                    byte result = buffer.Read<byte>();

                    Expect(result).ToBe(42);
                });

                It("should write and read an integer value", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write(12345);

                    buffer = new ByteBuffer(buffer.GetBuffer());
                    int result = buffer.Read<int>();

                    Expect(result).ToBe(12345);
                });

                It("should write and read a boolean value", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write(true);
                    buffer.Write(false);

                    buffer = new ByteBuffer(buffer.GetBuffer());
                    bool trueResult = buffer.Read<bool>();
                    bool falseResult = buffer.Read<bool>();

                    Expect(trueResult).ToBeTrue();
                    Expect(falseResult).ToBeFalse();
                });

                It("should write and read a float value", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write(3.14f);

                    buffer = new ByteBuffer(buffer.GetBuffer());
                    float result = buffer.ReadFloat();

                    Expect(result).ToBe(3.14f);
                });

                It("should write and read a string value", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    string text = "Hello, World!";
                    buffer.Write(text);

                    buffer = new ByteBuffer(buffer.GetBuffer());
                    string result = buffer.ReadString();

                    Expect(result).ToBe("Hello, World!");
                });

                It("should handle buffer underflow when reading an int", () =>
                {
                    var buffer = new ByteBuffer(2); 

                    try
                    {
                        buffer.ReadInt();
                        Expect(false).ToBeTrue(); 
                    }
                    catch (InvalidOperationException ex)
                    {
                        Expect(ex).NotToBeNull();
                        Expect(ex.Message).ToBe("Buffer underflow");
                    }
                });

                It("should handle buffer underflow when reading a byte", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();

                    try
                    {
                        buffer.ReadByte();
                        Expect(false).ToBeTrue(); // This should not be reached
                    }
                    catch (InvalidOperationException ex)
                    {
                        Expect(ex).NotToBeNull();
                    }
                });

                It("should handle buffer underflow when reading a float", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();

                    try
                    {
                        buffer.ReadFloat();
                        Expect(false).ToBeTrue(); // This should not be reached
                    }
                    catch (InvalidOperationException ex)
                    {
                        Expect(ex).NotToBeNull();
                    }
                });

                It("should handle buffer underflow when reading a string", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write(5); // Length of the string, but no actual string data

                    buffer = new ByteBuffer(buffer.GetBuffer());

                    try
                    {
                        buffer.ReadString();
                        Expect(false).ToBeTrue(); // This should not be reached
                    }
                    catch (InvalidOperationException ex)
                    {
                        Expect(ex).NotToBeNull();
                    }
                });

                It("should write and read multiple types sequentially", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write((byte)42);
                    buffer.Write(12345);
                    buffer.Write(3.14f);
                    buffer.Write("Test");

                    buffer = new ByteBuffer(buffer.GetBuffer());

                    byte byteResult = buffer.Read<byte>();
                    int intResult = buffer.Read<int>();
                    float floatResult = buffer.Read<float>();
                    string stringResult = buffer.Read<string>();

                    Expect(byteResult).ToBe(42);
                    Expect(intResult).ToBe(12345);
                    Expect(floatResult).ToBe(3.14f);
                    Expect(stringResult).ToBe("Test");
                });

                It("should correctly handle an empty buffer", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();

                    byte[] result = buffer.GetBuffer();
                    Expect(result.Length).ToBe(0);
                });

                It("should resize buffer when writing beyond initial capacity", () =>
                {
                    var buffer = new ByteBuffer(2); // Initial capacity of 2 bytes
                    buffer.Write((byte)1);
                    buffer.Write((byte)2);
                    buffer.Write((byte)3); // This should trigger a resize

                    Expect(buffer.GetBuffer().Length).ToBeGreaterThan(2);
                });

                It("should dispose and release resources properly", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write(42);
                    buffer.Dispose();

                    try
                    {
                        buffer.GetBuffer();
                        Expect(false).ToBeTrue();
                    }
                    catch (ObjectDisposedException ex)
                    {
                        Expect(ex).NotToBeNull();
                    }
                });

                It("should write and read a negative integer value", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write(-9876);

                    buffer = new ByteBuffer(buffer.GetBuffer());
                    int result = buffer.Read<int>();

                    Expect(result).ToBe(-9876);
                });

                It("should throw exception when reading unsupported type", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();

                    try
                    {
                        buffer.Read<double>(); // Double is not supported
                        Expect(false).ToBeTrue(); // This should not be reached
                    }
                    catch (NotSupportedException ex)
                    {
                        Expect(ex).NotToBeNull();
                    }
                });

                It("should handle buffer overflow when writing a large string", () =>
                {
                    var longString = new string('A', 5000); // Large string
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write(longString);

                    buffer = new ByteBuffer(buffer.GetBuffer());
                    string result = buffer.ReadString();

                    Expect(result).ToBe(longString);
                });

                It("should handle writing and reading a boolean false value", () =>
                {
                    var buffer = ByteBuffer.CreateEmptyBuffer();
                    buffer.Write(false);

                    buffer = new ByteBuffer(buffer.GetBuffer());
                    bool result = buffer.Read<bool>();

                    Expect(result).ToBeFalse();
                });

                It("should correctly read written data even after multiple buffer copies", () =>
                {
                    var originalBuffer = ByteBuffer.CreateEmptyBuffer();
                    originalBuffer.Write(123);
                    originalBuffer.Write(456);
                    originalBuffer.Write("Test string");

                    var copiedBuffer = new ByteBuffer(originalBuffer);
                    copiedBuffer.ResetPosition();

                    int firstValue = copiedBuffer.Read<int>();
                    int secondValue = copiedBuffer.Read<int>();
                    string stringValue = copiedBuffer.ReadString();

                    Expect(firstValue).ToBe(123);
                    Expect(secondValue).ToBe(456);
                    Expect(stringValue).ToBe("Test string");
                });
            });
        }
    }
}
