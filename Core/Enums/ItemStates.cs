[Flags]
public enum ItemStates
{
    None = 0,
    Blessed = 1 << 0,
    Insured = 1 << 1,
    Exceptional = 1 << 2,
    SpellChanneling = 1 << 3,
    Broken = 1 << 4
}
