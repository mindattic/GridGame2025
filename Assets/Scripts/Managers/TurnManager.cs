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
    public bool isStartPhase => currentPhase.Equals(TurnPhase.Start);
    public bool isMovePhase => currentPhase.Equals(TurnPhase.Move);
    public bool isPreAttackPhase => currentPhase.Equals(TurnPhase.PreAttack);
    public bool isAttackPhase => currentPhase.Equals(TurnPhase.Attack);
    public bool isPostAttackPhase => currentPhase.Equals(TurnPhase.PostAttack);
    public bool isEndPhase => currentPhase.Equals(TurnPhase.End);
    public bool isFirstTurn => currentTurn == 0;

    //System.Action event handlers
    public event System.Action<TurnPhase> onTurnPhaseChanged;

    //Fields
    public int currentTurn = 0;
    public Team currentTeam = Team.Player;
    public TurnPhase currentPhase = TurnPhase.Start;

    public void SetPhase(TurnPhase turnPhase)
    {
        currentPhase = turnPhase;
        onTurnPhaseChanged?.Invoke(currentPhase);
    }


  
    void Awake()
    {
        onTurnPhaseChanged += (TurnPhase turnPhase) => OnTurnPhaseChanged(turnPhase);
    }


    private void OnTurnPhaseChanged(TurnPhase turnPhase)
    {
        currentPhase = turnPhase;

        if (isPlayerTurn)
        {
            switch (currentPhase)
            {
                case TurnPhase.Start:
                    currentTurn++;
                    timerBar.Refill();
                    playerManager.TriggerGlow();
                    break;
            }
        }
        else if (isEnemyTurn)
        {
            switch (currentPhase)
            {
                case TurnPhase.Start:
                    timerBar.Lock();
                    actionManager.Add(new EnemySpawnAction());

                    bool anyReadyEnemies = enemies.Any(x => x.isPlaying && x.hasMaxAP);
                    if (!anyReadyEnemies)
                    {
                        actionManager.TriggerExecute();
                        // No enemy is ready; advance the turn immediately.
                        NextTurn();
                        break;
                    }

                    actionManager.Add(new EnemyStartAction());
                    actionManager.TriggerExecute();
                    break;
            }
        }
    }



    public void Initialize()
    {
        currentTurn = 0;
        currentTeam = Team.Player;
        playerManager.TriggerGlow();
        SetPhase(TurnPhase.Start);
    }

    public void NextTurn()
    {
        // Switch team for the next turn.
        currentTeam = isPlayerTurn ? Team.Enemy : Team.Player;
        supportLineManager.Clear();
        attackLineManager.Clear();
        SetPhase(TurnPhase.Start);
    }



}
