
namespace Tests
{
    public class ReactiveTests: AbstractTest
    {
        public ReactiveTests()
        {
            Describe("Reactive System", () =>
            {
                It("should notify subscribers when value changes", () =>
                {
                    var reactive = new Reactive<int>();
                    int value = 0;

                    reactive.Subscribe(v => value = v);
                    reactive.Value = 1;

                    Expect(value).ToBe(1);
                });

                It("should allow nested reactive objects", () =>
                {
                    var reactive = new Reactive<Reactive<int>>();
                    var nested = new Reactive<int> { Value = 0 };
                    reactive.Value = nested;

                    Expect(reactive.Value.Value).ToBe(0);
                    reactive.Value.Value = 5;
                    Expect(reactive.Value.Value).ToBe(5);
                });

                It("should unsubscribe properly", () =>
                {
                    var reactive = new Reactive<int>();
                    int value = 0;

                    var subscription = reactive.Subscribe(v => value = v);
                    reactive.Value = 10;
                    subscription.Dispose();
                    reactive.Value = 20;

                    Expect(value).ToBe(10); 
                });

                It("should allow multiple subscribers", () =>
                {
                    var reactive = new Reactive<int>();
                    int value1 = 0, value2 = 0;

                    reactive.Subscribe(v => value1 = v);
                    reactive.Subscribe(v => value2 = v);
                    reactive.Value = 5;

                    Expect(value1).ToBe(5);
                    Expect(value2).ToBe(5);
                });

                It("should clear all subscribers", () =>
                {
                    var reactive = new Reactive<int>();
                    int value = 0;

                    reactive.Subscribe(v => value = v);
                    reactive.ClearAll();
                    reactive.Value = 42;

                    Expect(value).ToBe(0); // No subscriber should update the value
                });

                It("should support subscribers receiving initial value", () =>
                {
                    var reactive = new Reactive<int> { Value = 100 };
                    int receivedValue = 0;

                    reactive.Subscribe(v => receivedValue = v);

                    Expect(receivedValue).ToBe(100);
                });

                It("should not notify disposed subscriber", () =>
                {
                    var reactive = new Reactive<int>();
                    int value1 = 0, value2 = 0;

                    var subscription1 = reactive.Subscribe(v => value1 = v);
                    reactive.Subscribe(v => value2 = v);

                    reactive.Value = 7;

                    subscription1.Dispose();
                    reactive.Value = 12;

                    Expect(value1).ToBe(7); // subscription1 should not update anymore
                    Expect(value2).ToBe(12);
                });

                It("should handle setting the same value multiple times", () =>
                {
                    var reactive = new Reactive<int>();
                    int updateCount = 0;

                    reactive.Subscribe(v => updateCount++);
                    reactive.Value = 10;
                    reactive.Value = 10; // Set the same value again

                    Expect(updateCount).ToBe(2); // Should only update once
                });
            });
        }
    }
}
