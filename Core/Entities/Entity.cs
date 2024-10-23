

partial class Entity 
{
    [Reply(ReplyType.AreaOfInterest, ReplyDataType.Index, ReplyTransformData.Base36)]
    public Reactive<string> Id { get; set; } = new Reactive<string>();

    [Reply(ReplyType.AreaOfInterest, ReplyDataType.Immutable)]
    public Reactive<string> Name { get; set; } = new Reactive<string>();
}