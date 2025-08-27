using UnityEngine;

// Summary:
//   Seven-component stat vector in this order:
//   Strength, Vitality, Speed, Stamina, Intelligence, Wisdom, Luck.
[System.Serializable]
public struct VectorStats
{
    public float str;
    public float vit;
    public float agi;
    public float spd;
    public float sta;
    public float intel;
    public float wis;
    public float lck;

    public VectorStats(float str, float vit, float agi, float spd, float sta, float intel, float wis, float lck)
    {
        this.str = str;
        this.vit = vit;
        this.agi = agi;
        this.spd = spd;
        this.sta = sta;
        this.intel = intel;
        this.wis = wis;
        this.lck = lck;
    }

    public static VectorStats operator +(VectorStats a, VectorStats b)
    {
        return new VectorStats(
            a.str + b.str,
            a.vit + b.vit,
            a.agi + b.agi,
            a.spd + b.spd,
            a.sta + b.sta,
            a.intel + b.intel,
            a.wis + b.wis,
            a.lck + b.lck
        );
    }

    public static VectorStats operator *(VectorStats a, float m)
    {
        return new VectorStats(
            a.str * m,
            a.vit * m,
            a.agi * m,
            a.spd * m,
            a.sta * m,
            a.intel * m,
            a.wis * m,
            a.lck * m
        );
    }
}
