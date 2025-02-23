using Assets.Scripts.Models;
using Game.Behaviors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    // External properties
    protected AttackLineManager attackLineManager => GameManager.instance.attackLineManager;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    protected PortraitManager portraitManager => GameManager.instance.portraitManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected PlayerManager playerManager => GameManager.instance.playerManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;

    protected TimerBarInstance timerBar => GameManager.instance.timerBar;
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;

    public bool isPlayerTurn => currentTeam.Equals(Team.Player);
    public bool isEnemyTurn => currentTeam.Equals(Team.Enemy);
    public bool isStartPhase => currentTurnPhase.Equals(TurnPhase.Start);
    public bool isMovePhase => currentTurnPhase.Equals(TurnPhase.Move);
    public bool isPreAttackPhase => currentTurnPhase.Equals(TurnPhase.PreAttack);
    public bool isAttackPhase => currentTurnPhase.Equals(TurnPhase.Attack);
    public bool isPostAttackPhase => currentTurnPhase.Equals(TurnPhase.PostAttack);
    public bool isEndPhase => currentTurnPhase.Equals(TurnPhase.End);
    public bool isFirstTurn => currentTurn == 1;

    //System.Action event handlers
    public event System.Action<TurnPhase> OnTurnPhaseChanged;

    //Fields
    public int currentTurn = 1;
    public Team currentTeam = Team.Player;
    public TurnPhase currentTurnPhase = TurnPhase.Start;

    public void SetPhase(TurnPhase turnPhase)
    {
        currentTurnPhase = turnPhase;
        OnTurnPhaseChanged?.Invoke(currentTurnPhase);
    }

    void Start()
    {
        // Subscribe to the TurnManager's OnTurnPhaseChanged event.
        OnTurnPhaseChanged += (TurnPhase turnPhase) =>
        {
            currentTurnPhase = turnPhase;

            if (isPlayerTurn)
            {
                switch (currentTurnPhase)
                {
                    case TurnPhase.Start:
                        // For player turns, pause in the Move phase.
                        currentTurn++;
                        timerBar.Refill();
                        playerManager.TriggerGlow();
                        break;
                    case TurnPhase.Attack:
                        actionManager.TriggerExecute();
                        break;
                }
            }
            else if (isEnemyTurn)
            {
                switch (currentTurnPhase)
                {
                    case TurnPhase.Start:
                        // For enemy turns, automatically add phase actions and run Execute().
                        timerBar.Lock();
                        actionManager.Add(new EnemySpawnAction());

                        bool anyReadyEnemies = enemies.Any(x => x.isPlaying && x.hasMaxAP);
                        if (!anyReadyEnemies)
                        {
                            actionManager.TriggerExecute();
                            break;
                        }

                        actionManager.Add(new EnemyStartAction());
                        actionManager.TriggerExecute();
                        break;
                }
            }
        };
    }

    public void Initialize()
    {
        currentTurn = 1;
        currentTeam = Team.Player;
        playerManager.TriggerGlow();
        SetPhase(TurnPhase.Start);
    }



    public void ResetSortingOrder()
    {
        foreach (var actor in actors.Where(x => x.isPlaying))
        {
            actor.sortingOrder = SortingOrder.Default;
        }
    }

    public void NextTurn()
    {
        // Switch team for the next turn.
        currentTeam = isPlayerTurn ? Team.Enemy : Team.Player;
        supportLineManager.Clear();
        attackLineManager.Clear();
        ResetSortingOrder();
        SetPhase(TurnPhase.Start);
    }



}
