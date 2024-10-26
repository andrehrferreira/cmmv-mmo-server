
namespace Tests
{
    public class NetworkEventsTests : AbstractTest
    {
        public NetworkEventsTests()
        {
            Describe("NetworkEvents System", () =>
            {
                It("should notify subscribers when event is emitted", () =>
                {
                    var networkEvents = new NetworkEvents<string>();
                    string receivedData = null;
                    Connection receivedSocket = null;

                    networkEvents.Subscribe((data, socket) =>
                    {
                        receivedData = data;
                        receivedSocket = socket;
                    });

                    var socket = new Connection();
                    networkEvents.Emit("Test Data", socket);

                    Expect(receivedData).ToBe("Test Data");
                    Expect(receivedSocket).NotToBeNull();
                    Expect(receivedSocket).ToBe(socket);
                });

                It("should unsubscribe properly and not receive events", () =>
                {
                    var networkEvents = new NetworkEvents<int>();
                    int value = 0;

                    var subscription = networkEvents.Subscribe((data, socket) => value = data);

                    networkEvents.Emit(42, null);
                    Expect(value).ToBe(42);

                    subscription.Dispose();

                    networkEvents.Emit(100, null);
                    Expect(value).ToBe(42); // Value should not change as the subscriber was unsubscribed
                });

                It("should clear all subscribers and not receive events", () =>
                {
                    var networkEvents = new NetworkEvents<double>();
                    double value = 0.0;

                    networkEvents.Subscribe((data, socket) => value = data);
                    networkEvents.Subscribe((data, socket) => value = data * 2);

                    networkEvents.Emit(3.14, null);
                    Expect(value).ToBe(6.28); // The last subscriber should set value to 6.28

                    networkEvents.ClearAll();

                    networkEvents.Emit(1.0, null);
                    Expect(value).ToBe(6.28); // Value should not change as all subscribers were cleared
                });

                It("should handle multiple subscribers correctly", () =>
                {
                    var networkEvents = new NetworkEvents<string>();
                    List<string> receivedMessages = new List<string>();

                    networkEvents.Subscribe((data, socket) => receivedMessages.Add("Subscriber 1: " + data));
                    networkEvents.Subscribe((data, socket) => receivedMessages.Add("Subscriber 2: " + data));

                    networkEvents.Emit("Test", null);

                    Expect(receivedMessages.Count).ToBe(2);
                    Expect(receivedMessages[0]).ToBe("Subscriber 1: Test");
                    Expect(receivedMessages[1]).ToBe("Subscriber 2: Test");
                });

                It("should handle empty socket correctly", () =>
                {
                    var networkEvents = new NetworkEvents<string>();
                    string receivedData = null;

                    networkEvents.Subscribe((data, socket) =>
                    {
                        receivedData = data;
                    });

                    networkEvents.Emit("Test with Null Socket", null);
                    Expect(receivedData).ToBe("Test with Null Socket");
                });
            });
        }
    }
}
