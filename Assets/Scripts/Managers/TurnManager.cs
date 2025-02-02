using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using Game.Behaviors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Phase = TurnPhase;

public class TurnManager : MonoBehaviour
{
    //External properties
    protected AttackLineManager attackLineManager => GameManager.instance.attackLineManager;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected CombatManager combatManager => GameManager.instance.combatManager;
    protected BoardOverlay boardOverlay => GameManager.instance.boardOverlay;
    //protected CombatParticipants participants { get => GameManager.instance.participants; set => GameManager.instance.participants = value; }
    protected PortraitManager portraitManager => GameManager.instance.portraitManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected TimerBarInstance timerBar => GameManager.instance.timerBar;
    protected List<ActorInstance> actors { get => GameManager.instance.actors; set => GameManager.instance.actors = value; }
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;
    public bool isPlayerTurn => currentTeam.Equals(Team.Player);
    public bool isEnemyTurn => currentTeam.Equals(Team.Enemy);
    public bool isStartPhase => currentPhase.Equals(Phase.Start);
    public bool isMovePhase => currentPhase.Equals(Phase.Move);
    public bool isAttackPhase => currentPhase.Equals(Phase.Attack);
    public bool isFirstTurn => currentTurn == 1;

    //Internal properties

    //Fields
    [SerializeField] public int currentTurn = 1;
    [SerializeField] public Team currentTeam = Team.Player;
    [SerializeField] public Phase currentPhase = Phase.Start;


    // These lists hold actions that are scheduled for each phase.
    private List<CombatAction> startPhaseActions = new List<CombatAction>();
    private List<CombatAction> movePhaseActions = new List<CombatAction>();
    private List<CombatAction> preAttackPhaseActions = new List<CombatAction>();
    private List<CombatAction> attackPhaseActions = new List<CombatAction>();
    private List<CombatAction> postAttackPhaseActions = new List<CombatAction>();
    private List<CombatAction> endPhaseActions = new List<CombatAction>();



    //Method which is automatically called before the first frame update  
    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        currentTurn = 1;
        currentTeam = Team.Player;
        currentPhase = Phase.Start;
        players.Where(x => x.isActive && x.isAlive).ToList().ForEach(x => x.glow.TriggerGlow());
        //musicSource.Stop();
        //musicSource.PlayOneShot(resourceManager.MusicTrack($"MelancholyLull"));
    }


    private IEnumerator ExecuteTurn()
    {
        // For example, resolve phases in the following order:
        yield return StartCoroutine(ResolvePhase(startPhaseActions));
        yield return StartCoroutine(ResolvePhase(movePhaseActions));
        yield return StartCoroutine(ResolvePhase(preAttackPhaseActions));
        yield return StartCoroutine(ResolvePhase(attackPhaseActions));
        yield return StartCoroutine(ResolvePhase(postAttackPhaseActions));
        yield return StartCoroutine(ResolvePhase(endPhaseActions));

        // After all phases, proceed to the next turn.
        NextTurn();
    }

    // Generic method to execute all actions for a given phase.
    // You can decide if they should run sequentially or concurrently.
    private IEnumerator ResolvePhase(List<CombatAction> actions)
    {
        // Option 1: Sequential execution (each action waits for the previous one to complete)
        foreach (CombatAction action in actions)
        {
            yield return StartCoroutine(action.Execute());
        }

        // Option 2: Concurrent execution (if actions do not depend on one another)
        // List<Coroutine> runningActions = new List<Coroutine>();
        // foreach (CombatAction action in actions)
        // {
        //     runningActions.Add(StartCoroutine(action.Execute()));
        // }
        // // Wait until all concurrent actions are complete.
        // foreach (Coroutine routine in runningActions)
        // {
        //     yield return routine;
        // }
    }

    public void NextTurn()
    {
        currentTeam = isPlayerTurn ? Team.Enemy : Team.Player;
        currentPhase = Phase.Start;

        supportLineManager.Clear();
        attackLineManager.Clear();
        combatManager.Clear();

        //Reset actor sorting
        actors.ForEach(x => x.sortingOrder = SortingOrder.Default);

        if (isPlayerTurn)
        {
            currentTurn++;
            timerBar.TriggerInitialize();
            players.Where(x => x.isActive && x.isAlive).ToList().ForEach(x => x.glow.TriggerGlow());
        }
        else if (isEnemyTurn)
        {
            timerBar.Hide();

            CheckEnemySpawn();
            ExecuteEnemyMove();
        }
    }

    #region Enemy Attack Methods


    private void CheckEnemySpawn()
    {
        //Check abort conditions
        if (!isEnemyTurn || !isStartPhase)
            return;

        var spawnableEnemies = enemies.Where(x => x.isSpawnable).ToList();
        foreach (var enemy in spawnableEnemies)
        {
            enemy.Spawn(Random.UnoccupiedLocation);
        }
    }

    private void ExecuteEnemyMove()
    {
        StartCoroutine(EnemyMove());
    }

    private IEnumerator EnemyMove()
    {
        //Check abort conditions
        if (!isEnemyTurn || !isStartPhase)
            yield break;

        currentPhase = Phase.Move;

        var readyEnemies = enemies.Where(x => x.isActive && x.isAlive && x.hasMaxAP).ToList();
        if (readyEnemies.Count > 0)
        {
            yield return Wait.For(Intermission.Before.Enemy.Move);

            foreach (var enemy in readyEnemies)
            {
                //enemy.render.SetParallaxSpeed(0.25f, 0.25f);
     
                enemy.CalculateAttackStrategy();
                yield return enemy.move.MoveTowardDestination();
            }

            currentPhase = Phase.Attack;
            ExecuteEnemyAttack();
        }
        else
        {
            NextTurn();
        }


    }





    private void ExecuteEnemyAttack()
    {
        StartCoroutine(EnemyAttack());
    }

    private IEnumerator EnemyAttack()
    {
        //Check abort conditions
        if (!isEnemyTurn || !isAttackPhase)
            yield break;

        var readyEnemies = enemies.Where(x => x.isActive && x.isAlive && x.hasMaxAP).ToList();
        if (readyEnemies.Count < 1)
            yield break;

        yield return Wait.For(Intermission.Before.Enemy.Attack);

        foreach (var enemy in readyEnemies)
        {
            var defendingPlayers = players.Where(x => x.isActive && x.isAlive && x.IsAdjacentTo(enemy.location)).ToList();
            if (defendingPlayers.Count < 1)
                continue;

            foreach (var player in defendingPlayers)
            {
                IEnumerator Attack()
                {
                    var isHit = Formulas.IsHit(enemy, player);
                    var isCriticalHit = false;
                    var damage = Formulas.CalculateDamage(enemy, player);
                    var attack = new AttackResult()
                    {
                        Opponent = player,
                        IsHit = isHit,
                        IsCriticalHit = isCriticalHit,
                        Damage = damage
                    };
                    yield return enemy.Attack(attack);
                }

                var direction = enemy.GetDirectionTo(player);
                var trigger = new Trigger(Attack());
                yield return enemy.action.Bump(direction, trigger);
            }

            enemy.actionBar.Reset();
            //enemy.render.SetParallaxSpeed(0.05f, 0.05f);
        }

        var dyingPlayers = actors.Where(x => x.isDying).ToList();
        foreach (var player in dyingPlayers)
        {
            yield return player.Die();
        }

        NextTurn();
    }

    #endregion



}
