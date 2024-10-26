public struct SkillValue : ISkillValue
{
    public int Value { get; set; }
    public int Cap { get; set; }
    public int Experience { get; set; }

    public SkillValue(int value = 0, int cap = 10, int experience = 0)
    {
        Value = value;
        Cap = cap;
        Experience = experience;
    }
}

public struct SkillLevelExperience : ILevelExperience
{
    public int Level { get; set; }
    public int Experience { get; set; }

    public SkillLevelExperience(int level, int experience)
    {
        Level = level;
        Experience = experience;
    }
}