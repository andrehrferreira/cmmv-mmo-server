
public abstract class AbstractTest
{
    protected void Describe(string description, Action suite)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"- {description}");
        Console.ResetColor();
        suite();
    }

    protected bool It(string description, Action test)
    {
        try
        {
            test();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[PASS] {description}");
            Console.ResetColor();
            return true;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[FAIL] {description}: {ex.Message}");
            Console.WriteLine(ex);
            Console.ResetColor();
            throw new Exception("");
        }
    }

    protected Expectation<T> Expect<T>(T actual)
    {
        return new Expectation<T>(actual);
    }
}

public class Expectation<T>
{
    private readonly T _actual;

    public Expectation(T actual)
    {
        _actual = actual;
    }

    public void ToBe(T expected)
    {
        if (!Equals(_actual, expected))
        {
            throw new Exception($"Expected: {expected}, but got: {_actual}");
        }
    }

    public void NotToBe(T unexpected)
    {
        if (Equals(_actual, unexpected))
        {
            throw new Exception($"Did not expect: {unexpected}, but got: {_actual}");
        }
    }

    public void NotToBeNull()
    {
        if (_actual == null)
        {
            throw new Exception("Expected value to not be null, but it was null.");
        }
    }

    public void ToBeTrue()
    {
        if (!(_actual is bool boolean) || !boolean)
        {
            throw new Exception($"Expected value to be true, but got: {_actual}");
        }
    }

    public void ToBeFalse()
    {
        if (!(_actual is bool boolean) || boolean)
        {
            throw new Exception($"Expected value to be false, but got: {_actual}");
        }
    }

    public void ToContain<U>(U item)
    {
        if (_actual is IEnumerable<U> collection)
        {
            if (!collection.Contains(item))
            {
                throw new Exception($"Expected collection to contain: {item}, but it did not.");
            }
        }
        else
        {
            throw new Exception("The actual value is not a collection that supports containment.");
        }
    }

    public void ToBeGreaterThan<U>(U value) where U : IComparable
    {
        if (_actual is not IComparable comparable)
        {
            throw new Exception($"The actual value does not support comparison.");
        }

        if (comparable.CompareTo(value) <= 0)
        {
            throw new Exception($"Expected value to be greater than: {value}, but got: {_actual}");
        }
    }

    public void ToBeLessThan<U>(U value) where U : IComparable
    {
        if (_actual is not IComparable comparable)
        {
            throw new Exception($"The actual value does not support comparison.");
        }

        if (comparable.CompareTo(value) >= 0)
        {
            throw new Exception($"Expected value to be less than: {value}, but got: {_actual}");
        }
    }
}
