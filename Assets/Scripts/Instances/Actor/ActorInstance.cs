using Assets.Scripts.Behaviors.Actor;
using Assets.Scripts.Events;
using Assets.Scripts.Instances.Actor;
using Assets.Scripts.Models;
using Game.Instances.Actor;
using Game.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// ActorInstance represents a game characterName (either hero or enemy) and encapsulates
// its state, behaviors, rendering, movement, and interactions with game systems.
public class ActorInstance : MonoBehaviour
{
    // Quick Reference Properties: Provide convenient access to core game systems via the GameManager singleton.
    protected List<ActorInstance> actors => GameManager.instance.actors;
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected BoardInstance board => GameManager.instance.board;
    protected CoinManager coinManager => GameManager.instance.coinManager;
    protected DamageTextManager damageTextManager => GameManager.instance.damageTextManager;
    protected DebugManager debugManager => GameManager.instance.debugManager;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
    protected bool hasFocusedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;
    protected float moveFocus => GameManager.instance.moveFocus;
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;
    protected PortraitManager portraitManager => GameManager.instance.portraitManager;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedHero;
    protected float snapThreshold => GameManager.instance.actorManager.snapTheshold;
    protected StageManager stageManager => GameManager.instance.stageManager;
    protected TileMap tileMap => GameManager.instance.tileMap;
    protected Vector3 tileScale => GameManager.instance.tileScale;
    protected float tileSize => GameManager.instance.tileSize;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    protected TileManager tileManager => GameManager.instance.tileManager;

    // Internal Properties: Provide information about the actor's state and position.
    public TileInstance currentTile => tileMap.GetTile(location); // Retrieves the tile corresponding to the actor's grid location.
    public bool isPlayer => team.Equals(Team.Hero);              // Determines if this actor belongs to the hero's team.
    public bool isEnemy => team.Equals(Team.Enemy);                // Determines if this actor is an enemy.
    public bool isActive => isActiveAndEnabled;                   // Checks if the GameObject is active.
    public bool isAlive => stats.HP > 0;                          // Actor is alive if HP is above zero.
    public bool isPlaying => isActive && isAlive;                 // Actor is active in the game (alive and enabled).
    public bool isDying => isActive && stats.HP < 1;              // Actor is in the process of dying (active but HP below 1).
    public bool isDead => !isActive && !isAlive;                  // Actor is dead when not active and HP is 0.
    public bool isSpawnable => !flags.HasSpawned && spawnTurn <= turnManager.currentTurn; // Actor can spawn if not already spawned and the spawn turn has arrived.
    public bool hasMaxAP => stats.AP == stats.MaxAP;              // Actor has maximum action points.

    public string characterName;                                // characterName actors for this actor.

    // Determines if the actor is invincible based on team-specific debug settings.
    public bool isInvincible => (isEnemy && debugManager.isEnemyInvincible) || (isPlayer && debugManager.isHeroInvincible);

    // Transform-related properties for position, rotation, scale and parent management.
    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true); // Preserves world position when changing parent.
    }
    public Vector3 position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }


    // Accessor for the position of the "Thumbnail" child object.
    public Vector3 thumbnailPosition
    {
        get => gameObject.transform.GetChild("Thumbnail").gameObject.transform.position;
        set => gameObject.transform.GetChild("Thumbnail").gameObject.transform.position = value;
    }

    public Quaternion rotation
    {
        get => gameObject.transform.rotation;
        set => gameObject.transform.rotation = value;
    }
    public Vector3 scale
    {
        get => gameObject.transform.localScale;
        set => gameObject.transform.localScale = value;
    }

    public SortingGroup sortingGroup
    {
        get => this.GetComponent<SortingGroup>();
    }





    public void SetSorting(string sortingLayer, int sortingOrder = 0)
    {
        sortingGroup.sortingLayerID = SortingLayer.NameToID(sortingLayer);
        sortingGroup.sortingOrder = sortingOrder;
    }

    // Fields: Core actors fields representing characterName stats, state, and modules.
    [SerializeField] public AnimationCurve glowCurve;   // Curve defining glow animation behavior.
    public Vector2Int previousLocation;                 // Grid location before the last movement.
    public Vector3 previousPosition;                    // World position before the last movement.
    public Vector2Int location;                         // CurrentProfile grid location.
    public Team team = Team.Neutral;                    // Actor's team affiliation.
    public int spawnTurn = 0;                           // Turn number when the actor is eligible to spawn.

    // Modules: Encapsulate various aspects of the actor such as rendering, stats, abilities, and animations.
    public ActorRenderers render = new ActorRenderers();
    public ActorStats stats = new ActorStats();
    public ActorFlags flags = new ActorFlags();
    public ActorAbilities abilities = new ActorAbilities();
    public ActorVFX vfx = new ActorVFX();
    public ActorWeapon weapon = new ActorWeapon();
    public ActorActions action = new ActorActions();
    public ActorMovement movement = new ActorMovement();
    public ActorHealthBar healthBar = new ActorHealthBar();
    public ActorActionBar actionBar = new ActorActionBar();
    public ActorGlow glow = new ActorGlow();
    public ActorParallax parallax = new ActorParallax();
    public ActorThumbnail thumbnail;

    // Methods for checking spatial relationships between this actor and others:

    public bool IsSameColumn(Vector2Int other) => location.x == other.x;
    public bool IsSameRow(Vector2Int other) => location.y == other.y;
    public bool IsAdjacentTo(Vector2Int other) => (IsSameColumn(other) || IsSameRow(other)) && Vector2Int.Distance(location, other).Equals(1);
    public bool IsNorthOf(Vector2Int other) => IsSameColumn(other) && location.y == other.y - 1;
    public bool IsEastOf(Vector2Int other) => IsSameRow(other) && location.x == other.x + 1;
    public bool IsSouthOf(Vector2Int other) => IsSameColumn(other) && location.y == other.y + 1;
    public bool IsWestOf(Vector2Int other) => IsSameRow(other) && location.x == other.x - 1;
    public bool IsNorthWestOf(Vector2Int other) => location.x == other.x - 1 && location.y == other.y - 1;
    public bool IsNorthEastOf(Vector2Int other) => location.x == other.x + 1 && location.y == other.y - 1;
    public bool IsSouthWestOf(Vector2Int other) => location.x == other.x - 1 && location.y == other.y + 1;
    public bool IsSouthEastOf(Vector2Int other) => location.x == other.x + 1 && location.y == other.y + 1;

    // Determines the cardinal/diagonal direction from this actor to another.
    // If 'mustBeAdjacent' is true, returns Direction.None if the other actor is not adjacent.
    public Direction GetDirectionTo(ActorInstance other, bool mustBeAdjacent = false)
    {
        if (mustBeAdjacent && !IsAdjacentTo(other.location))
            return Direction.None;

        var deltaX = location.x - other.location.x;
        var deltaY = location.y - other.location.y;

        // Handle simple cardinal directions.
        if (deltaX == 0 && deltaY > 0) return Direction.North;
        if (deltaX == 0 && deltaY < 0) return Direction.South;
        if (deltaX > 0 && deltaY == 0) return Direction.West;
        if (deltaX < 0 && deltaY == 0) return Direction.East;

        // Handle diagonal directions.
        if (deltaX > 0 && deltaY > 0) return Direction.NorthWest;
        if (deltaX < 0 && deltaY > 0) return Direction.NorthEast;
        if (deltaX > 0 && deltaY < 0) return Direction.SouthWest;
        if (deltaX < 0 && deltaY < 0) return Direction.SouthEast;

        // Default: no valid direction.
        return Direction.None;
    }

    /// <summary>
    /// Checks if there is any active actor within a given range in the specified cardinal direction.
    /// </summary>
    public bool HasAdjacent(Direction direction, int range)
    {
        for (int i = 1; i <= range; i++)
        {
            Vector2Int checkPos = location;
            switch (direction)
            {
                case Direction.North: checkPos += new Vector2Int(0, -i); break;
                case Direction.South: checkPos += new Vector2Int(0, i); break;
                case Direction.East: checkPos += new Vector2Int(i, 0); break;
                case Direction.West: checkPos += new Vector2Int(-i, 0); break;
            }
            if (actors.Any(actor => actor.isPlaying && actor.location == checkPos))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if there is any active actor within a given range in the specified diagonal direction.
    /// </summary>
    public bool HasDiagonal(Direction direction, int range)
    {
        for (int i = 1; i <= range; i++)
        {
            Vector2Int checkPos = location;
            switch (direction)
            {
                case Direction.NorthEast: checkPos += new Vector2Int(i, -i); break; 
                case Direction.NorthWest: checkPos += new Vector2Int(-i, -i); break;
                case Direction.SouthEast: checkPos += new Vector2Int(i, i); break;
                case Direction.SouthWest: checkPos += new Vector2Int(-i, i); break;
            }
            if (actors.Any(actor => actor.isPlaying && actor.location == checkPos))
                return true;
        }
        return false;
    }



    // Awake: Initialization of the actor actors. Sets up modules and subscribes to events.
    private void Awake()
    {
        // Assign modules with this actor actors context.
        render.Initialize(this);
        action.Initialize(this);
        movement.Initialize(this);
        healthBar.Initialize(this);
        actionBar.Initialize(this);
        glow.Initialize(this);
        parallax.Initialize(this);
        thumbnail = this.transform.Find(ComponentHelper.Actor.Front.Thumbnail).GetComponent<ActorThumbnail>();
 
    }

    // OnDestroy: Clean up event subscriptions if necessary to prevent memory leaks.
    private void OnDestroy()
    {
  
    }

    // Assign: Initializes and spawns the actor at the specified start location.
    public void Spawn(Vector2Int startLocation)
    {
        // Assign CurrentProfile and previous locations.
        location = startLocation;
        previousLocation = location;

        // Save world position based on grid location.
        position = Geometry.GetPositionByLocation(location);
        previousPosition = position;

        // Generate the thumbnail for UI/display purposes.
        thumbnail.Initialize(this);

        // Randomly assign weapon type and attributes.
        // TODO: Equip actor at stage manager load based on save file: party.json
        weapon.Type = Random.WeaponType();
        weapon.Attack = Random.Float(10, 15);
        weapon.Defense = Random.Float(0, 5);
        weapon.Name = $"{weapon.Type}";
        // Assign the weapon icon using resources.
        render.weaponIcon.sprite = SpriteRepo.WeaponTypes[weapon.Type.ToString()];

        // Configure visual appearance and effects based on team.
        if (isPlayer)
        {
            render.SetOpaqueColor(ColorHelper.Solid.White);
            render.SetQualityColor(ColorHelper.Solid.White);
            render.SetGlowColor(ColorHelper.Solid.White);
            render.SetParallaxSprite(SpriteRepo.Seamless["WhiteFire2"]);
            render.SetParallaxMaterial(MaterialRepo.Materials["PlayerParallax"], thumbnail.texture);
            render.SetParallaxAlpha(Opacity.Percent50);
            vfx.Attack = VisualEffectRepo.VisualEffects["BlueSlash1"];
        }
        else if (isEnemy)
        {
            render.SetOpaqueColor(ColorHelper.Solid.Red);
            render.SetQualityColor(ColorHelper.Solid.Red);
            render.SetGlowColor(ColorHelper.Solid.Red);
            render.SetParallaxSprite(SpriteRepo.Seamless["RedFire1"]);
            render.SetParallaxMaterial(MaterialRepo.Materials["EnemyParallax"], thumbnail.texture);
            render.SetParallaxAlpha(Opacity.Percent50);
            render.SetFrameColor(ColorHelper.Solid.Red);
            vfx.Attack = VisualEffectRepo.VisualEffects["DoubleClaw"];
        }

        // Assign name tag textarea and toggle its visibility based on debug settings.
        render.SetNameTagText(characterName);
        render.SetNameTagEnabled(isEnabled: debugManager.showActorNameTag);

        // Save health and action bars.
        healthBar.Update();
        actionBar.Reset();

        // Activate the actor if it is spawnable; otherwise, keep it inactive.
        if (isSpawnable)
        {
            gameObject.SetActive(true);
            flags.HasSpawned = true;
            // TriggerEvent fade-in and spin animations for visual feedback.
            action.TriggerFadeIn();
            action.TriggerSpin360();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

     // CalculateAttackStrategy: Chooses an attackResult strategy based on weighted randomness and sets the target location.
    public void CalculateAttackStrategy()
    {
        // Define weights for different strategies.
        int[] ratios = { 50, 20, 15, 10, 5 };
        var attackStrategy = Random.Strategy(ratios);

        Vector2Int targetLocation = LocationHelper.Nowhere;

        // SelectProfile target based on strategy.
        switch (attackStrategy)
        {
            case AttackStrategy.AttackClosest:
                // Choose the closest hero.
                var targetPlayer = heroes.Where(x => x.isPlaying).OrderBy(x => Vector3.Distance(x.position, position)).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;
            case AttackStrategy.AttackWeakest:
                // Choose the hero with the lowest HP.
                targetPlayer = heroes.Where(x => x.isPlaying).OrderBy(x => x.stats.HP).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;
            case AttackStrategy.AttackStrongest:
                // Choose the hero with the highest HP.
                targetPlayer = heroes.Where(x => x.isPlaying).OrderByDescending(x => x.stats.HP).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;
            case AttackStrategy.AttackRandom:
                // Choose a random hero's location.
                targetLocation = Random.Hero.location;
                break;
            case AttackStrategy.MoveAnywhere:
                // Choose a random location.
                targetLocation = Random.Location;
                break;
        }

        //Assign the actor's location to the nearest valid attackResult location relative to the target.
        location = Geometry.GetClosestAttackLocation(location, targetLocation);
        //Note: nextPosition is commented out and could be used for future logic.
        //nextPosition = Geometry.GetPositionByLocation(nextLocation.Value);
    }

    //TriggerTakeDamage: Begins the process for this actor to take damage from an attackResult.
    public void TriggerTakeDamage(AttackResult attack)
    {
        // If the actor is not active or alive, abort.
        if (!isActive || !isAlive)
            return;

        StartCoroutine(TakeDamage(attack));
    }

    //FireDamage: Coroutine to display fire damage textarea and wait until the next frame.
    public IEnumerator FireDamage(float amount)
    {
        damageTextManager.Spawn($"Fireball: - {amount} HP", position);
        yield return Wait.UntilNextFrame();
    }

    //Heal: Coroutine to display healing textarea and wait until the next frame.
    public IEnumerator Heal(float amount)
    {
        damageTextManager.Spawn($"Heal: +{amount} HP", position);
        yield return Wait.UntilNextFrame();
    }

    //TakeDamage: Coroutine that processes damage application, triggers VFX and animations, and updates HP.
    public IEnumerator TakeDamage(AttackResult attack)
    {
        if (!isPlaying)
            yield break;

        // Immediately apply damage and update health.
        if (!isInvincible)
        {
            stats.PreviousHP = stats.HP;
            stats.HP -= attack.Damage;
            stats.HP = Mathf.Clamp(stats.HP, 0, stats.MaxHP);
            healthBar.Update();
        }

        // Immediately display damage textarea and play sound.
        damageTextManager.Spawn(attack.Damage.ToString(), position);
        audioManager.Play($"Slash{Random.Int(1, 7)}");

        if (isDying)
            TriggerDie();

        // Start the damage animation as a separate coroutine so it doesn't block.
        //Execute(DamageTaken(attackResult));

        // Return immediately.
        yield break;
    }


    //private IEnumerator DamageTaken(AttackResult attackResult)
    //{
    //    float ticks = 0f;
    //    float duration = Interval.TenTicks; // For example, 1 second.

    //    while (ticks < duration)
    //    {
    //        action.TriggerGrow(); // Flinch effect.
    //        if (attackResult.IsCriticalHit)
    //            action.TriggerShake(ShakeIntensity.Medium);
    //        ticks += Interval.OneTick;
    //        yield return Wait.For(Interval.OneTick);
    //    }

    //    // Reset animations.
    //    action.TriggerShrink();
    //    action.TriggerShake(ShakeIntensity.Stop);

    //    if (isDying)
    //        TriggerDie();

    //    yield break;
    //}


    //AttackMiss: Coroutine to display a miss message and attackResult a dodge animation.
    public IEnumerator AttackMiss()
    {
        damageTextManager.Spawn("Miss", position);
        yield return action.Dodge();
    }

    //TriggerDie: Initiates the actor's death sequence.
    public void TriggerDie()
    {
        StartCoroutine(Die());
    }

    //Die: Coroutine that handles the actor's death sequence, including fading out, spawning coins, and deactivation.
    public IEnumerator Die()
    {
        //Abort if the actor is not in a dying state.
        if (!isDying)
            yield break;

        //Before: Assign actor to fully opaque.
        var alpha = 1f;
        render.SetAlpha(alpha);

        //Wait until the health bar has finished draining.
        if (healthBar.isDraining)
            yield return new WaitUntil(() => healthBar.isEmpty);

        //TriggerEvent portrait dissolve effect and play death sound.
        portraitManager.Dissolve(this);
        audioManager.Play("Death");

        //Assign sorting order to maximum so that the death sequence renders on top.
        //sortingOrder = SortingOrder.Max;

        //During: Gradually reduce the alpha value for a fade-out effect.
        var hasSpawnedCoins = false;
        while (alpha > 0f)
        {
            alpha -= Increment.OnePercent;
            alpha = Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Opaque);
            render.SetAlpha(alpha);

            //Assign coins when enemy fades below 10% opacity, if not already spawned.
            if (isEnemy && !hasSpawnedCoins && alpha < Opacity.Percent10)
            {
                hasSpawnedCoins = true;
                int amount = 10;
                TriggerSpawnCoins(amount);
            }

            yield return Wait.OneTick();
        }

        //After: Reset location and position, deactivate the actor, and check death event.
        location = LocationHelper.Nowhere;
        position = PositionHelper.Nowhere;
        gameObject.SetActive(false);
        stageManager.OnActorDeath();
    }

    //TriggerSpawnCoins: Helper function to begin spawning coins upon enemy death.
    private void TriggerSpawnCoins(int amount)
    {
        if (isPlaying)
            StartCoroutine(SpawnCoins(amount)); // TODO: Adjust coin spawning based on enemy stats if necessary.
    }

    //SpawnCoins: Coroutine that spawns a specified number of coins at the actor's position.
    IEnumerator SpawnCoins(int amount)
    {
        var i = 0;
        do
        {
            coinManager.Spawn(position);
            i++;
        } while (i < amount);

        yield return true;
    }

    //Teleport: Moves the actor instantly to a new grid location if within board bounds.
    public void Teleport(Vector2Int newLocation)
    {
        //Abort if the new location is out of bounds.
        if (!board.InBounds(newLocation))
            return;

        this.location = newLocation;
        transform.position = Geometry.GetPositionByLocation(this.location);
    }

    //Move: Attempts to move the actor in the specified direction if the target location is valid.
    public void Move(Vector2Int direction)
    {
        //Abort if the new location (CurrentProfile location + direction) is out of bounds.
        if (!board.InBounds(location + direction))
            return;

        var newLocation = location + direction;
        var tile = GameManager.instance.tileMap.GetTile(newLocation);
        if (tile == null) return;
        // Teleport to the new tile's location.
        Teleport(tile.location);
    }

    //SetReady: Resets the enemy actor's action points for a new turn.
    public void SetReady()
    {
        //Abort if the actor is not active, not alive, or not an enemy.
        if (!isActive || !isAlive || !isEnemy)
            return;

        stats.AP = stats.MaxAP;
        stats.PreviousAP = stats.MaxAP;

        //Save the action bar UI to reflect the refreshed action points.
        actionBar.Update();
    }
}
