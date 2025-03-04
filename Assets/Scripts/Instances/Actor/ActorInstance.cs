using Assets.Scripts.Behaviors.Actor;
using Assets.Scripts.Instances.Actor;
using Assets.Scripts.Models;
using Game.Instances.Actor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ActorInstance represents a game character (either player or enemy) and encapsulates
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
    protected float moveSpeed => GameManager.instance.moveSpeed;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;
    protected PortraitManager portraitManager => GameManager.instance.portraitManager;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    protected float snapDistance => GameManager.instance.snapThreshold;
    protected StageManager stageManager => GameManager.instance.stageManager;
    protected TileMap tileMap => GameManager.instance.tileMap;
    protected Vector3 tileScale => GameManager.instance.tileScale;
    protected float tileSize => GameManager.instance.tileSize;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    protected TileManager tileManager => GameManager.instance.tileManager;

    // Internal Properties: Provide information about the actor's state and position.
    public TileInstance currentTile => tileMap.GetTile(location); // Retrieves the tile corresponding to the actor's grid location.
    public bool isPlayer => team.Equals(Team.Player);              // Determines if this actor belongs to the player's team.
    public bool isEnemy => team.Equals(Team.Enemy);                // Determines if this actor is an enemy.
    public bool isActive => isActiveAndEnabled;                   // Checks if the GameObject is active.
    public bool isAlive => stats.HP > 0;                          // Actor is alive if HP is above zero.
    public bool isPlaying => isActive && isAlive;                 // Actor is active in the game (alive and enabled).
    public bool isDying => isActive && stats.HP < 1;              // Actor is in the process of dying (active but HP below 1).
    public bool isDead => !isActive && !isAlive;                  // Actor is dead when not active and HP is 0.
    public bool isSpawnable => !flags.HasSpawned && spawnTurn <= turnManager.currentTurn; // Actor can spawn if not already spawned and the spawn turn has arrived.
    public bool hasMaxAP => stats.AP == stats.MaxAP;              // Actor has maximum action points.

    // Determines if the actor is invincible based on team-specific debug settings.
    public bool isInvincible => (isEnemy && debugManager.isEnemyInvincible) || (isPlayer && debugManager.isPlayerInvincible);

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

    // Sorting order property adjusts the rendering layers for various parts of the actor.
    // Changing the sorting order updates multiple renderer components and invokes an event.
    public int sortingOrder
    {
        get
        {
            return render.opaque.sortingOrder;
        }
        set
        {
            render.opaque.sortingOrder = value + ActorLayer.Value.Opaque;
            render.quality.sortingOrder = value + ActorLayer.Value.Quality;
            render.parallax.sortingOrder = value + ActorLayer.Value.Parallax;
            render.glow.sortingOrder = value + ActorLayer.Value.Glow;
            render.thumbnail.sortingOrder = value + ActorLayer.Value.Thumbnail;
            render.frame.sortingOrder = value + ActorLayer.Value.Frame;
            render.statusIcon.sortingOrder = value + ActorLayer.Value.StatusIcon;
            render.healthBarBack.sortingOrder = value + ActorLayer.Value.HealthBar.Back;
            render.healthBarDrain.sortingOrder = value + ActorLayer.Value.HealthBar.Drain;
            render.healthBarFill.sortingOrder = value + ActorLayer.Value.HealthBar.Fill;
            render.healthBarText.sortingOrder = value + ActorLayer.Value.HealthBar.Text;
            render.actionBarBack.sortingOrder = value + ActorLayer.Value.ActionBar.Back;
            render.actionBarFill.sortingOrder = value + ActorLayer.Value.ActionBar.Fill;
            render.actionBarText.sortingOrder = value + ActorLayer.Value.ActionBar.Text;
            render.mask.sortingOrder = value + ActorLayer.Value.Mask;
            render.radialBack.sortingOrder = value + ActorLayer.Value.RadialBack;
            render.radial.sortingOrder = value + ActorLayer.Value.RadialFill;
            render.radialText.sortingOrder = value + ActorLayer.Value.RadialText;
            render.turnDelayText.sortingOrder = value + ActorLayer.Value.TurnDelayText;
            render.nameTagText.sortingOrder = value + ActorLayer.Value.NameTagText;
            render.weaponIcon.sortingOrder = value + ActorLayer.Value.WeaponIcon;
            render.armorNorth.sortingOrder = value + ActorLayer.Value.Armor.ArmorNorth;
            render.armorEast.sortingOrder = value + ActorLayer.Value.Armor.ArmorEast;
            render.armorSouth.sortingOrder = value + ActorLayer.Value.Armor.ArmorSouth;
            render.armorWest.sortingOrder = value + ActorLayer.Value.Armor.ArmorWest;
            onSortingOrderChanged?.Invoke(); // Notify listeners that sorting order has been updated.
        }
    }

    // Event handlers for various actor-related events.
    public System.Action<Vector2Int> onOverlapDetected;                                 // Invoked when the actor overlaps with a new grid location.
    public System.Action<Vector2Int, Vector2Int> onSelectedPlayerLocationChanged;       // Invoked when the actor changes location.
    public System.Action onActorDeath;                                                  // Invoked upon actor death.
    public System.Action onSortingOrderChanged;                                         // Invoked when the sorting order is modified.
    public System.Action onDragDetected;                                                // Invoked when a drag operation is detected on the actor.

    // Fields: Core data fields representing character stats, state, and modules.
    [SerializeField] public AnimationCurve glowCurve;   // Curve defining glow animation behavior.
    public Character character;                         // Character data for this actor.
    public Vector2Int previousLocation;                 // Grid location before the last movement.
    public Vector3 previousPosition;                    // World position before the last movement.
    public Vector2Int location;                         // Current grid location.
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
    public ActorThumbnail thumbnail = new ActorThumbnail();

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

    // Awake: Initialization of the actor instance. Sets up modules and subscribes to events.
    private void Awake()
    {
        // Initialize modules with this actor instance context.
        render.Initialize(this);
        action.Initialize(this);
        movement.Initialize(this);
        healthBar.Initialize(this);
        actionBar.Initialize(this);
        glow.Initialize(this);
        parallax.Initialize(this);
        thumbnail.Initialize(this);

        // Subscribe to event handlers to link movement and stage-related updates.
        onOverlapDetected += (location) => movement.OnOverlapDetected(location);
        onSelectedPlayerLocationChanged += (previousLocation, newLocation) => tileManager.OnSelectedPlayerLocationChanged(previousLocation, newLocation);
        onDragDetected += movement.OnDragDetected;
        onActorDeath += stageManager.OnActorDeath;
    }

    // OnDestroy: Clean up event subscriptions if necessary to prevent memory leaks.
    private void OnDestroy()
    {
        // if (movement != null)
        //     onOverlapDetected -= movement.OnOverlapDetected;
        // if (stageManager != null)
        //     onActorDeath -= stageManager.OnActorDeath;
    }

    // Spawn: Initializes and spawns the actor at the specified start location.
    public void Spawn(Vector2Int startLocation)
    {
        // Set current and previous locations.
        location = startLocation;
        previousLocation = location;
        // Update world position based on grid location.
        position = Geometry.GetPositionByLocation(location);
        previousPosition = position;

        // Generate the thumbnail for UI/display purposes.
        thumbnail.Generate();

        // Randomly assign weapon type and attributes.
        // TODO: Equip actor at stage manager load based on save file: party.json
        weapon.Type = Random.WeaponType();
        weapon.Attack = Random.Float(10, 15);
        weapon.Defense = Random.Float(0, 5);
        weapon.Name = $"{weapon.Type}";
        // Set the weapon icon using resources.
        render.weaponIcon.sprite = resourceManager.WeaponType(weapon.Type.ToString()).Value;

        // Configure visual appearance and effects based on team.
        if (isPlayer)
        {
            render.SetOpaqueColor(ColorHelper.Solid.White);
            render.SetQualityColor(ColorHelper.Solid.White);
            render.SetGlowColor(ColorHelper.Solid.White);
            render.SetParallaxSprite(resourceManager.Seamless("WhiteFire2").Value);
            render.SetParallaxMaterial(resourceManager.Material("PlayerParallax", thumbnail.texture).Value);
            render.SetParallaxAlpha(Opacity.Percent50);
            vfx.Attack = resourceManager.VisualEffect("BlueSlash1");
        }
        else if (isEnemy)
        {
            render.SetOpaqueColor(ColorHelper.Solid.Red);
            render.SetQualityColor(ColorHelper.Solid.Red);
            render.SetGlowColor(ColorHelper.Solid.Red);
            render.SetParallaxSprite(resourceManager.Seamless("RedFire1").Value);
            render.SetParallaxMaterial(resourceManager.Material("EnemyParallax", thumbnail.texture).Value);
            render.SetParallaxAlpha(Opacity.Percent50);
            render.SetFrameColor(ColorHelper.Solid.Red);
            vfx.Attack = resourceManager.VisualEffect("DoubleClaw");
        }

        // Set name tag text and toggle its visibility based on debug settings.
        render.SetNameTagText(name);
        render.SetNameTagEnabled(isEnabled: debugManager.showActorNameTag);

        // Update health and action bars.
        healthBar.Update();
        actionBar.Reset();

        // Activate the actor if it is spawnable; otherwise, keep it inactive.
        if (isSpawnable)
        {
            gameObject.SetActive(true);
            flags.HasSpawned = true;
            // Trigger fade-in and spin animations for visual feedback.
            action.TriggerFadeIn();
            action.TriggerSpin360();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Attack: Executes an attack on an opponent, displaying VFX and applying damage.
    public IEnumerator Attack(AttackResult attack)
    {
        // Spawn the attack visual effect at the opponent's position.
        // If the attack is a hit, trigger the opponent's damage routine; otherwise, perform a miss animation.
        yield return vfxManager.Spawn(
            vfx.Attack,
            attack.Opponent.position,
            new Trigger(coroutine: attack.IsHit ? attack.Opponent.TakeDamage(attack) : attack.Opponent.AttackMiss(),
                        isAsync: false));
    }

    // CalculateAttackStrategy: Chooses an attack strategy based on weighted randomness and sets the target location.
    public void CalculateAttackStrategy()
    {
        // Define weights for different strategies.
        int[] ratios = { 50, 20, 15, 10, 5 };
        var attackStrategy = Random.Strategy(ratios);

        Vector2Int targetLocation = Location.Nowhere;

        // Select target based on strategy.
        switch (attackStrategy)
        {
            case AttackStrategy.AttackClosest:
                // Choose the closest player.
                var targetPlayer = players.Where(x => x.isPlaying).OrderBy(x => Vector3.Distance(x.position, position)).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;
            case AttackStrategy.AttackWeakest:
                // Choose the player with the lowest HP.
                targetPlayer = players.Where(x => x.isPlaying).OrderBy(x => x.stats.HP).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;
            case AttackStrategy.AttackStrongest:
                // Choose the player with the highest HP.
                targetPlayer = players.Where(x => x.isPlaying).OrderByDescending(x => x.stats.HP).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;
            case AttackStrategy.AttackRandom:
                // Choose a random player's location.
                targetLocation = Random.Player.location;
                break;
            case AttackStrategy.MoveAnywhere:
                // Choose a random location.
                targetLocation = Random.Location;
                break;
        }

        // Set the actor's location to the nearest valid attack location relative to the target.
        location = Geometry.GetClosestAttackLocation(location, targetLocation);
        // Note: nextPosition is commented out and could be used for future logic.
        // nextPosition = Geometry.GetPositionByLocation(nextLocation.Value);
    }

    // TriggerTakeDamage: Begins the process for this actor to take damage from an attack.
    public void TriggerTakeDamage(AttackResult attack)
    {
        // If the actor is not active or alive, abort.
        if (!isActive || !isAlive)
            return;

        StartCoroutine(TakeDamage(attack));
    }

    // FireDamage: Coroutine to display fire damage text and wait until the next frame.
    public IEnumerator FireDamage(float amount)
    {
        damageTextManager.Spawn($"Fireball: - {amount} HP", position);
        yield return Wait.UntilNextFrame();
    }

    // Heal: Coroutine to display healing text and wait until the next frame.
    public IEnumerator Heal(float amount)
    {
        damageTextManager.Spawn($"Heal: +{amount} HP", position);
        yield return Wait.UntilNextFrame();
    }

    // TakeDamage: Coroutine that processes damage application, triggers VFX and animations, and updates HP.
    public IEnumerator TakeDamage(AttackResult attack)
    {
        // Abort if the actor is not active or alive.
        if (!isActive || !isAlive)
            yield break;

        // If the attack is critical, trigger a special VFX at the opponent's position.
        if (attack.IsCriticalHit)
            vfxManager.TriggerSpawn(resourceManager.VisualEffect("YellowHit"), attack.Opponent.position);

        // Optionally trigger pre-damage effects here.
        // yield return attack.Triggers.Before.StartCoroutine(this);

        // Set up damage animation duration.
        float ticks = 0f;
        float duration = Interval.TenTicks;

        // If the actor is not invincible, reduce HP and update the health bar.
        if (!isInvincible)
        {
            stats.PreviousHP = stats.HP;
            stats.HP -= attack.Damage;
            stats.HP = Mathf.Clamp(stats.HP, 0, stats.MaxHP);
            healthBar.Update();
        }

        // Display damage text and play a random slash sound effect.
        damageTextManager.Spawn(attack.Damage.ToString(), position);
        audioManager.Play($"Slash{Random.Int(1, 7)}");

        // During: Animate the damage reaction over the set duration.
        while (ticks < duration)
        {
            action.TriggerGrow(); // Possibly makes the actor appear to flinch.
            if (attack.IsCriticalHit)
                action.TriggerShake(ShakeIntensity.Medium); // Shake effect for critical hits.

            ticks += Interval.OneTick;
            yield return Wait.For(Interval.OneTick);
        }

        // Optionally trigger post-damage effects here.
        // yield return attack.Triggers.After.StartCoroutine(this);

        // After: Reset animations to normal.
        action.TriggerShrink();
        action.TriggerShake(ShakeIntensity.Stop);
    }

    // AttackMiss: Coroutine to display a miss message and trigger a dodge animation.
    public IEnumerator AttackMiss()
    {
        damageTextManager.Spawn("Miss", position);
        yield return action.Dodge();
    }

    // TriggerDie: Initiates the actor's death sequence.
    public void TriggerDie()
    {
        StartCoroutine(Die());
    }

    // Die: Coroutine that handles the actor's death sequence, including fading out, spawning coins, and deactivation.
    public IEnumerator Die()
    {
        // Abort if the actor is not in a dying state.
        if (!isDying)
            yield break;

        // Before: Set actor to fully opaque.
        var alpha = 1f;
        render.SetAlpha(alpha);

        // Wait until the health bar has finished draining.
        if (healthBar.isDraining)
            yield return new WaitUntil(() => healthBar.isEmpty);

        // Trigger portrait dissolve effect and play death sound.
        portraitManager.Dissolve(this);
        audioManager.Play("Death");
        // Set sorting order to maximum so that the death sequence renders on top.
        sortingOrder = SortingOrder.Max;

        // During: Gradually reduce the alpha value for a fade-out effect.
        var hasSpawnedCoins = false;
        while (alpha > 0f)
        {
            alpha -= Increment.OnePercent;
            alpha = Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Opaque);
            render.SetAlpha(alpha);

            // Spawn coins when enemy fades below 10% opacity, if not already spawned.
            if (isEnemy && !hasSpawnedCoins && alpha < Opacity.Percent10)
            {
                hasSpawnedCoins = true;
                int amount = 10;
                TriggerSpawnCoins(amount);
            }

            yield return Wait.OneTick();
        }

        // After: Reset location and position, deactivate the actor, and invoke the death event.
        location = Location.Nowhere;
        position = Position.Nowhere;
        gameObject.SetActive(false);
        onActorDeath.Invoke();
    }

    // TriggerSpawnCoins: Helper function to begin spawning coins upon enemy death.
    private void TriggerSpawnCoins(int amount)
    {
        if (isPlaying)
            StartCoroutine(SpawnCoins(amount)); // TODO: Adjust coin spawning based on enemy stats if necessary.
    }

    // SpawnCoins: Coroutine that spawns a specified number of coins at the actor's position.
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

    // Teleport: Moves the actor instantly to a new grid location if within board bounds.
    public void Teleport(Vector2Int newLocation)
    {
        // Abort if the new location is out of bounds.
        if (!board.InBounds(newLocation))
            return;

        this.location = newLocation;
        // Update world position based on the new grid location.
        transform.position = Geometry.GetPositionByLocation(this.location);
    }

    // Move: Attempts to move the actor in the specified direction if the target location is valid.
    public void Move(Vector2Int direction)
    {
        // Abort if the new location (current location + direction) is out of bounds.
        if (!board.InBounds(location + direction))
            return;

        var newLocation = location + direction;
        Debug.Log(newLocation);
        var tile = GameManager.instance.tileMap.GetTile(newLocation);
        if (tile == null) return;
        // Teleport to the new tile's location.
        Teleport(tile.location);
    }

    // SetReady: Resets the enemy actor's action points for a new turn.
    public void SetReady()
    {
        // Abort if the actor is not active, not alive, or not an enemy.
        if (!isActive || !isAlive || !isEnemy)
            return;

        stats.AP = stats.MaxAP;
        stats.PreviousAP = stats.MaxAP;

        // Update the action bar UI to reflect the refreshed action points.
        actionBar.Update();
    }
}
