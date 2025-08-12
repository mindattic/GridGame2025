using UnityEngine;

// Summary:
//   Seven-component stat vector in this order:
//   Strength, Vitality, Agility, Stamina, Intelligence, Wisdom, Luck.
[System.Serializable]
public struct Vector7
{
    public float str;
    public float vit;
    public float agi;
    public float sta;
    public float intel;
    public float wis;
    public float lck;

    public Vector7(float str, float vit, float agi, float sta, float intel, float wis, float lck)
    {
        this.str = str;
        this.vit = vit;
        this.agi = agi;
        this.sta = sta;
        this.intel = intel;
        this.wis = wis;
        this.lck = lck;
    }

    public static Vector7 operator +(Vector7 a, Vector7 b)
    {
        return new Vector7(
            a.str + b.str,
            a.vit + b.vit,
            a.agi + b.agi,
            a.sta + b.sta,
            a.intel + b.intel,
            a.wis + b.wis,
            a.lck + b.lck
        );
    }

    public static Vector7 operator *(Vector7 a, float m)
    {
        return new Vector7(
            a.str * m,
            a.vit * m,
            a.agi * m,
            a.sta * m,
            a.intel * m,
            a.wis * m,
            a.lck * m
        );
    }
}
