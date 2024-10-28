[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public class ModelAttribute : Attribute
{
    public string Name { get; }
    public ModelType Type { get; }

    public ModelAttribute(string name, ModelType type = ModelType.Unique)
    {
        Name = name;
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public class ModelFieldAttribute : Attribute
{
    public string Type { get; }
    public bool Index { get; }

    public ModelFieldAttribute(string type, bool index = false)
    {
        Type = type;
        Index = index;
    }
}