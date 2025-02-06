using Assets.Scripts.Behaviors.Actor;
using Assets.Scripts.Instances.Actor;
using Assets.Scripts.Models;
using Game.Instances.Actor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ActorInstance : MonoBehaviour
{
    //External properties
    protected AudioManager audioManager => GameManager.instance.audioManager;
    protected BoardInstance board => GameManager.instance.board;
    protected CoinManager coinManager => GameManager.instance.coinManager;
    protected DamageTextManager damageTextManager => GameManager.instance.damageTextManager;
    protected DebugManager debugManager => GameManager.instance.debugManager;
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
    public bool isAlive => isActive && stats.HP > 0;
    public bool isReady => isActive && isAlive && stats.AP == stats.MaxAP;
    public bool isDying => isActive && stats.HP < 1;
    public bool isDead => !isActive && stats.HP < 1;
    public bool isSpawnable => !isActive && isAlive && spawnDelay <= turnManager.currentTurn;
    public bool hasMaxAP => isActive && isAlive && stats.AP == stats.MaxAP;
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
        }
    }

    //Events
    public Action<ActorInstance> OnOverlapDetected;

    //Fields
    [SerializeField] public AnimationCurve glowCurve;
    public Character character;
    public Vector2Int previousLocation;
    public Vector2Int location;
    public Team team = Team.Neutral;
    public int spawnDelay = -1;
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

    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
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

        //nextLocation = null;
        //redirectedLocation = null;
        //nextPosition = null;

        //Event bindings
        OnOverlapDetected += (other) =>
        {
            //Debug.Log($"[OnOverlapDetected] {name} received event from {other.name}");
            move.HandleOnOverlapDetected(other);
        };
    }

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


    public void Spawn(Vector2Int startLocation)
    {
        gameObject.SetActive(true);

        previousLocation = startLocation;
        location = startLocation;

        position = Geometry.GetPositionByLocation(location);

        //sprites = resourceManager.ActorSprite(this.character.ToString());
        thumbnail.Generate();

        //TODO: Equip actor at stagemaanger load based on save file: party.json
        weapon.Type = Random.WeaponType();
        weapon.Attack = Random.Float(10, 15);
        weapon.Defense = Random.Float(0, 5);
        weapon.Name = $"{weapon.Type}";
        render.weaponIcon.sprite = resourceManager.WeaponType(weapon.Type.ToString()).Value;

        if (isPlayer)
        {
            render.SetQualityColor(ColorHelper.Solid.White);
            render.SetGlowColor(ColorHelper.Solid.White);
            render.SetParallaxSprite(resourceManager.Seamless("WhiteFire2").Value);


            render.SetParallaxMaterial(resourceManager.Material("PlayerParallax", thumbnail.texture).Value);
            render.SetParallaxAlpha(Opacity.Percent50);
            //render.SetParallaxSpeed(1, 1);

            //render.SetThumbnailMaterial(resourceManager.Material("Sprites-Default", thumbnailSettings.texture));
            //render.SetFrameColor(quality.Color);
            render.SetHealthBarColor(ColorHelper.HealthBar.Green);
            render.SetActionBarColor(ColorHelper.ActionBar.Blue);
            render.SetSelectionBoxEnabled(isEnabled: false);
            vfx.Attack = resourceManager.VisualEffect("BlueSlash1");
        }
        else if (isEnemy)
        {
            render.SetQualityColor(ColorHelper.Solid.Red);
            render.SetGlowColor(ColorHelper.Solid.Red);
            render.SetParallaxSprite(resourceManager.Seamless("RedFire1").Value);
            render.SetParallaxMaterial(resourceManager.Material("EnemyParallax", thumbnail.texture).Value);
            render.SetParallaxAlpha(Opacity.Percent50);
            //render.SetParallaxSpeed(1, 1);
            //render.SetThumbnailMaterial(resourceManager.Material("Sprites-Default", thumbnailSettings.texture));
            render.SetFrameColor(ColorHelper.Solid.Red);
            render.SetHealthBarColor(ColorHelper.HealthBar.Green);
            render.SetActionBarColor(ColorHelper.ActionBar.Blue);
            render.SetSelectionBoxEnabled(isEnabled: false);
            vfx.Attack = resourceManager.VisualEffect("DoubleClaw");
        }

        render.SetNameTagText(name);
        render.SetNameTagEnabled(isEnabled: debugManager.showActorNameTag);

        healthBar.Update();
        actionBar.Reset();
        action.TriggerFadeIn();
        action.TriggerSpin360();
    }


    private void Update()
    {
        CheckRotation();
    }
    void FixedUpdate()
    {
    }


    private void CheckRotation()
    {
        if (transform.localEulerAngles.y == 0f)
            return;

        bool isFlipped = transform.localEulerAngles.y > 90f && transform.localEulerAngles.y < 270f;

        //Toggle visibility
        render.front.gameObject.SetActive(!isFlipped);
        render.back.gameObject.SetActive(isFlipped);
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
                var targetPlayer = players.Where(x => x.isActive && x.isAlive).OrderBy(x => Vector3.Distance(x.position, position)).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;

            case AttackStrategy.AttackWeakest:
                targetPlayer = players.Where(x => x.isActive && x.isAlive).OrderBy(x => x.stats.HP).FirstOrDefault();
                targetLocation = targetPlayer.location;
                break;

            case AttackStrategy.AttackStrongest:
                targetPlayer = players.Where(x => x.isActive && x.isAlive).OrderByDescending(x => x.stats.HP).FirstOrDefault();
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
            yield return new WaitUntil(() => stats.PreviousHP == stats.HP);
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
        //nextPosition = null;

        gameObject.SetActive(false);
    }

    private void TriggerSpawnCoins(int amount)
    {
        if (isActive && isAlive)
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

    //public void ExecuteAngry()
    //{
    //   if (isActive && isAlive)
    //       StartCoroutine(Angry());
    //}

    //private IEnumerator Angry()
    //{
    //   //Check abort conditions
    //   if (!hasMaxAP || flags.isAngry)
    //       yield break;

    //   //Before:
    //   flags.isAngry = true;
    //   bool isDone = false;
    //   bool hasFlipped = false;
    //   var rotY = 0f;
    //   var speed = tileSize * 24f;
    //   render.turnDelayText.gameObject.transform.rotation = Geometry.rotation(0, rotY, 0);

    //   //During:
    //   while (!isDone)
    //   {
    //       rotY += !hasFlipped ? speed : -speed;

    //       if (!hasFlipped && rotY >= 90f)
    //       {
    //           rotY = 90f;
    //           hasFlipped = true;
    //           //turnDelay--;
    //           //turnDelay = Math.Clamp(turnDelay, 0, 9);

    //           //actionBar.Update();
    //           //UpdateTurnDelayText();
    //       }

    //       isDone = hasFlipped && rotY <= 0f;
    //       if (isDone)
    //       {
    //           rotY = 0f;
    //       }

    //       //render.turnDelayText.gameObject.transform.rotation = Geometry.rotation(0, rotY, 0);
    //       yield return Wait.OneTick();
    //   }

    //   //After:
    //   //render.turnDelayText.gameObject.transform.rotation = Geometry.rotation(0, 0, 0);
    //   //if (turnDelay > 0)
    //   //{
    //   //   TriggerTurnDelayWiggle();
    //   //}

    //   IEnumerator SetThumbnailSprite()
    //   {
    //       render.SetThumbnailSprite(sprites.attack);
    //       yield break;
    //   }
    //   Trigger trigger = new Trigger(SetThumbnailSprite());
    //   action.TriggerSpin90(trigger);
    //}

    //public IEnumerator Bump(Direction direction)
    //{

    //   //Before:
    //   BumpStage stage = BumpStage.StartCoroutine;
    //   var targetPosition = position;
    //   var range = tileSize * percent33;
    //   sortingOrder = SortingOrder.Default;

    //   //During:
    //   while (stage != BumpStage.End)
    //   {
    //       switch (stage)
    //       {
    //           case BumpStage.StartCoroutine:
    //               {
    //                   sortingOrder = SortingOrder.Max;
    //                   position = currentTile.position;
    //                   targetPosition = Geometry.GetDirectionalPosition(position, direction, range);
    //                   stage = BumpStage.MoveToward;
    //               }
    //               break;

    //           case BumpStage.MoveToward:
    //               {
    //                   var delta = targetPosition - position;
    //                   if (Mathf.Abs(delta.x) > bumpSpeed)
    //                       position = Vector2.MoveTowards(position, new Vector3(targetPosition.x, position.y, position.z), bumpSpeed);
    //                   else if (Mathf.Abs(delta.y) > bumpSpeed)
    //                       position = Vector2.MoveTowards(position, new Vector3(position.x, targetPosition.y, position.z), bumpSpeed);

    //                   var isSnapDistance = Vector2.Distance(position, targetPosition) <= bumpSpeed;
    //                   if (isSnapDistance)
    //                   {
    //                       position = targetPosition;
    //                       targetPosition = currentTile.position;
    //                       stage = BumpStage.MoveAway;
    //                   }
    //               }
    //               break;

    //           case BumpStage.MoveAway:
    //               {
    //                   var delta = targetPosition - position;
    //                   if (Mathf.Abs(delta.x) > bumpSpeed)
    //                       position = Vector2.MoveTowards(position, new Vector3(targetPosition.x, position.y, position.z), bumpSpeed);
    //                   else if (Mathf.Abs(delta.y) > bumpSpeed)
    //                       position = Vector2.MoveTowards(position, new Vector3(position.x, targetPosition.y, position.z), bumpSpeed);

    //                   var isSnapDistance = Vector2.Distance(position, targetPosition) <= bumpSpeed;
    //                   if (isSnapDistance)
    //                   {
    //                       position = targetPosition;
    //                       targetPosition = currentTile.position;
    //                       stage = BumpStage.End;
    //                   }
    //               }
    //               break;

    //           case BumpStage.End:
    //               {
    //                   sortingOrder = SortingOrder.Default;
    //                   position = targetPosition;
    //               }
    //               break;
    //       }

    //       yield return Wait.OneTick();
    //   }

    //   //After:
    //   sortingOrder = SortingOrder.Default;
    //   position = targetPosition;
    //}

    //public void ParallaxFadeOutAsync()
    //{
    //   if (!isActive && isAlive)
    //       return;
    //   TriggerFadeOut(render.parallax, TriggerFill.FivePercent, Interval.OneTick, startAlpha: 0.5f, endAlpha: 0f);
    //}


    //public IEnumerator TriggerFadeIn(
    //    SpriteRenderer spriteRenderer,
    //    float increment,
    //    float interval,
    //    float startAlpha = 0f,
    //    float endAlpha = 1f)
    //{
    //   //Before:
    //   var alpha = startAlpha;
    //   spriteRenderer.color = new Color(1, 1, 1, alpha);

    //   //During:
    //   while (alpha < endAlpha)
    //   {
    //       alpha += increment;
    //       alpha = Mathf.Clamp(alpha, 0, endAlpha);
    //       spriteRenderer.color = new Color(1, 1, 1, alpha);
    //       yield return interval;
    //   }

    //   //After:
    //   spriteRenderer.color = new Color(1, 1, 1, endAlpha);
    //}

    //public void TriggerFadeIn(
    //  SpriteRenderer spriteRenderer,
    //  float increment,
    //  float interval,
    //  float startAlpha = 0f,
    //  float endAlpha = 1f)
    //{
    //   StartCoroutine(TriggerFadeIn(spriteRenderer, increment, interval, startAlpha, endAlpha));
    //}


    //public IEnumerator FillRadial()
    //{
    //Before:
    //flags.IsWaiting = true;

    //During:
    //while (hasMovingPlayer)
    //{
    //if (ap < apMax)
    //{
    //   ap += Stats.Speed;
    //   ap = Math.Clamp(ap, 0, apMax);
    //}

    //TriggerFill ring
    //var fill = 360 - (360 * (ap / apMax));
    //render.radial.material.SetFloat("_Arc2", fill);

    //Drain ring
    //var fill = (360 * (ap / apMax));
    //render.radial.material.SetFloat("_Arc1", fill);

    //Write text
    //render.radialText.text = ap < apMax ? $"{Math.Round(ap / apMax * 100)}" : "100";

    //yield return Wait.OneTick();
    //}




    //}


    //void Update()
    //{
    //if (!isTurnDelayWiggling)
    //{
    //   isTurnDelayWiggling = Random.Int(1, 20) == 1 ? true : false;
    //   StartCoroutine(TurnDelayWiggle());
    //}


    //Check abort status
    //if (!isActive && isAlive || isMoving)
    //   return;

    //var closestTile = Geometry.GetClosestTile(boardPosition);
    //if (boardLocation != closestTile.boardLocation)
    //{
    //   previousLocation = boardLocation;


    //   audioManager.Start($"Move{Random.Int(1, 6)}");

    //   var overlappingActor = GetOverlappingActor(closestTile);

    //   //Assign overlapping actors boardLocation to currentFps actor's boardLocation
    //   if (overlappingActor != null)
    //   {
    //       overlappingActor.boardLocation = boardLocation;
    //       overlappingActor.nextPosition = Geometry.GetPositionByLocation(overlappingActor.boardLocation);
    //       overlappingActor.isMoving = true;
    //       StartCoroutine(overlappingActor.MoveTowardDestination());
    //   }

    //   //Assign currentFps actor's boardLocation to closest tile boardLocation
    //   boardLocation = closestTile.boardLocation;
    //   StartCoroutine(MoveTowardDestination());
    //}
    //}


    //public void AssignSkillWait()
    //{
    //   //Check abort conditions
    //   if (!isActive && isAlive)
    //       return;

    //   //TODO: Calculate based on Stats....
    //   float min = (Interval.OneSecond * 20) - Stats.Agility * Formulas.LuckModifier(Stats);
    //   float max = (Interval.OneSecond * 40) - Stats.Agility * Formulas.LuckModifier(Stats);

    //   sp = 0;
    //   apMax = Random.Float(min, max);
    //}

    //private void UpdateTurnDelayText()
    //{
    //   render.SetTurnDelayTextEnabled(turnDelay > 0);
    //   render.SetTurnDelayText($"{turnDelay}");
    //}


    //public IEnumerator RadialBackFadeIn()
    //{
    //   //Before:
    //   var maxAlpha = 0.5f;
    //   var alpha = 0f;
    //   render.radialBack.color = new color(0, 0, 0, alpha);

    //   //During:
    //   while (alpha < maxAlpha)
    //   {
    //       alpha += TriggerFill.OnePercent;
    //       alpha = Mathf.Clamp(alpha, 0, maxAlpha);
    //       render.radialBack.color = new color(0, 0, 0, alpha);
    //       yield return Wait.OneTick();
    //   }

    //   //After:
    //   render.radialBack.color = new color(0, 0, 0, maxAlpha);
    //}

    //public IEnumerator RadialBackFadeOut()
    //{
    //   //Before:
    //   var maxAlpha = 0.5f;
    //   var alpha = maxAlpha;
    //   render.radialBack.color = new color(0, 0, 0, maxAlpha);

    //   //During:
    //   while (alpha > 0)
    //   {
    //       alpha -= TriggerFill.OnePercent;
    //       alpha = Mathf.Clamp(alpha, 0, maxAlpha);
    //       render.radialBack.color = new color(0, 0, 0, alpha);
    //       yield return Destroy.OneTick();
    //   }

    //   //After:
    //   render.radialBack.color = new color(0, 0, 0, 0);
    //}

    //public void CalculateTurnDelay()
    //{
    //   IEnumerator Attack()
    //   {
    //       render.SetThumbnailSprite(sprites.idle);

    //       //render.SetThumbnailMaterial(resourceManager.Material("Sprites-Default", idle.texture));
    //       yield return Wait.Continue(); //Wait until the next frame
    //   }
    //   TriggerSpin90(Attack());

    //   //turnDelay = Formulas.CalculateTurnDelay(Stats);
    //   //UpdateTurnDelayText();

    //   UpdateActionBar();
    //}


    //private void Swap(Vector2Int newLocation)
    //{
    //   boardLocation = newLocation;
    //   nextPosition = Geometry.GetPositionByLocation(boardLocation);
    //   isMoving = true;
    //   StartCoroutine(MoveTowardDestination());
    //}



    //public void AssignActionWait()
    //{
    //   //Check abort conditions
    //   if (!isActive && isAlive)
    //       return;

    //   //TODO: Calculate based on Stats....
    //   float min = (Interval.OneSecond * 10) - amplitude * LuckModifier;
    //   float max = (Interval.OneSecond * 20) - amplitude * LuckModifier;

    //   ap = 0;
    //   maxAp = Random.Float(min, max);

    //   render.SetActionBarColor(Colors.ActionBarFill.Blue);
    //}

    //public void CheckActionBar()
    //{
    //Check abort conditions
    //if (!isActive && isAlive || turnManager.isEnemyTurn || (!turnManager.isStartPhase && !turnManager.isMovePhase))
    //   return;


    //if (ap < maxAp)
    //{
    //   ap += Time.deltaTime;
    //   ap = Math.Clamp(ap, 0, maxAp);
    //}
    //else
    //{

    //   render.CycleActionBarColor();
    //}

    //UpdateActionBar();
    //}

    //public void UpdateActionBar()
    //{
    //   var scale = render.actionBarBack.transform.localScale;
    //   var x = Mathf.Clamp(scale.x * (ap / maxAp), 0, scale.x);
    //   render.actionBarFill.transform.localScale = new Vector3(x, scale.y, scale.z);

    //   Percent complete
    //   render.actionBarText.text = ap < maxAp ? $@"{Math.Round(ap / maxAp * 100)}" : "";

    //   Seconds remaining
    //   render.radialText.text = ap < maxAp ? $"{Math.Round(maxAp - ap)}" : "";



    //}

    //public void SetStatus(Status icon)
    //{
    //   //Check abort conditions
    //   if (!isActive)
    //       return;

    //   StartCoroutine(SetStatusIcon(icon));
    //}

    //private IEnumerator SetStatusIcon(Status status)
    //{
    //   //Before:
    //   float increment = TriggerFill.FivePercent;
    //   float alpha = render.statusIcon.color.a;
    //   render.statusIcon.color = new Color(1, 1, 1, alpha);

    //   //During:
    //   while (alpha > 0)
    //   {
    //       alpha -= increment;
    //       alpha = Mathf.Clamp(alpha, 0, 1);
    //       render.statusIcon.color = new Color(1, 1, 1, alpha);
    //       yield return Wait.OneTick();
    //   }

    //   //Before:
    //   render.statusIcon.sprite = resourceManager.statusSprites.First(x => x.id.Equals(status.ToString())).thumbnailSettings;
    //   alpha = 0;
    //   render.statusIcon.color = new Color(1, 1, 1, alpha);

    //   //During:
    //   while (alpha < 1)
    //   {
    //       alpha += increment;
    //       alpha = Mathf.Clamp(alpha, 0, 1);
    //       render.statusIcon.color = new Color(1, 1, 1, alpha);

    //       yield return Wait.OneTick();
    //   }
    //}

    //private void CheckBobbing()
    //{
    //   //Check abort conditions
    //   if (!isActive && isAlive || !turnManager.isStartPhase)
    //       return;


    //   //Source: https://forum.unity.com/threads/how-to-make-an-object-move-up-and-down-on-a-loop.380159/
    //   var pos = new Vector3(
    //       transform.boardPosition.x,
    //       transform.boardPosition.y + (glowCurve.Evaluate(Time.time % glowCurve.length) * (tileSize / 64)),
    //       transform.boardPosition.z);

    //   var rot = new Vector3(
    //      transform.angularRotation.x,
    //      transform.angularRotation.y,
    //      transform.angularRotation.z + (glowCurve.Evaluate(Time.time % glowCurve.length) * (tileSize / 128)));

    //   render.idle.transform.Rotate(Vector3.up * glowCurve.Evaluate(Time.time % glowCurve.length) * (tileSize / 3));

    //   render.glow.transform.boardPosition = pos;
    //   render.idle.transform.boardPosition = pos;
    //   render.frame.transform.boardPosition = pos;
    //   render.idle.transform.boardPosition = pos;
    //   render.idle.transform.angularRotation = rot;
    //}


    private void OnDestroy()
    {
        OnOverlapDetected -= move.HandleOnOverlapDetected;
    }
}
