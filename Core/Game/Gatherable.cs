public class ItemResource
{
    public Type Item { get; set; }
    public int Chance { get; set; }
    public int? Max { get; set; }

    public ItemResource(Type item, int chance, int? max = null)
    {
        Item = item;
        Chance = chance;
        Max = max;
    }
}

public class Gatherable
{ 
}
