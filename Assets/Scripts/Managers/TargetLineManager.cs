using System;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Events;
using game = GameManagerHelper;
public class TargetLineManager : MonoBehaviour
{
    protected InputManager inputManager => GameManager.instance.inputManager;
    protected BoardInstance board => GameManager.instance.board;
    protected float tileSize => GameManager.instance.tileSize;

    private Camera mainCamera;
    private TargetLineInstance targetLinePrefab;
    private float lockRadius;

    private ActorInstance hoveredTarget;
    private Vector3 buttonOrigin;
    private Action<ActorInstance> onTargetConfirmed;
    private TargetLineInstance activeLine;
    private ActorInstance lastClicked;

    private void Awake()
    {
        mainCamera = Camera.main;
        lockRadius = tileSize / 2f;

        if (!PrefabRepo.Prefabs.TryGetValue("TargetLinePrefab", out var prefabGO))
            Debug.LogError("TargetLinePrefab not found in PrefabRepo.");
        else if ((targetLinePrefab = prefabGO.GetComponent<TargetLineInstance>()) == null)
            Debug.LogError("TargetLinePrefab is missing TargetLineInstance.");
    }

    /// <summary>
    /// Called when an ability button is clicked.
    /// Switches into AbilityTarget mode, spawns the line, and snaps to a random actor.
    /// </summary>
    public void BeginTargeting(Vector3 fromWorldPosition, Action<ActorInstance> onConfirmed)
    {
        // 1) switch global input mode
        inputManager.inputMode = InputMode.AbilityTarget;

        // 2) store callback & origin
        buttonOrigin = fromWorldPosition;
        onTargetConfirmed = onConfirmed;
        lastClicked = null;

        // 3) instantiate line
        var go = Instantiate(targetLinePrefab.gameObject, Vector3.zero, Quaternion.identity);
        activeLine = go.GetComponent<TargetLineInstance>();
        activeLine.name = $"TargetLine_{Guid.NewGuid():N}";
        activeLine.parent = board.transform;
        activeLine.buttonPosition = buttonOrigin;
        activeLine.cursorPosition = buttonOrigin;

        // 4) snap to a random actor initially
        var heroes = GameManager.instance.heroes.ToList();
        if (heroes.Count > 0)
        {
            var randomHero = heroes[UnityEngine.Random.Range(0, heroes.Count)];
            SnapToTarget(randomHero);
        }
    }

    /// <summary>
    /// Call when the player taps a actor while in AbilityTarget mode.
    /// </summary>
    public void OnTargetTouch(ActorInstance hero)
    {
        if (inputManager.inputMode != InputMode.AbilityTarget)
            return;

        if (hero == lastClicked)
        {
            // double-click: confirm
            onTargetConfirmed?.Invoke(hero);
            EndTargeting();
        }
        else
        {
            // first click: just snap
            SnapToTarget(hero);
            lastClicked = hero;
        }
    }

    private void SnapToTarget(ActorInstance actor)
    {
        hoveredTarget = actor;
        activeLine.cursorPosition = actor.position;
        activeLine.UpdateArcPoints(buttonOrigin, actor.position);
    }

    private void EndTargeting()
    {
        // cleanup line
        if (activeLine != null)
        {
            activeLine.TriggerDespawn();
            Destroy(activeLine.gameObject);
            activeLine = null;
        }
        onTargetConfirmed = null;
        lastClicked = null;

        // restore normal input
        inputManager.inputMode = InputMode.HeroTurn;
    }
}
