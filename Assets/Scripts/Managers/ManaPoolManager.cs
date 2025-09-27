using TMPro;
using UnityEngine;
using g = Assets.Helpers.GameHelper;
using Assets.Helper;

public class ManaPoolManager : MonoBehaviour
{
    [Header("Config")]
    public int maxMana = 100;
    public int heroMana = 0;
    public int enemyMana = 0;

    [Header("Gain Rates")]
    public int perTurnGain = 5;     // passive gain each turn start
    public int onAttackGain = 5;    // gain when dealing damage
    public int onHitGain = 3;       // gain when taking damage

    private TextMeshProUGUI HeroLabel;
    private TextMeshProUGUI EnemyLabel;

    private void Awake()
    {
        HeroLabel = GameObjectHelper.Game.Card.HeroMana;
        EnemyLabel = GameObjectHelper.Game.Card.EnemyMana;

        // Start hero team with 100 MP (clamped to max)
        heroMana = Mathf.Clamp(100, 0, maxMana);
        enemyMana = Mathf.Clamp(enemyMana, 0, maxMana);
        RefreshUI();
    }

    public bool TrySpend(Team team, int cost)
    {
        cost = Mathf.Max(0, cost);
        if (team == Team.Hero)
        {
            if (heroMana < cost) return false;
            heroMana -= cost;
        }
        else
        {
            if (enemyMana < cost) return false;
            enemyMana -= cost;
        }
        RefreshUI();
        return true;
    }

    public void Gain(Team team, int amount)
    {
        amount = Mathf.Max(0, amount);
        if (team == Team.Hero)
            heroMana = Mathf.Clamp(heroMana + amount, 0, maxMana);
        else
            enemyMana = Mathf.Clamp(enemyMana + amount, 0, maxMana);
        RefreshUI();
    }

    public void OnTurnStarted(Team team)
    {
        Gain(team, perTurnGain);
    }

    // Optional hooks you can call where appropriate
    public void OnDealtDamage(Team team) => Gain(team, onAttackGain);
    public void OnTookDamage(Team team) => Gain(team, onHitGain);

    public void RefreshUI()
    {
        if (HeroLabel != null) HeroLabel.text = $"MP: {heroMana}";
        if (EnemyLabel != null) EnemyLabel.text = $"MP: {enemyMana}";
    }
}
