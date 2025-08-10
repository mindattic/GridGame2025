using System;

[Serializable]
public class ActorStats : BaseStats
{
    public float Level = 1f;

    public float PreviousHP;
    public float HP;
    public float MaxHP;

    public float PreviousAP;
    public float AP;
    public float MaxAP;

    public ActorStats() { }

    public ActorStats(ActorStats other)
    {
        if (other == null) return;

        Level = other.Level;

        PreviousHP = other.HP;
        HP = other.HP;
        MaxHP = other.MaxHP;

        PreviousAP = 0f;
        AP = 0f;
        MaxAP = 100f;

        Strength = other.Strength;
        Vitality = other.Vitality;
        Agility = other.Agility;
        Stamina = other.Stamina;
        Intelligence = other.Intelligence;
        Wisdom = other.Wisdom;
        Luck = other.Luck;
    }
}
