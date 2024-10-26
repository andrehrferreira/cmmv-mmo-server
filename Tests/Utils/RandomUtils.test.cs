namespace Tests
{
    public class RandomUtilsTests : AbstractTest
    {
        public RandomUtilsTests()
        {
            Describe("RandomUtils", () =>
            {
                It("should generate a random integer within the specified range", () =>
                {
                    int min = 5;
                    int max = 10;
                    int result = RandomUtils.MinMaxInt(min, max);

                    Expect(result).ToBeGreaterThanOrEqualTo(min);
                    Expect(result).ToBeLessThanOrEqualTo(max);
                });

                It("should return the min value if min is greater than or equal to max", () =>
                {
                    int min = 10;
                    int max = 5;
                    int result = RandomUtils.MinMaxInt(min, max);

                    Expect(result).ToBe(min);
                });

                It("should return true for DropChance with a 100% chance", () =>
                {
                    bool result = RandomUtils.DropChance(100);
                    Expect(result).ToBeTrue();
                });

                It("should return false for DropChance with a 0% chance", () =>
                {
                    bool result = RandomUtils.DropChance(0);
                    Expect(result).ToBeFalse();
                });

                It("should return false for DropChance with a negative chance", () =>
                {
                    bool result = RandomUtils.DropChance(-10);
                    Expect(result).ToBeFalse();
                });

                It("should return false for DropChance with a chance greater than 100", () =>
                {
                    bool result = RandomUtils.DropChance(110);
                    Expect(result).ToBeFalse();
                });

                It("should select a random element from an array", () =>
                {
                    int[] array = { 1, 2, 3, 4, 5 };
                    int result = RandomUtils.ArrRandom(array);

                    Expect(array).ToContain(result);
                });

                It("should return default value for an empty array", () =>
                {
                    int[] emptyArray = { };
                    int result = RandomUtils.ArrRandom(emptyArray);

                    Expect(result).ToBe(default(int));
                });

                It("should select a random element from a list", () =>
                {
                    List<string> list = new List<string> { "apple", "banana", "cherry" };
                    string result = RandomUtils.ArrRandom(list);

                    Expect(list).ToContain(result);
                });

                It("should return default value for an empty list", () =>
                {
                    List<string> emptyList = new List<string>();
                    string result = RandomUtils.ArrRandom(emptyList);

                    Expect(result).ToBe(default(string));
                });

                It("should return approximately the correct percentage for DropChance with a 50% chance", () =>
                {
                    int attempts = 10000;
                    int successCount = 0;
                    double chance = 50.0;

                    for (int i = 0; i < attempts; i++)
                    {
                        if (RandomUtils.DropChance(chance))
                            successCount++;
                    }

                    double successRate = (successCount / (double)attempts) * 100;
                    Expect(successRate).ToBeApproximately(chance, 5.0); // Allowing a margin of error of 5%
                });
            });
        }
    }
}
