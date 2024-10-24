public struct Base36
{
    private const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Converts a Base36 string to an integer.
    /// </summary>
    /// <param name="id">The Base36 string to convert.</param>
    /// <returns>An integer representing the converted Base36 value.</returns>
    /// <exception cref="ArgumentException">Thrown when the input string is null, empty, or contains invalid characters.</exception>
    public static int ToInt(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("The input string cannot be null or empty.");

        int result = 0;

        foreach (char c in id.ToUpper())
        {
            int digitValue = Digits.IndexOf(c);

            if (digitValue == -1)
            {
                throw new ArgumentException("Invalid Base36 value.");
            }

            result = result * 36 + digitValue;
        }

        return result;
    }

    /// <summary>
    /// Converts an integer to a Base36 string representation.
    /// </summary>
    /// <param name="value">The integer value to convert. Must be non-negative.</param>
    /// <returns>A Base36 string representation of the integer.</returns>
    /// <exception cref="ArgumentException">Thrown when the input value is negative.</exception>
    public static string ToString(int value)
    {
        if (value < 0) throw new ArgumentException("Value cannot be negative.");

        string result = string.Empty;

        do
        {
            result = Digits[value % 36] + result;
            value /= 36;
        } while (value > 0);

        return result;
    }
}
