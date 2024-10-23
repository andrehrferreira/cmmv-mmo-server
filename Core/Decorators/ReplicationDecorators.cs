[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public class ReplyAttribute : Attribute
{
    public ReplyType Type { get; }
    public ReplyDataType DataType { get; }
    public ReplyTransformData Transform { get; }

    public ReplyAttribute(
        ReplyType type, 
        ReplyDataType dataType = ReplyDataType.Variable,
        ReplyTransformData transform = ReplyTransformData.Literal
    )
    {
        Type = type;
        DataType = dataType;
        Transform = transform;
    }
}