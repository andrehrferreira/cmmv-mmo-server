
public struct Base36
{
    public static int ToInt(string Id)
    {
        return Convert.ToInt32(Id, 36);
    }

    public static string ToString(int value)
    {
        return value.ToString("x").ToUpper();
    }
}
