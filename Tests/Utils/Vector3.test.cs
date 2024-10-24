namespace Tests
{
    public class Vector3Tests : AbstractTest
    {
        public Vector3Tests()
        {
            Describe("Vector3", () =>
            {
                It("should correctly create a Vector3 and access its properties", () =>
                {
                    var vector = new Vector3(1.0f, 2.0f, 3.0f);

                    Expect(vector.X).ToBe(1.0f);
                    Expect(vector.Y).ToBe(2.0f);
                    Expect(vector.Z).ToBe(3.0f);
                });

                It("should calculate the magnitude correctly", () =>
                {
                    var vector = new Vector3(3.0f, 4.0f, 0.0f);
                    float magnitude = vector.Magnitude;

                    Expect(magnitude).ToBe(5.0f);
                });

                It("should normalize a vector correctly", () =>
                {
                    var vector = new Vector3(3.0f, 4.0f, 0.0f);
                    var normalized = vector.Normalized;

                    Expect(normalized.X).ToBe(3.0f / 5.0f);
                    Expect(normalized.Y).ToBe(4.0f / 5.0f);
                    Expect(normalized.Z).ToBe(0.0f);
                });

                It("should add two vectors correctly", () =>
                {
                    var a = new Vector3(1.0f, 2.0f, 3.0f);
                    var b = new Vector3(4.0f, 5.0f, 6.0f);
                    var result = a + b;

                    Expect(result).ToBe(new Vector3(5.0f, 7.0f, 9.0f));
                });

                It("should subtract two vectors correctly", () =>
                {
                    var a = new Vector3(5.0f, 7.0f, 9.0f);
                    var b = new Vector3(1.0f, 2.0f, 3.0f);
                    var result = a - b;

                    Expect(result).ToBe(new Vector3(4.0f, 5.0f, 6.0f));
                });

                It("should multiply a vector by a scalar correctly", () =>
                {
                    var vector = new Vector3(1.0f, 2.0f, 3.0f);
                    var result = vector * 2.0f;

                    Expect(result).ToBe(new Vector3(2.0f, 4.0f, 6.0f));
                });

                It("should divide a vector by a scalar correctly", () =>
                {
                    var vector = new Vector3(4.0f, 6.0f, 8.0f);
                    var result = vector / 2.0f;

                    Expect(result).ToBe(new Vector3(2.0f, 3.0f, 4.0f));
                });

                It("should throw exception when dividing by zero", () =>
                {
                    var vector = new Vector3(1.0f, 2.0f, 3.0f);

                    try
                    {
                        var result = vector / 0.0f;
                        Expect(false).ToBeTrue(); // This should not be reached
                    }
                    catch (DivideByZeroException ex)
                    {
                        Expect(ex).NotToBeNull();
                    }
                });

                It("should calculate the dot product correctly", () =>
                {
                    var a = new Vector3(1.0f, 2.0f, 3.0f);
                    var b = new Vector3(4.0f, -5.0f, 6.0f);
                    float result = Vector3.Dot(a, b);

                    Expect(result).ToBe(12.0f); // 1*4 + 2*(-5) + 3*6
                });

                It("should calculate the cross product correctly", () =>
                {
                    var a = new Vector3(1.0f, 2.0f, 3.0f);
                    var b = new Vector3(4.0f, 5.0f, 6.0f);
                    var result = Vector3.Cross(a, b);

                    Expect(result).ToBe(new Vector3(-3.0f, 6.0f, -3.0f));
                });

                It("should correctly compare two vectors for equality", () =>
                {
                    var a = new Vector3(1.0f, 2.0f, 3.0f);
                    var b = new Vector3(1.0f, 2.0f, 3.0f);
                    var c = new Vector3(4.0f, 5.0f, 6.0f);

                    Expect(a.Equals(b)).ToBeTrue();
                    Expect(a.Equals(c)).ToBeFalse();
                });

                It("should return the correct string representation", () =>
                {
                    var vector = new Vector3(1.0f, 2.0f, 3.0f);
                    string result = vector.ToString();

                    Expect(result).ToBe("Vector3(1, 2, 3)");
                });

                It("should return 0 for the magnitude of a zero vector", () =>
                {
                    var zeroVector = new Vector3(0.0f, 0.0f, 0.0f);
                    float magnitude = zeroVector.Magnitude;

                    Expect(magnitude).ToBe(0.0f);
                });

                It("should return a zero vector when normalizing a zero vector", () =>
                {
                    var zeroVector = new Vector3(0.0f, 0.0f, 0.0f);
                    var normalized = zeroVector.Normalized;

                    Expect(normalized).ToBe(new Vector3(0.0f, 0.0f, 0.0f));
                });

                It("should return false when comparing a vector with null", () =>
                {
                    var vector = new Vector3(1.0f, 2.0f, 3.0f);
                    bool isEqual = vector.Equals(null);

                    Expect(isEqual).ToBeFalse();
                });

                It("should return consistent hash codes for equal vectors", () =>
                {
                    var vector1 = new Vector3(1.0f, 2.0f, 3.0f);
                    var vector2 = new Vector3(1.0f, 2.0f, 3.0f);

                    int hash1 = vector1.GetHashCode();
                    int hash2 = vector2.GetHashCode();

                    Expect(hash1).ToBe(hash2);
                });

                It("should throw exception when dividing a vector by zero scalar", () =>
                {
                    var vector = new Vector3(1.0f, 2.0f, 3.0f);

                    try
                    {
                        var result = vector / 0.0f;
                        Expect(false).ToBeTrue(); // Isso não deve ser alcançado
                    }
                    catch (DivideByZeroException ex)
                    {
                        Expect(ex).NotToBeNull();
                    }
                });

                It("should return 0 for the dot product of perpendicular vectors", () =>
                {
                    var a = new Vector3(1.0f, 0.0f, 0.0f);
                    var b = new Vector3(0.0f, 1.0f, 0.0f);
                    float result = Vector3.Dot(a, b);

                    Expect(result).ToBe(0.0f);
                });

                It("should return a zero vector when calculating the cross product of parallel vectors", () =>
                {
                    var a = new Vector3(1.0f, 2.0f, 3.0f);
                    var b = new Vector3(2.0f, 4.0f, 6.0f); // Vetor paralelo a 'a'
                    var result = Vector3.Cross(a, b);

                    Expect(result).ToBe(new Vector3(0.0f, 0.0f, 0.0f));
                });

                It("should return the correct vector when calculating the cross product of opposing vectors", () =>
                {
                    var a = new Vector3(1.0f, 0.0f, 0.0f);
                    var b = new Vector3(0.0f, 1.0f, 0.0f);
                    var result = Vector3.Cross(a, b);

                    Expect(result).ToBe(new Vector3(0.0f, 0.0f, 1.0f));
                });

                It("should return the same vector when adding a zero vector", () =>
                {
                    var vector = new Vector3(5.0f, -3.0f, 2.0f);
                    var zeroVector = new Vector3(0.0f, 0.0f, 0.0f);
                    var result = vector + zeroVector;

                    Expect(result).ToBe(vector);
                });

                It("should return the same vector when subtracting a zero vector", () =>
                {
                    var vector = new Vector3(5.0f, -3.0f, 2.0f);
                    var zeroVector = new Vector3(0.0f, 0.0f, 0.0f);
                    var result = vector - zeroVector;

                    Expect(result).ToBe(vector);
                });

                It("should correctly multiply a vector by a negative scalar", () =>
                {
                    var vector = new Vector3(1.0f, 2.0f, 3.0f);
                    var result = vector * -2.0f;

                    Expect(result).ToBe(new Vector3(-2.0f, -4.0f, -6.0f));
                });
            });
        }
    }
}
