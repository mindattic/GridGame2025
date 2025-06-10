using Assets.Scripts.Actions;
using Assets.Scripts.GUI;
using Assets.Scripts.Models;
using Game.Behaviors;
using Game.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class DebugManager : MonoBehaviour
{
    //Quick Reference Properties
    protected List<ActorInstance> actors => GameManager.instance.actors;
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;
    protected IEnumerable<ActorInstance> enemies => GameManager.instance.enemies;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected AttackLineManager attackLineManager => GameManager.instance.attackLineManager;
    protected CoinManager coinManager => GameManager.instance.coinManager;
    protected DamageTextManager damageTextManager => GameManager.instance.damageTextManager;
    protected PortraitManager portraitManager => GameManager.instance.portraitManager;
    protected StageManager stageManager => GameManager.instance.stageManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;
    protected TutorialPopup tutorialPopup => GameManager.instance.tutorialPopup;
    protected ProjectileManager projectileManager => GameManager.instance.projectileManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;

    //Internal properties
    ActorInstance hero1 => heroes.Skip(0).Take(1).First();
    ActorInstance hero2 => heroes.Skip(1).Take(1).First();
    ActorInstance hero3 => heroes.Skip(2).Take(1).First();
    ActorInstance hero4 => heroes.Skip(3).Take(1).First();

    //Fields
    [SerializeField] private TMP_Dropdown Dropdown;
    public bool showActorNameTag = false;
    public bool showActorFrame = false;
    public bool showTutorials = false;
    public bool isHeroInvincible = false;
    public bool isEnemyInvincible = false;
    public bool isTimerInfinite = false;
    public bool isEnemyStunned = false;


    public void PortraitTest()
    {
        var hero = Random.Hero;
        var direction = Random.Direction;
        portraitManager.TriggerSlideIn(hero, direction);
    }

    public void DamageTextTest()
    {
        var text = $"{Random.Int(1, 3)}";
        damageTextManager.Spawn(text, hero1.position);
    }

    public void DamageTextBounceTest()
    {
        var text = $"{Random.Int(1, 3)}";
        damageTextManager.Spawn(text, hero1.position, DamageTextStyle.Bounce);
    }

    public void BumpTest()
    {
        var direction = Random.Direction;
        hero1.action.TriggerBump(direction);
    }

    public void ShakeTest()
    {
        var intensity = Random.ShakeIntensityLevel();
        var duration = Random.Float(Interval.HalfSecond, Interval.TwoSeconds);
        hero1.action.TriggerShake(intensity, duration);
    }

    public void DodgeTest()
    {
        hero1.action.TriggerDodge();
    }

    public void SpinTest()
    {
        hero1.action.TriggerSpin360();
    }

    public void SupportLineTest()
    {
        var alignedPairs = new HashSet<ActorPair>();
        foreach (var actor1 in heroes)
        {
            foreach (var actor2 in heroes)
            {
                if (actor1 == null || actor2 == null || actor1.Equals(actor2) || !actor1.isActive || !actor1.isAlive || !actor2.isActive || !actor2.isAlive)
                    continue;

                if (actor1.IsSameColumn(actor2.location))
                {
                    var pair = new ActorPair(actor1, actor2, Axis.Vertical);
                    alignedPairs.Add(pair);
                }
                else if (actor1.IsSameRow(actor2.location))
                {
                    var pair = new ActorPair(actor1, actor2, Axis.Horizontal);
                    alignedPairs.Add(pair);
                }

            }
        }

        foreach (var pair in alignedPairs)
        {
            //pair.startActor.sortingOrder = SortingOrder.Supporter;
            //pair.endActor.sortingOrder = SortingOrder.Supporter;
            supportLineManager.Spawn(pair.startActor, pair.endActor);
        }

        IEnumerator _()
        {
            yield return Wait.For(Interval.ThreeSeconds);

            foreach (var supportLine in supportLineManager.supportLines.Values)
            {
                supportLine.TriggerDespawn();
            }
        }

        StartCoroutine(_());
    }

    public void AttackLineTest()
    {
        var enemy1 = enemies.Skip(0).Take(1).FirstOrDefault();
        var enemy2 = enemies.Skip(1).Take(1).FirstOrDefault();
        var enemy3 = enemies.Skip(2).Take(1).FirstOrDefault();
        var enemy4 = enemies.Skip(3).Take(1).FirstOrDefault();
        var enemy5 = enemies.Skip(4).Take(1).FirstOrDefault();
        var enemy6 = enemies.Skip(5).Take(1).FirstOrDefault();

        actors.FirstOrDefault(x => x.location == new Vector2Int(3, 1))?.Teleport(new Vector2Int(1, 1));
        actors.FirstOrDefault(x => x.location == new Vector2Int(3, 2))?.Teleport(new Vector2Int(1, 2));
        actors.FirstOrDefault(x => x.location == new Vector2Int(3, 3))?.Teleport(new Vector2Int(1, 3));
        actors.FirstOrDefault(x => x.location == new Vector2Int(3, 4))?.Teleport(new Vector2Int(1, 4));
        actors.FirstOrDefault(x => x.location == new Vector2Int(3, 5))?.Teleport(new Vector2Int(1, 5));
        actors.FirstOrDefault(x => x.location == new Vector2Int(3, 6))?.Teleport(new Vector2Int(1, 6));
        actors.FirstOrDefault(x => x.location == new Vector2Int(3, 7))?.Teleport(new Vector2Int(1, 7));
        actors.FirstOrDefault(x => x.location == new Vector2Int(3, 8))?.Teleport(new Vector2Int(1, 8));

        hero1.Teleport(new Vector2Int(3, 1));
        enemy1?.Teleport(new Vector2Int(3, 2));
        enemy2?.Teleport(new Vector2Int(3, 3));
        enemy3?.Teleport(new Vector2Int(3, 4));
        enemy4?.Teleport(new Vector2Int(3, 5));
        enemy5?.Teleport(new Vector2Int(3, 6));
        enemy6?.Teleport(new Vector2Int(3, 7));
        hero2.Teleport(new Vector2Int(3, 8));




        var alignedPairs = new HashSet<ActorPair>();
        foreach (var actor1 in heroes)
        {
            foreach (var actor2 in heroes)
            {
                if (actor1 == null || actor2 == null
                    || actor1.Equals(actor2)
                    || !actor1.isActive || !actor1.isAlive
                    || !actor2.isActive || !actor2.isAlive)
                    continue;

                if (actor1.IsSameColumn(actor2.location))
                {
                    var pair = new ActorPair(actor1, actor2, Axis.Vertical);
                    alignedPairs.Add(pair);
                }
                else if (actor1.IsSameRow(actor2.location))
                {
                    var pair = new ActorPair(actor1, actor2, Axis.Horizontal);
                    alignedPairs.Add(pair);
                }

            }
        }

        foreach (var actorPair in alignedPairs)
        {
            //actorPair.startActor.sortingOrder = SortingOrder.Attacker;
            //actorPair.endActor.sortingOrder = SortingOrder.Attacker;
            attackLineManager.Spawn(actorPair);
        }

        IEnumerator _()
        {
            yield return Wait.For(Interval.ThreeSeconds);

            foreach (var attackLine in attackLineManager.attackLines.Values)
            {
                attackLine.TriggerDespawn();
            }
        }

        StartCoroutine(_());
    }

    public void EnemyAttackTest()
    {
        var attackingEnemies = enemies.Where(x => x.isPlaying).ToList();
        attackingEnemies.ForEach(x => x.SetReady());

        if (turnManager.isHeroTurn)
            turnManager.NextTurn();

    }

    public void TitleTest()
    {
        var text = DateTime.UtcNow.Ticks.ToString();
        canvasOverlay.FadeIn();
        canvasOverlay.FadeOut();

    }

    public void TooltipTest()
    {
        var tt = new TooltipSettings()
        {
            message = "Tap here to confirm",
            target = hero1.transform,
            placement = TooltipPlacement.Top,
            useFade = true,
            useTypewriter = true,
            autoDestroy = true,
            followPointer = false,
            autoDestroyDelay = 2.5f,
        };

        Tooltip.Show(tt);
    }

    public void TutorialTest()
    {
        var tutorial = TutorialRepo.Tutorials["Tutorial1"];
        tutorialPopup.Load(tutorial);
    }

    public void VFXTest_BlueSlash1()
    {
        var attack = new AttackResult()
        {
            Opponent = hero1,
            IsHit = true,
            IsCriticalHit = Random.Int(1, 10) == 10,
            Damage = 3
        };

        if (attack.IsCriticalHit)
        {
            var crit = VisualEffectRepo.VisualEffects["YellowHit"];
            vfxManager.TriggerSpawn(crit, hero1.position);
            attack.Damage = (int)Math.Round(attack.Damage * 1.5f);
        }

        var vfx = VisualEffectRepo.VisualEffects["BlueSlash1"];
        var trigger = new Trigger(hero1.TakeDamage(attack));
        vfxManager.TriggerSpawn(vfx, hero1.position, trigger);
    }

    public void VFXTest_BlueSlash2()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueSlash2"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_BlueSlash3()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueSlash3"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_BlueSword()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueSword"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_BlueSword4X()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueSword4X"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_BloodClaw()
    {
        var vfx = VisualEffectRepo.VisualEffects["BloodClaw"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_LevelUp()
    {
        var vfx = VisualEffectRepo.VisualEffects["LevelUp"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_YellowHit()
    {
        var vfx = VisualEffectRepo.VisualEffects["YellowHit"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_DoubleClaw()
    {
        var vfx = VisualEffectRepo.VisualEffects["DoubleClaw"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_LightningExplosion()
    {
        var vfx = VisualEffectRepo.VisualEffects["LightningExplosion"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_BuffLife()
    {
        var vfx = VisualEffectRepo.VisualEffects["BuffLife"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_RotaryKnife()
    {
        var vfx = VisualEffectRepo.VisualEffects["RotaryKnife"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_AirSlash()
    {
        var vfx = VisualEffectRepo.VisualEffects["AirSlash"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_FireRain()
    {
        var vfx = VisualEffectRepo.VisualEffects["FireRain"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_RayBlast()
    {
        var vfx = VisualEffectRepo.VisualEffects["RayBlast"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_LightningStrike()
    {
        var vfx = VisualEffectRepo.VisualEffects["LightningStrike"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_PuffyExplosion()
    {
        var vfx = VisualEffectRepo.VisualEffects["PuffyExplosion"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_RedSlash2X()
    {
        var vfx = VisualEffectRepo.VisualEffects["RedSlash2X"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_GodRays()
    {
        var vfx = VisualEffectRepo.VisualEffects["GodRays"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_AcidSplash()
    {
        var vfx = VisualEffectRepo.VisualEffects["AcidSplash"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }
    public void VFXTest_GreenBuff()
    {
        var vfx = VisualEffectRepo.VisualEffects["GreenBuff"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_GoldBuff()
    {
        var vfx = VisualEffectRepo.VisualEffects["GoldBuff"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_HexShield()
    {
        var vfx = VisualEffectRepo.VisualEffects["HexShield"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_ToxicCloud()
    {
        var vfx = VisualEffectRepo.VisualEffects["ToxicCloud"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_OrangeSlash()
    {
        var vfx = VisualEffectRepo.VisualEffects["OrangeSlash"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_MoonFeather()
    {
        var vfx = VisualEffectRepo.VisualEffects["MoonFeather"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_PinkSpark()
    {
        var vfx = VisualEffectRepo.VisualEffects["PinkSpark"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_BlueYellowSword()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueYellowSword"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_BlueYellowSword3X()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueYellowSword3X"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }

    public void VFXTest_RedSword()
    {
        var vfx = VisualEffectRepo.VisualEffects["RedSword"];
        vfxManager.TriggerSpawn(vfx, hero1.position);
        vfxManager.TriggerSpawn(vfx, hero2.position);
    }


    public void AlignTest()
    {
        //Assign exactly nine slimes
        for (int i = 0; i < 9; i++)
            SpawnSlime();

        //SelectProfile specific enemies for teleportation
        var enemy1 = enemies.ElementAtOrDefault(0);
        var enemy2 = enemies.ElementAtOrDefault(1);
        var enemy3 = enemies.ElementAtOrDefault(2);
        var enemy4 = enemies.ElementAtOrDefault(3);
        var enemy5 = enemies.ElementAtOrDefault(4);
        var enemy6 = enemies.ElementAtOrDefault(5);
        var enemy7 = enemies.ElementAtOrDefault(6);
        var enemy8 = enemies.ElementAtOrDefault(7);
        var enemy9 = enemies.ElementAtOrDefault(8);

        //Define the group to remain aligned
        var group = new[] { hero1, hero2, hero3, hero4, enemy1, enemy2, enemy3, enemy4, enemy5, enemy6, enemy7, enemy8, enemy9 };

        //Teleport actors in the group to specific positions
        hero1?.Teleport(new Vector2Int(1, 1));
        enemy1?.Teleport(new Vector2Int(1, 2));
        enemy2?.Teleport(new Vector2Int(1, 3));
        hero2?.Teleport(new Vector2Int(1, 4));
        enemy3?.Teleport(new Vector2Int(2, 4));
        enemy4?.Teleport(new Vector2Int(3, 4));
        enemy5?.Teleport(new Vector2Int(4, 4));
        enemy6?.Teleport(new Vector2Int(5, 4));
        hero3?.Teleport(new Vector2Int(6, 4));
        enemy7?.Teleport(new Vector2Int(6, 5));
        enemy8?.Teleport(new Vector2Int(6, 6));
        enemy9?.Teleport(new Vector2Int(6, 7));
        hero4?.Teleport(new Vector2Int(6, 8));

        //Move all other actors to unoccupied locations
        actors.Except(group).ToList().ForEach(x => x.Teleport(Random.UnoccupiedLocation));
    }



    public void CoinTest()
    {
        var vfx = VisualEffectRepo.VisualEffects["YellowHit"];


        IEnumerator spawnTenCoins()
        {
            var i = 0;
            do
            {
                coinManager.Spawn(hero1.position);
                i++;
            } while (i < 10);

            yield return true;
        }
        var trigger = new Trigger(spawnTenCoins());

        vfxManager.TriggerSpawn(vfx, hero1.position, trigger);
    }

    public void SpawnSlime()
    {
        stageManager.AddEnemy(CharacterHelper.Slime);
    }

    public void SpawnBat()
    {
        stageManager.AddEnemy(CharacterHelper.Bat);
    }

    public void SpawnScorpion()
    {
        stageManager.AddEnemy(CharacterHelper.Scorpion);
    }

    public void SpawnYeti()
    {
        stageManager.AddEnemy(CharacterHelper.Yeti);
    }


    public void SpawnRandomEnemy()
    {
        var r = Random.Int(1, 10);
        if (r <= 7) SpawnSlime();
        else if (r == 8) SpawnBat();
        else if (r == 9) SpawnScorpion();
        else if (r == 10) SpawnYeti();
    }


    public void FireballTest()
    {
        var source = hero1;
        var target = enemies.FirstOrDefault();
        projectileManager.EnqueueFireball(source, target);
        actionManager.TriggerExecute();
    }

    public void HealTest()
    {
        var source = hero1;
        var target = hero2;

        projectileManager.EnqueueHeal(source, target);
        actionManager.TriggerExecute();
    }
}
