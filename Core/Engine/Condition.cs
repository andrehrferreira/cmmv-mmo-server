public class Condition
{
    public Entity Dealer { get; private set; }
    public double Lifetime { get; private set; }
    public ConditionType Type { get; private set; }
    public Dices Value { get; private set; }
    public int DamageBonus { get; private set; } = 0;

    public Condition(ConditionType type, double lifetime, Entity dealer, Dices value = Dices.None, int damageBonus = 0)
    {
        Type = type;
        Lifetime = lifetime * 1000;
        Dealer = dealer;
        Value = value;
        DamageBonus = damageBonus;
    }

    public void Apply(Entity c)
    {
        switch (Type)
        {
            case ConditionType.Burning:
                c.States.AddFlag(EntityStates.Burning);
                break;
            case ConditionType.Bleeding:
                c.States.AddFlag(EntityStates.Bleeding);
                break;
            case ConditionType.Poisoned:
                c.States.AddFlag(EntityStates.Poisoned);
                break;
            case ConditionType.Slowed:
                break;
            case ConditionType.Frozen:
                c.States.AddFlag(EntityStates.Frozen);
                break;
            case ConditionType.Stunned:
                c.States.AddFlag(EntityStates.Stunned);
                break;
            case ConditionType.Feared:
                c.States.AddFlag(EntityStates.Feared);
                break;
        }
    }
}