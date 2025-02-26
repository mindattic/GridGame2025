using Assets.Scripts.Behaviors.Actor;
using Assets.Scripts.Instances.Actor;
using Assets.Scripts.Models;
using Game.Instances.Actor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActorInstance : MonoBehaviour
{
    //External properties
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected BoardInstance board => GameManager.instance.board;
    protected CoinManager coinManager => GameManager.instance.coinManager;
    protected DamageTextManager damageTextManager => GameManager.instance.damageTextManager;
    protected DebugManager debugManager => GameManager.instance.debugManager;
    protected StageManager stageManager => GameManager.instance.stageManager;
    protected PortraitManager portraitManager => GameManager.instance.portraitManager;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected TileManager tileManager => GameManager.instance.tileManager;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    protected float moveSpeed => GameManager.instance.moveSpeed;
    protected float snapDistance => GameManager.instance.snapDistance;
    protected float tileSize => GameManager.instance.tileSize;
    protected Vector3 tileScale => GameManager.instance.tileScale;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    protected List<ActorInstance> actors => GameManager.instance.actors;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;
    protected bool hasFocusedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;

    //Internal properties
    public TileInstance currentTile => board.tileMap.GetTile(location); //tiles.First(x => x.location.Equals(location));
    public bool isPlayer => team.Equals(Team.Player);
    public bool isEnemy => team.Equals(Team.Enemy);
    public bool isFocusedPlayer => hasFocusedActor && Equals(focusedActor);
    public bool isSelectedPlayer => hasSelectedPlayer && Equals(selectedPlayer);
    public bool onNorthEdge => location.y == 1;
    public bool onEastEdge => location.x == board.columnCount;
    public bool onSouthEdge => location.y == board.rowCount;
    public bool onWestEdge => location.x == 1;
    public bool isActive => isActiveAndEnabled;
    public bool isAlive => stats.HP > 0;
    public bool isPlaying => isActive && isAlive;
    public bool isDying => isActive && stats.HP < 1;
    public bool isDead => !isActive && !isAlive;
    public bool isSpawnable => !hasSpawned && spawnTurn <= turnManager.currentTurn;
    public bool hasMaxAP => stats.AP == stats.MaxAP;

    public bool isInvincible => (isEnemy && debugManager.isEnemyInvincible) || (isPlayer && debugManager.isPlayerInvincible);
    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }
    public Vector3 position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }
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
            render.overlay.sortingOrder = value + ActorLayer.Value.Overlay;
            render.selectionBox.sortingOrder = value + ActorLayer.Value.SelectionBox;
            OnSortingOrderChanged?.Invoke();
        }
    }

    //System.Action event handlers
    public System.Action<ActorInstance> onOverlapDetected;
    public System.Action onActorDeath;
    public System.Action OnSortingOrderChanged;

    //Fields
    [SerializeField] public AnimationCurve glowCurve;
    public Character character;
    public Vector2Int previousLocation;
    public Vector2Int location;
    public Team team = Team.Neutral;
    public int spawnTurn = 0;
    public bool hasSpawned = false;
    public int attackingPairCount = 0;
    public int supportingPairCount = 0;
    public float wiggleSpeed;
    public float wiggleAmplitude;

    //Modules
    public ActorRenderers render = new ActorRenderers();
    public ActorStats stats = new ActorStats();
    public ActorFlags flags = new ActorFlags();
    public ActorAbilities abilities = new ActorAbilities();
    public ActorVFX vfx = new ActorVFX();
    public ActorWeapon weapon = new ActorWeapon();
    public ActorActions action = new ActorActions();
    public ActorMovement move = new ActorMovement();
    public ActorHealthBar healthBar = new ActorHealthBar();
    public ActorActionBar actionBar = new ActorActionBar();
    public ActorGlow glow = new ActorGlow();
    public ActorParallax parallax = new ActorParallax();
    public ActorThumbnail thumbnail = new ActorThumbnail();

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

    public Direction GetDirectionTo(ActorInstance other, bool mustBeAdjacent = false)
    {
        if (mustBeAdjacent && !IsAdjacentTo(other.location))
            return Direction.None;

        var deltaX = location.x - other.location.x;
        var deltaY = location.y - other.location.y;

        //Handle simple cardinal directions
        if (deltaX == 0 && deltaY > 0) return Direction.North;
        if (deltaX == 0 && deltaY < 0) return Direction.South;
        if (deltaX > 0 && deltaY == 0) return Direction.West;
        if (deltaX < 0 && deltaY == 0) return Direction.East;

        //Handle diagonals
        if (deltaX > 0 && deltaY > 0) return Direction.NorthWest;
        if (deltaX < 0 && deltaY > 0) return Direction.NorthEast;
        if (deltaX > 0 && deltaY < 0) return Direction.SouthWest;
        if (deltaX < 0 && deltaY < 0) return Direction.SouthEast;

        //Default case for no movement or invalid input
        return Direction.None;
    }

    private void Awake()
    {
        //gameObject.SetActive(false);

        render.Initialize(this);
        action.Initialize(this);
        move.Initialize(this);
        healthBar.Initialize(this);
        actionBar.Initialize(this);
        glow.Initialize(this);
        parallax.Initialize(this);
        thumbnail.Initialize(this);

        wiggleSpeed = tileSize * 24f;
        wiggleAmplitude = 15f;  //Amplitude (difference from -45 degrees)

        //Events
        onOverlapDetected += (actor) => move.OnOverlapDetected(actor);
        onActorDeath += stageManager.OnActorDeath;

    }

    private void OnDestroy()
    {
        onOverlapDetected -= move.OnOverlapDetected;
        onActorDeath -= stageManager.OnActorDeath;
    }

    public void Spawn(Vector2Int startLocation)
    {
        location = startLocation;
        previousLocation = location;
        position = Geometry.GetPositionByLocation(location);

        thumbnail.Generate();

        //TODO: Equip actor at stagemanger load based on save file: party.json
        weapon.Type = Random.WeaponType();
        weapon.Attack = Random.Float(10, 15);
        weapon.Defense = Random.Float(0, 5);
        weapon.Name = $"{weapon.Type}";
        render.weaponIcon.sprite = resourceManager.WeaponType(weapon.Type.ToString()).Value;

        if (isPlayer)
        {
            render.SetOpaqueColor(ColorHelper.Solid.White);
            render.SetQualityColor(ColorHelper.Solid.White);
            render.SetGlowColor(ColorHelper.Solid.White);
            render.SetParallaxSprite(resourceManager.Seamless("WhiteFire2").Value);
            render.SetParallaxMaterial(resourceManager.Material("PlayerParallax", thumbnail.texture).Value);
            render.SetParallaxAlpha(Opacity.Percent50);
            render.SetSelectionBoxEnabled(isEnabled: false);
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
            render.SetSelectionBoxEnabled(isEnabled: false);
            vfx.Attack = resourceManager.VisualEffect("DoubleClaw");
        }

        render.SetNameTagText(name);
        render.SetNameTagEnabled(isEnabled: debugManager.showActorNameTag);

        healthBar.Update();
        actionBar.Reset();

        if (isSpawnable)
        {
            gameObject.SetActive(true);
            hasSpawned = true;
            action.TriggerFadeIn();
            action.TriggerSpin360();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public IEnumerator Attack(AttackResult attack)
    {
        yield return vfxManager.Spawn(
            vfx.Attack,
            attack.Opponent.position,
            new Trigger(coroutine: attack.IsHit ? attack.Opponent.TakeDamage(attack) : attack.Opponent.AttackMiss(),
                        isAsync: false));
    }

    public void CalculateAttackStrategy()
    {
        //Randomly select an Strength attackStrategy
        int[] ratios = { 50, 20, 15, 10, 5 };
        var attackStrategy = Random.Strategy(ratios);


        Vector2Int targetLocation = board.NowhereLocation;

        switch (attackStrategy)
        {
            case AttackStrategy.AttackClosest:
                var targetPlayer = players.Where(x => x.isPlaying).OrderBy(x => Vector3.Distance(x.position, position)).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;

            case AttackStrategy.AttackWeakest:
                targetPlayer = players.Where(x => x.isPlaying).OrderBy(x => x.stats.HP).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;

            case AttackStrategy.AttackStrongest:
                targetPlayer = players.Where(x => x.isPlaying).OrderByDescending(x => x.stats.HP).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;

            case AttackStrategy.AttackRandom:
                targetLocation = Random.Player.location;
                break;

            case AttackStrategy.MoveAnywhere:
                targetLocation = Random.Location;
                break;
        }

        location = Geometry.GetClosestAttackLocation(location, targetLocation);
        //nextPosition = Geometry.GetPositionByLocation(nextLocation.Value);
    }

    public void TriggerTakeDamage(AttackResult attack)
    {
        if (!isActive || !isAlive)
            return;

        StartCoroutine(TakeDamage(attack));
    }

    public IEnumerator FireDamage(float amount)
    {
        damageTextManager.Spawn($"Fireball: - {amount} HP", position);
        yield return Wait.UntilNextFrame();
    }

    public IEnumerator Heal(float amount)
    {
        damageTextManager.Spawn($"Heal: +{amount} HP", position);
        yield return Wait.UntilNextFrame();
    }

    public IEnumerator TakeDamage(AttackResult attack)
    {
        //Check abort conditions
        if (!isActive || !isAlive)
            yield break;

        if (attack.IsCriticalHit)
            vfxManager.TriggerSpawn(resourceManager.VisualEffect("YellowHit"), attack.Opponent.position);

        //Trigger coroutine (if applicable):
        //yield return attack.Triggers.Before.StartCoroutine(this);

        //Before:
        float ticks = 0f;
        float duration = Interval.TenTicks;

        if (!isInvincible)
        {
            stats.PreviousHP = stats.HP;
            stats.HP -= attack.Damage;
            stats.HP = Mathf.Clamp(stats.HP, 0, stats.MaxHP);
            healthBar.Update();
        }

        damageTextManager.Spawn(attack.Damage.ToString(), position);
        audioManager.Play($"Slash{Random.Int(1, 7)}");

        //During:
        while (ticks < duration)
        {
            action.TriggerGrow();
            if (attack.IsCriticalHit)
                action.TriggerShake(ShakeIntensity.Medium);

            ticks += Interval.OneTick;
            yield return Wait.For(Interval.OneTick);
        }

        //Trigger coroutine (if applicable):
        //yield return attack.Triggers.After.StartCoroutine(this);

        //After:
        action.TriggerShrink();
        action.TriggerShake(ShakeIntensity.Stop);
    }

    public IEnumerator AttackMiss()
    {
        damageTextManager.Spawn("Miss", position);
        yield return action.Dodge();
    }

    public void TriggerDie()
    {
        StartCoroutine(Die());
    }

    public IEnumerator Die()
    {
        //Check abort conditions
        if (!isDying)
            yield break;

        //Before:
        var alpha = 1f;
        render.SetAlpha(alpha);

        //Wait for health bar to finish draining
        if (healthBar.isDraining)
            yield return new WaitUntil(() => healthBar.isEmpty);
        //while (healthBar.isDraining)
        //    yield return Wait.UntilNextFrame();

        //yield return Wait.For(Intermission.After.HealthBar.Empty);

        portraitManager.Dissolve(this);
        audioManager.Play("Death");
        sortingOrder = SortingOrder.Max;

        //During:
        var hasSpawnedCoins = false;
        while (alpha > 0f)
        {
            alpha -= Increment.OnePercent;
            alpha = Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Opaque);
            render.SetAlpha(alpha);

            if (isEnemy && !hasSpawnedCoins && alpha < Opacity.Percent10)
            {
                hasSpawnedCoins = true;
                int amount = 10;
                TriggerSpawnCoins(amount);
            }

            yield return Wait.OneTick();
        }

        //After:       
        location = board.NowhereLocation;
        position = board.NowherePosition;
        gameObject.SetActive(false);
        onActorDeath.Invoke();
    }

    private void TriggerSpawnCoins(int amount)
    {
        if (isPlaying)
            StartCoroutine(SpawnCoins(amount)); //TODO: TriggerSpawn coins based on enemy Stats...
    }

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



    public void Teleport(Vector2Int location)
    {

        this.location = location;
        transform.position = Geometry.GetPositionByLocation(this.location);
    }


    public void SetAttacking()
    {
        flags.IsAttacking = true;
        attackingPairCount++;
        sortingOrder = SortingOrder.Attacker;
    }

    public void SetDefending()
    {
        flags.IsDefending = true;
        sortingOrder = SortingOrder.Target;
    }

    public void SetSupporting()
    {
        flags.IsSupporting = true;
        supportingPairCount++;
        sortingOrder = SortingOrder.Supporter;
    }

    public void SetDefault()
    {
        flags.IsAttacking = false;
        flags.IsDefending = false;
        flags.IsSupporting = false;
        sortingOrder = SortingOrder.Default;
    }


    public void SetReady()
    {
        //Check abort conditions
        if (!isActive || !isAlive || !isEnemy)
            return;

        stats.AP = stats.MaxAP;
        stats.PreviousAP = stats.MaxAP;

        actionBar.Update();
    }



}
