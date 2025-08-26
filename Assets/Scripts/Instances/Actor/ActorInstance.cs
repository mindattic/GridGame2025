using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Behaviors.Actor;
using Assets.Scripts.Instances.Actor;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameHelper;

// ActorInstance represents a game characterName (either hero or attacker) and encapsulates
// its state, behaviors, rendering, Move, and interactions with game systems.
public class ActorInstance : MonoBehaviour
{
    #region Instance Properies
    public TileInstance currentTile => g.TileMap.GetTile(location); // Retrieves the tile corresponding to the actor's grid location.
    public bool IsHero => team.Equals(Team.Hero);              // Determines if this actor belongs to the hero's team.
    public bool IsEnemy => team.Equals(Team.Enemy);                // Determines if this actor is an attacker.
    public bool IsActive => isActiveAndEnabled;                   // Checks if the GameObject is active.
    public bool IsAlive => Stats.HP > 0;                          // Actor is alive if HP is above zero.
    public bool IsPlaying => IsActive && IsAlive;                 // Actor is active in the game (alive and enabled).
    public bool IsDying => IsActive && Stats.HP < 1;              // Actor is in the process of dying (active but HP below 1).
    public bool IsDead => !IsActive && !IsAlive;                  // Actor is dead when not active and HP is 0.
    public bool IsSpawnable => !Flags.HasSpawned && spawnTurn <= g.TurnManager.CurrentTurn; // Actor can spawn if not already spawned and the spawn turn has arrived.
    public bool HasMaxAP => Stats.AP == Stats.MaxAP;              // Actor has maximum Animation points.

    public bool IsReady => IsPlaying && HasMaxAP;

    //public bool IsSameColumn(Vector2Int other) => location.s == other.s;
    //public bool IsSameRow(Vector2Int other) => location.y == other.y;
    //public bool IsAdjacentTo(Vector2Int other) => (IsSameColumn(other) || IsSameRow(other)) && Vector2Int.Distance(location, other).Equals(1);

    // Determines if the actor is invincible based on team-specific debug settings.
    public bool IsInvincible => (IsEnemy && g.DebugManager.isEnemyInvincible) || (IsHero && g.DebugManager.isHeroInvincible);

    // Transform-related properties for position, rotation, scale and parent management.
    public Transform Parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true); // Preserves world position when changing parent.
    }
    public Vector3 Position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }


    // Accessor for the position of the "Thumbnail" child object.
    public Vector3 ThumbnailPosition
    {
        get => gameObject.transform.GetChild("Thumbnail").gameObject.transform.position;
        set => gameObject.transform.GetChild("Thumbnail").gameObject.transform.position = value;
    }

    public Quaternion Rotation
    {
        get => gameObject.transform.rotation;
        set => gameObject.transform.rotation = value;
    }
    public Vector3 Scale
    {
        get => gameObject.transform.localScale;
        set => gameObject.transform.localScale = value;
    }

    public SortingGroup SortingGroup
    {
        get => this.GetComponent<SortingGroup>();
    }
    #endregion

    // --- Timeline Turn Order (TurnDelay) ------------------------------

    private int turnDelay = -1;

    /// <summary>
    /// Current turn delay for this actor. Heroes typically ignore this.
    /// </summary>
    public int TurnDelay => turnDelay;

    /// <summary>
    /// Set an initial delay for enemies if not already set. Heroes are ignored.
    /// </summary>
    public void SetInitialTurnDelay(int min, int max)
    {
        if (!IsEnemy)
            return;

        if (turnDelay < 0)
            turnDelay = RNG.Int(min, max);
    }

    /// <summary>
    /// Decrease delay by amount (default 1). Clamped to 0. Heroes ignored.
    /// </summary>
    public void DecrementTurnDelay(int amount = 1)
    {
        if (!IsEnemy)
            return;

        if (turnDelay < 0)
            return;

        turnDelay = Mathf.Max(0, turnDelay - Mathf.Max(1, amount));
    }

    /// <summary>
    /// Apply a new delay value after this enemy completes its turn. Heroes ignored.
    /// </summary>
    public void ApplyNewTurnDelay(int value)
    {
        if (!IsEnemy)
            return;

        turnDelay = Mathf.Max(0, value);
    }

    #region Sorting

    /// <summary>
    /// Sets sorting layer and order.
    /// </summary>
    /// <param name="sortingLayer">Layer name.</param>
    /// <param name="sortingOrder">Order number.</param>
    public void SetSorting(string sortingLayer, int sortingOrder = 0)
    {
        SortingGroup.sortingLayerID = SortingLayer.NameToID(sortingLayer);
        SortingGroup.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// Subscribe to global sort requests.
    /// </summary>
    private void OnEnable()
    {
        SortingManager.OnSortRequested += HandleSortEvent;
    }

    /// <summary>
    /// Unsubscribe to prevent memory leaks.
    /// </summary>
    private void OnDisable()
    {
        SortingManager.OnSortRequested -= HandleSortEvent;
    }

    /// <summary>
    /// Respond to sort requests by applying layer/order based on event type.
    /// </summary>
    /// <param name="e">Sort event context.</param>
    private void HandleSortEvent(SortEvent e)
    {
        switch (e.Type)
        {
            case SortEventType.Focus:
                // Focused actor on top, others below
                if (this == e.Initiator)
                    SetSorting(SortingHelper.Layer.ActorAbove, SortingHelper.Order.Max);
                else
                    SetSorting(SortingHelper.Layer.ActorBelow, SortingHelper.Order.Min);
                break;

            case SortEventType.Drag:
                // Dragged actor on top
                if (this == e.Initiator)
                    SetSorting(SortingHelper.Layer.ActorAbove, SortingHelper.Order.Max);
                else
                    SetSorting(SortingHelper.Layer.ActorBelow, SortingHelper.Order.Min);
                break;

            case SortEventType.LocationChanged:
                // Location change: selected hero above all
                if (this == e.Initiator)
                    SetSorting(SortingHelper.Layer.ActorAbove, SortingHelper.Order.Max);
                else
                    SetSorting(SortingHelper.Layer.ActorBelow, SortingHelper.Order.Min);
                break;

            case SortEventType.Drop:
                // Reset all actors to below
                SetSorting(SortingHelper.Layer.ActorBelow, SortingHelper.Order.Min);
                break;

            case SortEventType.ActorMoving:
                // Moving actor slightly above
                if (this == e.Initiator)
                    SetSorting(SortingHelper.Layer.ActorAbove, 0);
                else
                    SetSorting(SortingHelper.Layer.ActorBelow, SortingHelper.Order.Min);
                break;

            case SortEventType.Overlap:
                // Initiator on top, target below
                if (this == e.Initiator)
                    SetSorting(SortingHelper.Layer.ActorAbove, SortingHelper.Order.Max);
                else if (this == e.Target)
                    SetSorting(SortingHelper.Layer.ActorBelow, SortingHelper.Order.Min);
                break;

            case SortEventType.PincerAttack:
                // Determine role in participants
                bool isAttacker = e.Participants.pair
                                    .Any(p => p.attacker1 == this || p.attacker2 == this);
                bool isOpponent = e.Participants.pair
                                    .SelectMany(p => p.opponents)
                                    .Contains(this);
                bool isSupporter = e.Participants.pair
                                    .SelectMany(p => p.supporters1.Concat(p.supporters2))
                                    .Contains(this);

                if (isAttacker)
                    SetSorting(SortingHelper.Layer.ActorAbove, SortingHelper.Order.Attacker);
                else if (isOpponent)
                    SetSorting(SortingHelper.Layer.ActorAbove, SortingHelper.Order.Opponent);
                else if (isSupporter)
                    SetSorting(SortingHelper.Layer.ActorAbove, SortingHelper.Order.Supporter);
                else
                    SetSorting(SortingHelper.Layer.ActorBelow, SortingHelper.Order.Min);
                break;

            case SortEventType.Bump:
                if (this == e.Initiator)
                    SetSorting(SortingHelper.Layer.ActorAbove, SortingHelper.Order.Max);
                else if (this == e.Target)
                    SetSorting(SortingHelper.Layer.ActorAbove, SortingHelper.Order.Min);
                break;

            default:
                // Default fallback for all actors
                SetSorting(SortingHelper.Layer.ActorBelow, SortingHelper.Order.Min);
                break;
        }
    }

    #endregion

    // Fields: Core actors fields representing characterName Stats, state, and modules.
    [SerializeField] public AnimationCurve glowCurve;   // Curve defining Glow Animation behavior.
    public Vector2Int previousLocation;                 // Grid location before the last Move.
    public Vector3 previousPosition;                    // World position before the last Move.
    public Vector2Int location;                         // CurrentProfile grid location.
    public Team team = Team.Neutral;                    // Actor's team affiliation.
    public int spawnTurn = 0;                           // TurnManager number when the actor is eligible to spawn.
    public string characterName;                                // characterName actors for this actor.


    // Modules: Encapsulate various aspects of the actor such as rendering, Stats, Abilities, and animations.
    public ActorRenderers Render = new ActorRenderers();
    public ActorStats Stats = new ActorStats();
    public ActorFlags Flags = new ActorFlags();
    public ActorVFX Vfx = new ActorVFX();
    public ActorWeapon Weapon = new ActorWeapon();
    public ActorAnimation Animation = new ActorAnimation();
    public ActorMovement Move = new ActorMovement();
    public ActorHealthBar HealthBar = new ActorHealthBar();
    public ActorActionBar ActionBar = new ActorActionBar();
    public ActorGlow Glow = new ActorGlow();
    public ActorParallax Parallax = new ActorParallax();
    public ActorThumbnail Thumbnail;
    public List<Ability> Abilities = new List<Ability>();


    // Determines the cardinal or diagonal direction from this actor to another.
    // If mustBeAdjacent is true, returns Direction.None when the other actor is not adjacent.
    public Direction GetDirectionTo(ActorInstance other, bool mustBeAdjacent = true)
    {
        // Validate target before any access
        if (other == null)
        {
            Debug.LogError($"GetDirectionTo called with null 'other' by {name}");
            return Direction.None;
        }

        // Enforce adjacency only when requested
        if (mustBeAdjacent && !Geometry.IsAdjacentTo(this, other))
            return Direction.None;

        var deltaX = location.x - other.location.x;
        var deltaY = location.y - other.location.y;

        // Cardinal directions
        if (deltaX == 0 && deltaY > 0) return Direction.North;
        if (deltaX == 0 && deltaY < 0) return Direction.South;
        if (deltaX > 0 && deltaY == 0) return Direction.West;
        if (deltaX < 0 && deltaY == 0) return Direction.East;

        // Diagonals
        if (deltaX > 0 && deltaY > 0) return Direction.NorthWest;
        if (deltaX < 0 && deltaY > 0) return Direction.NorthEast;
        if (deltaX > 0 && deltaY < 0) return Direction.SouthWest;
        if (deltaX < 0 && deltaY < 0) return Direction.SouthEast;

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
            if (g.Actors.All.Any(actor => actor.IsPlaying && actor.location == checkPos))
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
            if (g.Actors.All.Any(actor => actor.IsPlaying && actor.location == checkPos))
                return true;
        }
        return false;
    }



    // Awake: Initialization of the actor g.Actors.All. Sets up modules and subscribes to events.
    private void Awake()
    {
        // Show modules with this actor actors context.
        Render.Initialize(this);
        Animation.Initialize(this);
        Move.Initialize(this);
        HealthBar.Initialize(this);
        ActionBar.Initialize(this);
        Glow.Initialize(this);
        Parallax.Initialize(this);
        Thumbnail = this.transform.Find(GameObjectHelper.Actor.Front.Thumbnail).GetComponent<ActorThumbnail>();

    }

    // OnDestroy: Clean up event subscriptions if necessary to prevent memory leaks.
    private void OnDestroy()
    {

    }

    // Show: Initializes and spawns the actor at the specified start location.
    public void Spawn(Vector2Int startLocation)
    {
        // Show CurrentProfile and previous locations.
        location = startLocation;
        previousLocation = location;

        // Save world position based on grid location.
        Position = Geometry.GetPositionByLocation(location);
        previousPosition = Position;

        // Generate the Thumbnail for UI/display purposes.
        Thumbnail.Initialize(this);

        // Randomly assign Weapon type and attributes.
        // TODO: Equip actor at stage manager load based on save file: party.json
        Weapon.Type = RNG.WeaponType();
        Weapon.Attack = RNG.Float(10, 15);
        Weapon.Defense = RNG.Float(0, 5);
        Weapon.Name = $"{Weapon.Type}";
        // Show the Weapon icon using resources.
        Render.weaponIcon.sprite = SpriteLibrary.WeaponTypes[Weapon.Type.ToString()];

        // Configure visual appearance and effects based on team.
        if (IsHero)
        {
            Render.SetOpaqueColor(ColorHelper.Solid.White);
            Render.SetQualityColor(ColorHelper.Solid.White);
            Render.SetGlowColor(ColorHelper.Solid.White);
            Render.SetParallaxSprite(SpriteLibrary.Seamless["WhiteFire2"]);
            Render.SetParallaxMaterial(MaterialLibrary.Materials["PlayerParallax"], Thumbnail.texture);
            Render.SetParallaxAlpha(Opacity.Percent50);
            Vfx.Attack = VfxLibrary.VisualEffects["BlueSlash1"];
        }
        else if (IsEnemy)
        {
            Render.SetOpaqueColor(ColorHelper.Solid.Black);
            Render.SetQualityColor(ColorHelper.Solid.GunMetal);
            Render.SetGlowColor(ColorHelper.Solid.GunMetal);
            Render.SetParallaxSprite(SpriteLibrary.Seamless["RedFire1"]);
            Render.SetParallaxMaterial(MaterialLibrary.Materials["EnemyParallax"], Thumbnail.texture);
            Render.SetParallaxAlpha(Opacity.Percent50);
            Render.SetFrameColor(ColorHelper.Solid.GunMetal);
            Vfx.Attack = VfxLibrary.VisualEffects["DoubleClaw"];



            SetInitialTurnDelay(3, 10);
        }

        // Show name tag textarea and toggle its visibility based on debug settings.
        Render.SetNameTagText(characterName);
        Render.SetNameTagEnabled(isEnabled: g.DebugManager.showActorNameTag);

        // Save health and Animation bars.
        HealthBar.Update();
        ActionBar.Reset();

        // Activate the actor if it is spawnable; otherwise, keep it inactive.
        if (IsSpawnable)
        {
            gameObject.SetActive(true);
            Flags.HasSpawned = true;
            Animation.FadeIn();
            Animation.Spin360();
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
        var attackStrategy = RNG.Strategy(ratios);

        Vector2Int targetLocation = LocationHelper.Nowhere;

        // SelectProfile target based on strategy.
        switch (attackStrategy)
        {
            case AttackStrategy.AttackClosest:
                // Pick the closest hero.
                var targetPlayer = g.Actors.Heroes.Where(x => x.IsPlaying).OrderBy(x => Vector3.Distance(x.Position, Position)).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;
            case AttackStrategy.AttackWeakest:
                // Pick the hero with the lowest HP.
                targetPlayer = g.Actors.Heroes.Where(x => x.IsPlaying).OrderBy(x => x.Stats.HP).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;
            case AttackStrategy.AttackStrongest:
                // Pick the hero with the highest HP.
                targetPlayer = g.Actors.Heroes.Where(x => x.IsPlaying).OrderByDescending(x => x.Stats.HP).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;
            case AttackStrategy.AttackRandom:
                // Pick a random hero's location.
                targetLocation = RNG.Hero.location;
                break;
            case AttackStrategy.MoveAnywhere:
                // Pick a random location.
                targetLocation = RNG.Location;
                break;
        }

        //Show the actor's location to the nearest valid attackResult location relative to the target.
        location = Geometry.GetClosestAttackLocation(location, targetLocation);
        //Note: nextPosition is commented out and could be used for future logic.
        //nextPosition = Geometry.GetPositionByLocation(nextLocation.Value);
    }

    public void FireDamage(float amount) => StartCoroutine(FireDamageRoutine(amount));
    public IEnumerator FireDamageRoutine(float amount)
    {
        g.CombatTextManager.Spawn($"Fireball: - {amount} HP", Position);
        yield return Wait.None();
    }


    public void Heal(int amount) => StartCoroutine(HealRoutine(amount));
    public IEnumerator HealRoutine(int amount)
    {
        // Immediately apply healing and update health.
        if (!IsInvincible)
        {
            Stats.PreviousHP = Stats.HP;
            Stats.HP += amount;
            Stats.HP = Mathf.Clamp(Stats.HP, 0, Stats.MaxHP);
            HealthBar.Update();
        }

        // Display healing combat text and play sound.
        g.CombatTextManager.Spawn(amount.ToString(), Position, "Heal");
        g.AudioManager.Play("Heal"); // Replace with your healing SFX key

        yield break;
    }



    //DamageRoutine: StartCoroutine that processes damage application, executes VfxManager and animations, and updates HP.
    public void Damage(AttackResult attackResult) => StartCoroutine(DamageRoutine(attackResult));
    public IEnumerator DamageRoutine(AttackResult attackResult)
    {
        // Immediately apply damage and update health.
        if (!IsInvincible)
        {
            Stats.PreviousHP = Stats.HP;
            Stats.HP -= attackResult.Damage;
            Stats.HP = Mathf.Clamp(Stats.HP, 0, Stats.MaxHP);
            HealthBar.Update();
        }

        var style = CombatTextHelper.GetStyle(attackResult);
        g.CombatTextManager.Spawn(attackResult.Damage.ToString(), Position, style);
        g.AudioManager.Play($"Slash{RNG.Int(1, 7)}");

        yield break;
    }


    //AttackMissRoutine: StartCoroutine to display a miss message and attackResult a dodge Animation.
    public IEnumerator AttackMissRoutine()
    {
        g.CombatTextManager.Spawn("Miss", Position);
        yield return Animation.DodgeRoutine();
    }

    //Die: Initiates the actor's death sequence.
    public void Die()
    {
        StartCoroutine(DieRoutine());
    }

    //DieRoutine: StartCoroutine that handles the actor's death sequence, including fading out, spawning coins, and deactivation.
    public IEnumerator DieRoutine()
    {
        //Before: Show actor to fully opaque.
        var alpha = 1f;
        Render.SetAlpha(alpha);

        //Wait until the health fill has finished draining.
        if (HealthBar.isDraining)
            yield return new WaitUntil(() => HealthBar.isEmpty);

        //ProcessRoutine portrait dissolve effect and play death sound.
        g.Portrait3DManager.Dissolve(this);
        g.AudioManager.Play("Death");

        //Show sorting order to maximum so that the death sequence renders on top.
        //sortingOrder = SortingOrder.Max;

        //During: Gradually reduce the alpha value for a overlay-out effect.
        var hasSpawnedCoins = false;
        while (alpha > 0f)
        {
            alpha -= Increment.Percent1;
            alpha = Mathf.Clamp(alpha, Increment.Transparent, Opacity.Opaque);
            Render.SetAlpha(alpha);

            //Show coins when attacker fades below 10% opacity, if not already spawned.
            if (IsEnemy && !hasSpawnedCoins && alpha < Opacity.Percent10)
            {
                hasSpawnedCoins = true;
                int amount = 10;
                SpawnCoins(amount);
            }

            yield return Wait.OneTick();
        }

        //After: Reset location and position, deactivate the actor, and check death event.
        location = LocationHelper.Nowhere;
        Position = PositionHelper.Nowhere;
        gameObject.SetActive(false);
        g.StageManager.OnActorDeath();
    }

    //SpawnCoins: Helper function to begin spawning coins upon attacker death.
    private void SpawnCoins(int amount)
    {
        if (IsPlaying)
            StartCoroutine(SpawnCoinsRoutine(amount)); // TODO: Adjust coin spawning based on attacker Stats if necessary.
    }

    //SpawnCoinsRoutine: StartCoroutine that spawns a specified number of coins at the actor's position.
    IEnumerator SpawnCoinsRoutine(int amount)
    {
        var i = 0;
        do
        {
            g.CoinManager.Spawn(Position);
            i++;
        } while (i < amount);

        yield return true;
    }

    //Teleport: Moves the actor instantly to a new grid location if within board bounds.
    public void Teleport(Vector2Int newLocation)
    {
        //if (newLocation == null)
        //    newLocation = LocationHelper.Nowhere;

        //Abort if the new location is out of bounds.
        if (!g.Board.InBounds(newLocation))
            return;

        var occupant = g.Actors.All.FirstOrDefault(x => x.IsPlaying && x.location == newLocation);
        if (occupant.Exists())
            occupant.Teleport(RNG.Location);

        this.location = newLocation;
        transform.position = Geometry.GetPositionByLocation(location);
    }

    /// <summary>
    /// Teleports this actor to the first unoccupied tile that comes AFTER the given position
    /// using the board's natural order (top-left to bottom-right). Wraps around at the end.
    /// Requires g.Tiles to be ordered top-left to bottom-right.
    /// </summary>
    public void TeleportAfter(Vector2Int after)
    {
        var tiles = g.Tiles.ToList();
        if (tiles.Count == 0)
        {
            Debug.LogWarning("TeleportAfter: no tiles available.");
            return;
        }

        int startIndex = tiles.FindIndex(t => t != null && t.location == after);
        if (startIndex < 0)
        {
            Debug.LogWarning($"TeleportAfter: starting tile {after} not found.");
            return;
        }

        // Scan from the next tile, wrapping around once if needed
        for (int step = 1; step <= tiles.Count; step++)
        {
            int idx = (startIndex + step) % tiles.Count;
            var tile = tiles[idx];

            if (tile != null && !tile.IsOccupied)
            {
                Teleport(tile.location);
                return;
            }
        }

        Debug.LogWarning($"TeleportAfter: no unoccupied tile found after {after}.");
    }

    //Seek: Attempts to Move the actor in the specified direction if the target location is valid.
    public void TeleportToward(Vector2Int direction)
    {
        //Abort if the new location (CurrentProfile location + direction) is out of bounds.
        if (!g.Board.InBounds(location + direction))
            return;

        var newLocation = location + direction;
        var tile = g.TileMap.GetTile(newLocation);
        if (tile == null) return;
        // Teleport to the new tile's location.
        Teleport(tile.location);
    }

    //SetReady: Resets the attacker actor's Animation points for a new turn.
    public void SetReady()
    {
        //Abort if the actor is not active, not alive, or not an attacker.
        if (!IsActive || !IsAlive || !IsEnemy)
            return;

        Stats.AP = Stats.MaxAP;
        Stats.PreviousAP = Stats.MaxAP;

        //Save the Animation fill UI to reflect the refreshed Animation points.
        ActionBar.Update();
    }
}
