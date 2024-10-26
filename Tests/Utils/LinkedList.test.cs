namespace Tests
{
    public class LinkedListTests : AbstractTest
    {
        public LinkedListTests()
        {
            Describe("LinkedList", () =>
            {
                It("should append elements correctly", () =>
                {
                    var list = new LinkedList<int>();
                    list.Append(1);
                    list.Append(2);
                    list.Append(3);

                    var currentNode = list.GetHead();
                    Expect(currentNode.Data).ToBe(1);
                    Expect(currentNode.Next.Data).ToBe(2);
                    Expect(currentNode.Next.Next.Data).ToBe(3);
                });

                It("should prepend elements correctly", () =>
                {
                    var list = new LinkedList<int>();
                    list.Prepend(3);
                    list.Prepend(2);
                    list.Prepend(1);

                    var currentNode = list.GetHead();
                    Expect(currentNode.Data).ToBe(1);
                    Expect(currentNode.Next.Data).ToBe(2);
                    Expect(currentNode.Next.Next.Data).ToBe(3);
                });

                It("should remove an element correctly", () =>
                {
                    var list = new LinkedList<int>();
                    list.Append(1);
                    list.Append(2);
                    list.Append(3);

                    list.Remove(2);

                    var currentNode = list.GetHead();
                    Expect(currentNode.Data).ToBe(1);
                    Expect(currentNode.Next.Data).ToBe(3);
                    Expect(currentNode.Next.Next).ToBeNull();
                });

                It("should remove the head element correctly", () =>
                {
                    var list = new LinkedList<int>();
                    list.Append(1);
                    list.Append(2);
                    list.Append(3);

                    list.Remove(1);

                    var currentNode = list.GetHead();
                    Expect(currentNode.Data).ToBe(2);
                    Expect(currentNode.Next.Data).ToBe(3);
                });

                It("should handle removing an element that does not exist", () =>
                {
                    var list = new LinkedList<int>();
                    list.Append(1);
                    list.Append(2);
                    list.Append(3);

                    list.Remove(4); // Element not in the list

                    var currentNode = list.GetHead();
                    Expect(currentNode.Data).ToBe(1);
                    Expect(currentNode.Next.Data).ToBe(2);
                    Expect(currentNode.Next.Next.Data).ToBe(3);
                });

                It("should handle removing an element from an empty list", () =>
                {
                    var list = new LinkedList<int>();

                    list.Remove(1); // Attempt to remove from an empty list

                    Expect(list.GetHead()).ToBeNull();
                });

                It("should maintain integrity after multiple operations", () =>
                {
                    var list = new LinkedList<int>();
                    list.Append(2);
                    list.Prepend(1);
                    list.Append(3);
                    list.Remove(2);
                    list.Prepend(0);

                    var currentNode = list.GetHead();
                    Expect(currentNode.Data).ToBe(0);
                    Expect(currentNode.Next.Data).ToBe(1);
                    Expect(currentNode.Next.Next.Data).ToBe(3);
                });
            });
        }
    }
}
