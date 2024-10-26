public struct StateFlags
{
    private int Flags;

    public StateFlags(int initialFlags = 0)
    {
        Flags = initialFlags;
    }

    public StateFlags(EntityStates initialFlags = 0)
    {
        Flags = (int)initialFlags;
    }

    public StateFlags(BuffsStates initialFlags = 0)
    {
        Flags = (int)initialFlags;
    }

    public StateFlags(ItemStates initialFlags = 0)
    {
        Flags = (int)initialFlags;
    }

    public void AddFlag(EntityStates flag)
    {
        Flags |= (int)flag;
    }

    public void AddFlag(BuffsStates flag)
    {
        Flags |= (int)flag;
    }

    public void AddFlag(ItemStates flag)
    {
        Flags |= (int)flag;
    }

    public void RemoveFlag(EntityStates flag)
    {
        Flags &= ~(int)flag;
    }

    public void RemoveFlag(BuffsStates flag)
    {
        Flags &= ~(int)flag;
    }

    public void RemoveFlag(ItemStates flag)
    {
        Flags &= ~(int)flag;
    }

    public bool HasFlag(EntityStates flag)
    {
        return (Flags & (int)flag) == (int)flag;
    }

    public bool HasFlag(BuffsStates flag)
    {
        return (Flags & (int)flag) == (int)flag;
    }

    public bool HasFlag(ItemStates flag)
    {
        return (Flags & (int)flag) == (int)flag;
    }

    public bool DontHasFlag(EntityStates flag)
    {
        return !HasFlag(flag);
    }

    public bool DontHasFlag(BuffsStates flag)
    {
        return !HasFlag(flag);
    }

    public bool DontHasFlag(ItemStates flag)
    {
        return !HasFlag(flag);
    }

    public int GetCurrentFlags()
    {
        return Flags;
    }
}