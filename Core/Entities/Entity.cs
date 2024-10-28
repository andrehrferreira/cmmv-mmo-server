using System.Reactive.Subjects;
using System.ComponentModel;

public partial class Entity: IEquatable<Entity>, IComparable<Entity>
{
    public static Dictionary<string, Func<object>> Entities = new Dictionary<string, Func<object>>();
    public static Dictionary<string, Func<Player, object>> Summons = new Dictionary<string, Func<Player, object>>();
    public static Dictionary<string, Func<Player, object>> Pets = new Dictionary<string, Func<Player, object>>();

    //Network
    public string ConnId { get; set; }
    public Connection Conn { get; set; }
    public string CharacterId { get; set; }
    public long LastUpdate { get; set; }
    public bool Admin { get; set; }
    public bool Removed { get; set; }

    //Map / Position
    public Maps Map { get; set; }
    public string MapIndex { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 RespawnPosition { get; set; }
    public Respawn Respawn { get; set; }
    public int MovementDistance { get; set; } = 600;
    public int MaxDistanceToRespawn { get; set; } = 3000;
    public int Speed { get; set; } = 700;

    // Character info
    [Reply(ReplyType.AreaOfInterest, ReplyDataType.Index, ReplyTransformData.Base36)]
    public string Id { get; set; }

    [Reply(ReplyType.AreaOfInterest, ReplyDataType.Immutable)]
    public string Name { get; set; }
    
    public virtual bool IsCreature { get; set; } = false;
    public List<Entity> AreaOfInterest = new List<Entity>();

    [Reply(ReplyType.AreaOfInterest, ReplyDataType.Variable, ReplyTransformData.Flag)]
    public StateFlags States { get; set; } = new StateFlags((int)EntityStates.None);

    [Reply(ReplyType.AreaOfInterest, ReplyDataType.Variable, ReplyTransformData.Flag)]
    public StateFlags BuffsState { get; set; } = new StateFlags((int)BuffsStates.None);

    public List<Condition> Conditions { get; set; } = new List<Condition>();
    public EntitiesKind Kind { get; set; } = EntitiesKind.None;
    public Team Team;
    public Entity TeamOwner = null;
    public Dictionary<string, int> DamageCauser = new Dictionary<string, int>();
    public bool InAction = false;
    public bool InHealAction = false;
    public bool DestroyOnDie = false;
    public Guild Guild = null;

    //Target
    public string Target = null;
    public Entity TargetActor = null;
    public IDisposable TargetOnDie { get; set; }
    public IDisposable TargetOnDestroy { get; set; }

    //Loot

    //Stats
    public int StatsPoints { get; set; } = 0;
    public int StatsCap { get; set; } = 225;
    public int Str { get; set; } = 0;
    public int Dex { get; set; } = 0;
    public int Int { get; set; } = 0;
    public int Vig { get; set; } = 0;
    public int Agi { get; set; } = 0;
    public int Luc { get; set; } = 0;
    public int Life { get; set; } = 0;
    public int MaxLife { get; set; } = 0;
    public int LifeByte { get; set; } = 0;
    public int Mana { get; set; } = 0;
    public int MaxMana { get; set; } = 0;
    public int Stamina { get; set; } = 0;
    public int MaxStamina { get; set; } = 0;
    public int BonusStr { get; set; } = 0;
    public int BonusDex { get; set; } = 0;
    public int BonusInt { get; set; } = 0;
    public int BonusVig { get; set; } = 0;
    public int BonusAgi { get; set; } = 0;
    public int BonusLuc { get; set; } = 0;

    public bool FixedLife { get; set; } = false;
    public Dictionary<SkillName, ISkillValue> Skills { get; set; } = new Dictionary<SkillName, ISkillValue>();
    public Container Inventory { get; set; }
    public int Karma { get; set; } = 0;
    public bool IsDead { get; set; } = false;
    public bool Sprint { get; set; } = false;

    // Resistences
    public int PhysicalResistance { get; set; } = 0;
    public int FireResistance { get; set; } = 0;
    public int ColdResistance { get; set; } = 0;
    public int PoisonResistance { get; set; } = 0;
    public int EnergyResistance { get; set; } = 0;
    public int LightResistance { get; set; } = 0;
    public int DarkResistance { get; set; } = 0;
    public int BonusPhysicalResistance { get; set; } = 0;
    public int BonusFireResistance { get; set; } = 0;
    public int BonusColdResistance { get; set; } = 0;
    public int BonusPoisonResistance { get; set; } = 0;
    public int BonusEnergyResistance { get; set; } = 0;
    public int BonusLightResistance { get; set; } = 0;
    public int BonusDarkResistance { get; set; } = 0;

    // Statics
    public int LifeRegeneration { get; set; } = 0;
    public int ManaRegeneration { get; set; } = 0;
    public int StaminaRegeneration { get; set; } = 0;
    public int BonusPhysicalDamage { get; set; } = 0;
    public int BonusMagicDamage { get; set; } = 0;
    public string WeaponDamage { get; set; } = "";
    public int WeaponSpeed { get; set; } = 0;
    public int CriticalChance { get; set; } = 0;
    public int CriticalDamage { get; set; } = 0;
    public int Armor { get; set; } = 0;
    public int DamageReduction { get; set; } = 0;
    public int DodgeChance { get; set; } = 0;
    public int ReflectionPhysicalDamage { get; set; } = 0;
    public int ReflectionMagicDamage { get; set; } = 0;
    public int LowerManaCost { get; set; } = 0;
    public int LowerStamCost { get; set; } = 0;
    public int FasterCasting { get; set; } = 0;
    public int CooldownReduction { get; set; } = 0;

    // Elemental Damage
    public int FireDamage { get; set; } = 0;
    public int ColdDamage { get; set; } = 0;
    public int EnergyDamage { get; set; } = 0;
    public int PoisonDamage { get; set; } = 0;
    public int LightDamage { get; set; } = 0;
    public int DarkDamage { get; set; } = 0;

    // Bonus collect
    public int BonusCollectsMineral { get; set; } = 0;
    public int BonusCollectsSkins { get; set; } = 0;
    public int BonusCollectsWood { get; set; } = 0;

    // Events
    public Subject<Entity> OnDie { get; } = new Subject<Entity>();
    public Subject<Entity> OnDestroy { get; } = new Subject<Entity>();
    public Subject<Condition> OnConditionChanged { get; } = new Subject<Condition>();

    public bool Equals(Entity other)
    {
        return Id == other.Id || (MapIndex == other.MapIndex && Map.Equals(other.Map));
    }

    public int CompareTo(Entity other)
    {
        return Id.CompareTo(other.Id);
    }

    public static bool operator ==(Entity left, Entity right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !left.Equals(right);
    }

    //Network
    public void Reply(ServerPacket packetType, ByteBuffer data, bool Queue = false, bool encryptData = false)
    {
        var entities = new HashSet<Entity>(AreaOfInterest) { this };

        foreach (var entity in entities) { 
            if (entity.Conn != null)
            {
                if (Queue)
                    QueueBuffer.AddBuffer(packetType, entity.Conn.Id, data);
                else
                    entity.Conn.Send(packetType, data.GetBuffer(), encryptData);
            }
        };
    }
}