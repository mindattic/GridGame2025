using Assets.Scripts.Events;
using Assets.Scripts.Models;
using Game.Behaviors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //Quick Reference Properties
    protected AttackLineManager attackLineManager => GameManager.instance.attackLineManager;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected PortraitManager portraitManager => GameManager.instance.portraitManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected HeroManager heroManager => GameManager.instance.heroManager;
    protected EventManager eventManager => GameManager.instance.eventManager;
    protected TimerBar timerBar => GameManager.instance.timerBar;
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;

    //Properties
    public bool isHeroTurn => currentTeam.Equals(Team.Hero);
    public bool isEnemyTurn => currentTeam.Equals(Team.Enemy);
    public bool isStartPhase => currentPhase.Equals(TurnPhase.Start);
    public bool isMovePhase => currentPhase.Equals(TurnPhase.Move);
    public bool isPreAttackPhase => currentPhase.Equals(TurnPhase.PreAttack);
    public bool isAttackPhase => currentPhase.Equals(TurnPhase.Attack);
    public bool isPostAttackPhase => currentPhase.Equals(TurnPhase.PostAttack);
    public bool isEndPhase => currentPhase.Equals(TurnPhase.End);
    public bool isFirstTurn => currentTurn == 0;

    //Fields
    public int currentTurn = 0;
    public Team currentTeam = Team.Hero;
    public TurnPhase currentPhase = TurnPhase.Start;

    public void SetPhase(TurnPhase turnPhase)
    {
        currentPhase = turnPhase;
        OnTurnPhaseChanged(currentPhase); // direct call instead of event
    }

    private void OnTurnPhaseChanged(TurnPhase turnPhase)
    {
        currentPhase = turnPhase;

        if (isHeroTurn)
        {
            if (turnPhase == TurnPhase.Start)
            {
                currentTurn++;
                timerBar.Refill();
                heroManager.TriggerGlow();
            }
        }
        else if (isEnemyTurn)
        {
            if (turnPhase == TurnPhase.Start)
            {
                timerBar.Lock();
                eventManager.Add(new EnemySpawnAwait());

                bool anyReadyEnemies = enemies.Any(x => x.isPlaying && x.hasMaxAP);
                if (!anyReadyEnemies)
                {
                    eventManager.TriggerExecute();
                    NextTurn(); // No enemy ready; immediately switch turn.
                    return;
                }

                eventManager.Add(new EnemyStartAwait());
                eventManager.TriggerExecute();
            }
        }
    }

    public void Initialize()
    {
        currentTurn = 0;
        currentTeam = Team.Hero;
        heroManager.TriggerGlow();
        SetPhase(TurnPhase.Start);
    }

    public void NextTurn()
    {
        currentTeam = isHeroTurn ? Team.Enemy : Team.Hero;
        supportLineManager.Clear();
        attackLineManager.DespawnAll();
        SetPhase(TurnPhase.Start);
    }
}
