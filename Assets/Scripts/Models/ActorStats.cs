[System.Serializable]
public class ActorStats
{
    public float Level;
    public float PreviousHP;
    public float HP;
    public float MaxHP;
    public float PreviousAP;
    public float AP;
    public float MaxAP;
    public float Strength;
    public float Vitality;
    public float Agility;
    public float Speed;
    public float Luck;

    public ActorStats() { }

    public ActorStats(ActorStats other)
    {
        Level = other.Level;
        PreviousHP = other.HP;
        HP = other.HP;
        MaxHP = other.MaxHP;
        PreviousAP = 0;
        AP = 0;
        MaxAP = 100;
        Strength = other.Strength;
        Vitality = other.Vitality;
        Agility = other.Agility;
        Speed = other.Speed;
        Luck = other.Luck;
    }
}
