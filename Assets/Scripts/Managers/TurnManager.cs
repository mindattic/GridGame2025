using Assets.Scripts.Models;
using Game.Behaviors;
using System;
using System.Collections;
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
    protected TimerBarInstance timerBar => GameManager.instance.timerBar;
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;

    public bool isPlayerTurn => currentTeam.Equals(Team.Player);
    public bool isEnemyTurn => currentTeam.Equals(Team.Enemy);
    public bool isStartPhase => CurrentPhase.Equals(TurnPhase.Start);
    public bool isMovePhase => CurrentPhase.Equals(TurnPhase.Move);
    public bool isPreAttackPhase => CurrentPhase.Equals(TurnPhase.PreAttack);
    public bool isAttackPhase => CurrentPhase.Equals(TurnPhase.Attack);
    public bool isPostAttackPhase => CurrentPhase.Equals(TurnPhase.PostAttack);
    public bool isEndPhase => CurrentPhase.Equals(TurnPhase.End);
    public bool isFirstTurn => currentTurn == 1;

    // Internal properties
    public int currentTurn = 1;
    public Team currentTeam = Team.Player;

    // Instead of a public field for the current phase, we create a private backing field and a public property.
    private TurnPhase phase = TurnPhase.Start;
    public TurnPhase CurrentPhase
    {
        get { return phase; }
        set
        {
            if (phase != value)
            {
                phase = value;
                // Fire the event when the phase changes.
                OnTurnPhaseChanged?.Invoke(phase);
            }
        }
    }

    // Event that fires whenever the turn phase changes.
    public event Action<TurnPhase> OnTurnPhaseChanged;

    // Phase action lists (used on enemy turns)
    private List<TurnAction> startPhaseActions = new List<TurnAction>();
    private List<TurnAction> movePhaseActions = new List<TurnAction>();
    private List<TurnAction> preAttackPhaseActions = new List<TurnAction>();
    private List<TurnAction> attackPhaseActions = new List<TurnAction>();
    private List<TurnAction> postAttackPhaseActions = new List<TurnAction>();
    private List<TurnAction> endPhaseActions = new List<TurnAction>();

    void Start()
    {
        // Optionally initialize the turn here.
    }

    public void Initialize()
    {
        currentTurn = 1;
        currentTeam = Team.Player;
        CurrentPhase = TurnPhase.Start; // This will fire OnTurnPhaseChanged.
        // For example, trigger glow for all active players.
        players.Where(x => x.isActive && x.isAlive).ToList().ForEach(x => x.glow.TriggerGlow());
    }

    // Add an action to a specific phase.
    public void AddActionToPhase(TurnPhase phase, TurnAction action)
    {
        switch (phase)
        {
            case TurnPhase.Start:
                startPhaseActions.Add(action);
                break;
            case TurnPhase.Move:
                movePhaseActions.Add(action);
                break;
            case TurnPhase.PreAttack:
                preAttackPhaseActions.Add(action);
                break;
            case TurnPhase.Attack:
                attackPhaseActions.Add(action);
                break;
            case TurnPhase.PostAttack:
                postAttackPhaseActions.Add(action);
                break;
            case TurnPhase.End:
                endPhaseActions.Add(action);
                break;
        }
    }

    public void ResetSortingOrder()
    {
        foreach (var actor in actors.Where(x => x.isActive && x.isAlive))
        {
            actor.sortingOrder = SortingOrder.Default;
        }
    }

    public void NextTurn()
    {
        // Switch team for the next turn.
        currentTeam = isPlayerTurn ? Team.Enemy : Team.Player;
        // Set phase to Start (which will fire the OnTurnPhaseChanged event).
        CurrentPhase = TurnPhase.Start;

        supportLineManager.Clear();
        attackLineManager.Clear();
        ResetSortingOrder();

        if (isEnemyTurn)
        {
            // For enemy turns, automatically add phase actions and run ExecuteTurn().
            timerBar.Lock();
            AddActionToPhase(TurnPhase.Start, new EnemySpawnAction());
            AddActionToPhase(TurnPhase.Start, new EnemyStartAction());
            StartCoroutine(ExecuteTurn());
        }
        else if (isPlayerTurn)
        {
            // For player turns, we want to pause in the Move phase.
            currentTurn++;
            timerBar.Refill();
            players.Where(x => x.isActive && x.isAlive).ToList().ForEach(x => x.glow.TriggerGlow());
            // Do NOT start ExecuteTurn() automatically here.
            // Instead, SelectedPlayerManager or another system will eventually call turnManager.NextTurn() when the player has completed their move.
        }
    }

    public void TriggerExecuteTurn()
    {
        StartCoroutine(ExecuteTurn());
    }


    // Execute all phases in order (used for enemy turns)
    private IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(ResolvePhase(startPhaseActions));
        yield return StartCoroutine(ResolvePhase(movePhaseActions));
        yield return StartCoroutine(ResolvePhase(preAttackPhaseActions));
        yield return StartCoroutine(ResolvePhase(attackPhaseActions));
        yield return StartCoroutine(ResolvePhase(postAttackPhaseActions));
        yield return StartCoroutine(ResolvePhase(endPhaseActions));

        NextTurn();
    }

    // Execute a specific phase's actions.
    private IEnumerator ResolvePhase(List<TurnAction> actions)
    {
        foreach (TurnAction action in actions)
        {
            yield return StartCoroutine(action.Execute());
        }
        actions.Clear();
    }
}
