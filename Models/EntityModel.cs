[Model("Entity", ModelType.List)]
public struct EntityModel
{
    [ModelField("id", true)]
    public string Id;

    [ModelField("int")]
    public int State;

    [ModelField("vector3")]
    public Vector3 Position;
}