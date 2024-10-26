namespace Tests
{
    public class RotatorTests : AbstractTest
    {
        public RotatorTests()
        {
            Describe("Rotator", () =>
            {
                It("should correctly create a Rotator and access its properties", () =>
                {
                    var rotator = new Rotator(45.0f, 30.0f, 60.0f);

                    Expect(rotator.Roll).ToBe(45.0f);
                    Expect(rotator.Pitch).ToBe(30.0f);
                    Expect(rotator.Yaw).ToBe(60.0f);
                });

                It("should return the zero rotator correctly", () =>
                {
                    var zeroRotator = Rotator.Zero;

                    Expect(zeroRotator.Roll).ToBe(0.0f);
                    Expect(zeroRotator.Pitch).ToBe(0.0f);
                    Expect(zeroRotator.Yaw).ToBe(0.0f);
                });

                It("should correctly rotate a vector", () =>
                {
                    var rotator = new Rotator(0.0f, 90.0f, 0.0f);
                    var vector = new Vector3(1.0f, 0.0f, 0.0f);
                    var result = rotator.RotateVector(vector);

                    // The vector should be rotated along the Y axis
                    Expect(result.X).ToBeApproximately(0.0f);
                    Expect(result.Y).ToBeApproximately(0.0f);
                    Expect(result.Z).ToBeApproximately(-1.0f);
                });

                It("should return true when comparing different rotators using Diff", () =>
                {
                    var rotator1 = new Rotator(45.0f, 30.0f, 60.0f);
                    var rotator2 = new Rotator(45.0f, 31.0f, 60.0f);

                    bool diff = rotator1.Diff(rotator2);

                    Expect(diff).ToBeTrue();
                });

                It("should return false when comparing identical rotators using Diff", () =>
                {
                    var rotator1 = new Rotator(45.0f, 30.0f, 60.0f);
                    var rotator2 = new Rotator(45.0f, 30.0f, 60.0f);

                    bool diff = rotator1.Diff(rotator2);

                    Expect(diff).ToBeFalse();
                });

                It("should create a copy of a rotator correctly", () =>
                {
                    var original = new Rotator(10.0f, 20.0f, 30.0f);
                    var copy = original.Copy();

                    Expect(copy.Roll).ToBe(original.Roll);
                    Expect(copy.Pitch).ToBe(original.Pitch);
                    Expect(copy.Yaw).ToBe(original.Yaw);
                    Expect(copy.Equals(original)).ToBeTrue();
                });

                It("should correctly convert a rotator to a string", () =>
                {
                    var rotator = new Rotator(15.0f, 25.0f, 35.0f);
                    string result = rotator.ToString();

                    Expect(result).ToBe("Roll: 15, Pitch: 25, Yaw: 35");
                });

                It("should return the same vector when rotating with a zero rotator", () =>
                {
                    var rotator = Rotator.Zero;
                    var vector = new Vector3(1.0f, 2.0f, 3.0f);
                    var result = rotator.RotateVector(vector);

                    Expect(result).ToBe(vector);
                });

                It("should correctly compare two rotators for equality", () =>
                {
                    var rotator1 = new Rotator(10.0f, 20.0f, 30.0f);
                    var rotator2 = new Rotator(10.0f, 20.0f, 30.0f);
                    var rotator3 = new Rotator(15.0f, 25.0f, 35.0f);

                    Expect(rotator1.Equals(rotator2)).ToBeTrue();
                    Expect(rotator1.Equals(rotator3)).ToBeFalse();
                });
            });
        }
    }
}
