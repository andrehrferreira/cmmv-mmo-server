public interface IGatherable
{
    string Map { get; set; }
    int X { get; set; }
    int Y { get; set; }
    int Z { get; set; }
    int Timer { get; set; }
    bool RespawnOnStart { get; set; }
    long Timeout { get; set; } 
    List<GatherableType> Entities { get; set; }
    int MeshIndex { get; set; }
    string FoliageId { get; set; }
}