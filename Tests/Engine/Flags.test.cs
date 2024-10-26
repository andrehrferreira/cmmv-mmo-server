namespace Tests
{
    public class StateFlagsTests : AbstractTest
    {
        public StateFlagsTests()
        {
            Describe("StateFlags", () =>
            {
                It("should correctly add and verify EntityStates flags", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(EntityStates.Ally);
                    stateFlags.AddFlag(EntityStates.Dead);

                    Expect(stateFlags.HasFlag(EntityStates.Ally)).ToBeTrue();
                    Expect(stateFlags.HasFlag(EntityStates.Dead)).ToBeTrue();
                    Expect(stateFlags.HasFlag(EntityStates.Enemy)).ToBeFalse(); // Enemy flag not added
                });

                It("should correctly remove EntityStates flags", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(EntityStates.Ally);
                    stateFlags.AddFlag(EntityStates.Dead);
                    stateFlags.RemoveFlag(EntityStates.Ally);

                    Expect(stateFlags.HasFlag(EntityStates.Ally)).ToBeFalse();
                    Expect(stateFlags.HasFlag(EntityStates.Dead)).ToBeTrue(); // Dead flag should still be present
                });

                It("should correctly check for the absence of EntityStates flags using DontHasFlag", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(EntityStates.Ally);

                    Expect(stateFlags.DontHasFlag(EntityStates.Enemy)).ToBeTrue(); // Enemy flag not added
                    Expect(stateFlags.DontHasFlag(EntityStates.Ally)).ToBeFalse(); // Ally flag is present
                });

                It("should correctly add and verify BuffsStates flags", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(EntityStates.Burning);
                    stateFlags.AddFlag(EntityStates.Bleeding);

                    Expect(stateFlags.HasFlag(EntityStates.Burning)).ToBeTrue();
                    Expect(stateFlags.HasFlag(EntityStates.Bleeding)).ToBeTrue();
                    Expect(stateFlags.HasFlag(EntityStates.Poisoned)).ToBeFalse(); // Poisoned flag not added
                });

                It("should correctly remove BuffsStates flags", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(EntityStates.Poisoned);
                    stateFlags.RemoveFlag(EntityStates.Poisoned);

                    Expect(stateFlags.HasFlag(EntityStates.Poisoned)).ToBeFalse(); // Poisoned flag should be removed
                });

                It("should correctly check for the absence of BuffsStates flags using DontHasFlag", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(EntityStates.Stunned);

                    Expect(stateFlags.DontHasFlag(EntityStates.Stunned)).ToBeFalse(); // Stunned flag is present
                    Expect(stateFlags.DontHasFlag(EntityStates.Burning)).ToBeTrue(); // Burning flag not added
                });

                It("should correctly add and verify ItemStates flags", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(ItemStates.Broken);

                    Expect(stateFlags.HasFlag(ItemStates.Broken)).ToBeTrue();
                    Expect(stateFlags.HasFlag(ItemStates.Blessed)).ToBeFalse(); // Poisoned flag not added
                });

                It("should correctly remove ItemStates flags", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(ItemStates.Blessed);
                    stateFlags.RemoveFlag(ItemStates.Blessed);

                    Expect(stateFlags.HasFlag(ItemStates.Blessed)).ToBeFalse(); // Equipped flag should be removed
                });

                It("should correctly check for the absence of ItemStates flags using DontHasFlag", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(ItemStates.Blessed);

                    Expect(stateFlags.DontHasFlag(ItemStates.Blessed)).ToBeFalse(); // Poisoned flag is present
                    Expect(stateFlags.DontHasFlag(ItemStates.Broken)).ToBeTrue(); // Broken flag not added
                });

                It("should correctly return the current flags value", () =>
                {
                    var stateFlags = new StateFlags();
                    stateFlags.AddFlag(EntityStates.Ally);
                    stateFlags.AddFlag(EntityStates.Burning);

                    int currentFlags = stateFlags.GetCurrentFlags();
                    Expect((currentFlags & (int)EntityStates.Ally) != 0).ToBeTrue();
                    Expect((currentFlags & (int)EntityStates.Burning) != 0).ToBeTrue();
                });
            });
        }
    }
}
