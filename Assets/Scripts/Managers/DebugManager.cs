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
using UnityEngine;
using static Unity.VisualScripting.Member;

public class DebugManager : MonoBehaviour
{
    //External properties
    protected List<ActorInstance> actors => GameManager.instance.actors;
    protected IQueryable<ActorInstance> players => GameManager.instance.players;
    protected IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    protected ActorManager actorManager => GameManager.instance.actorManager;
    protected AttackLineManager attackLineManager => GameManager.instance.attackLineManager;
    protected CoinManager coinManager => GameManager.instance.coinManager;
    protected DamageTextManager damageTextManager => GameManager.instance.damageTextManager;
    protected DataManager dataManager => GameManager.instance.dataManager;
    protected PortraitManager portraitManager => GameManager.instance.portraitManager;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected StageManager stageManager => GameManager.instance.stageManager;
    protected SupportLineManager supportLineManager => GameManager.instance.supportLineManager;
    protected TooltipManager tooltipManager => GameManager.instance.tooltipManager;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    protected CanvasOverlay canvasOverlay => GameManager.instance.canvasOverlay;
    protected TutorialPopup tutorialPopup => GameManager.instance.tutorialPopup;
    protected SpellManager spellManager => GameManager.instance.spellManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;

    //Internal properties
    ActorInstance paladin => players.First(x => x.name.StartsWith("Paladin"));
    ActorInstance barbarian => players.First(x => x.name.StartsWith("Barbarian"));
    ActorInstance cleric => players.First(x => x.name.StartsWith("Cleric"));
    ActorInstance ninja => players.First(x => x.name.StartsWith("Ninja"));

    //Fields
    [SerializeField] private TMP_Dropdown Dropdown;
    public bool showActorNameTag = false;
    public bool showActorFrame = false;
    public bool showTutorials = false;
    public bool isPlayerInvincible = false;
    public bool isEnemyInvincible = false;
    public bool isTimerInfinite = false;
    public bool isEnemyStunned = false;


    public void PortraitTest()
    {
        var player = Random.Player;
        var direction = Random.Direction;
        portraitManager.SlideIn(player, direction);
    }

    public void DamageTextTest()
    {
        var text = $"{Random.Int(1, 3)}";
        damageTextManager.Spawn(text, paladin.position);
    }

    public void BumpTest()
    {
        var direction = Random.Direction;
        paladin.action.TriggerBump(direction);
    }

    public void ShakeTest()
    {
        var intensity = Random.ShakeIntensityLevel();
        var duration = Random.Float(Interval.HalfSecond, Interval.TwoSeconds);
        paladin.action.TriggerShake(intensity, duration);
    }

    public void DodgeTest()
    {
        paladin.action.TriggerDodge();
    }

    public void SpinTest()
    {
        paladin.action.TriggerSpin360();
    }

    public void SupportLineTest()
    {
        var alignedPairs = new HashSet<ActorPair>();
        foreach (var actor1 in players)
        {
            foreach (var actor2 in players)
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
            pair.startActor.sortingOrder = SortingOrder.Supporter;
            pair.endActor.sortingOrder = SortingOrder.Supporter;
            supportLineManager.Spawn(pair);
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

        paladin.Teleport(new Vector2Int(3, 1));
        enemy1?.Teleport(new Vector2Int(3, 2));
        enemy2?.Teleport(new Vector2Int(3, 3));
        enemy3?.Teleport(new Vector2Int(3, 4));
        enemy4?.Teleport(new Vector2Int(3, 5));
        enemy5?.Teleport(new Vector2Int(3, 6));
        enemy6?.Teleport(new Vector2Int(3, 7));
        barbarian.Teleport(new Vector2Int(3, 8));




        var alignedPairs = new HashSet<ActorPair>();
        foreach (var actor1 in players)
        {
            foreach (var actor2 in players)
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
            actorPair.startActor.sortingOrder = SortingOrder.Attacker;
            actorPair.endActor.sortingOrder = SortingOrder.Attacker;
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

        if (turnManager.isPlayerTurn)
            turnManager.NextTurn();

    }

    public void TitleTest()
    {
        var text = DateTime.UtcNow.Ticks.ToString();
        canvasOverlay.TriggerFadeIn(text);
        canvasOverlay.TriggerFadeOut(Interval.ThreeSeconds);

    }

    public void TooltipTest()
    {
        var text = $"Test {Random.Int(1000, 9999)}";
        var position = Random.Player.currentTile.position;
        tooltipManager.Spawn(text, position);
    }

    public void TutorialTest()
    {
        var tutorial = resourceManager.tutorials.FirstOrDefault().Value;
        tutorialPopup.Load(tutorial);
    }

    public void VFXTest_BlueSlash1()
    {
        var attack = new AttackResult()
        {
            Opponent = paladin,
            IsHit = true,
            IsCriticalHit = Random.Int(1, 10) == 10,
            Damage = 3
        };

        if (attack.IsCriticalHit)
        {
            var crit = resourceManager.VisualEffect("YellowHit");
            vfxManager.TriggerSpawn(crit, paladin.position);
            attack.Damage = (int)Math.Round(attack.Damage * 1.5f);
        }

        var vfx = resourceManager.VisualEffect("BlueSlash1");
        var trigger = new Trigger(paladin.TakeDamage(attack));
        vfxManager.TriggerSpawn(vfx, paladin.position, trigger);
    }

    public void VFXTest_BlueSlash2()
    {
        var vfx = resourceManager.VisualEffect("BlueSlash2");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_BlueSlash3()
    {
        var vfx = resourceManager.VisualEffect("BlueSlash3");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_BlueSword()
    {
        var vfx = resourceManager.VisualEffect("BlueSword");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_BlueSword4X()
    {
        var vfx = resourceManager.VisualEffect("BlueSword4X");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_BloodClaw()
    {
        var vfx = resourceManager.VisualEffect("BloodClaw");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_LevelUp()
    {
        var vfx = resourceManager.VisualEffect("LevelUp");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_YellowHit()
    {
        var vfx = resourceManager.VisualEffect("YellowHit");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_DoubleClaw()
    {
        var vfx = resourceManager.VisualEffect("DoubleClaw");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_LightningExplosion()
    {
        var vfx = resourceManager.VisualEffect("LightningExplosion");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_BuffLife()
    {
        var vfx = resourceManager.VisualEffect("BuffLife");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_RotaryKnife()
    {
        var vfx = resourceManager.VisualEffect("RotaryKnife");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_AirSlash()
    {
        var vfx = resourceManager.VisualEffect("AirSlash");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_FireRain()
    {
        var vfx = resourceManager.VisualEffect("FireRain");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_RayBlast()
    {
        var vfx = resourceManager.VisualEffect("RayBlast");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_LightningStrike()
    {
        var vfx = resourceManager.VisualEffect("LightningStrike");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_PuffyExplosion()
    {
        var vfx = resourceManager.VisualEffect("PuffyExplosion");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_RedSlash2X()
    {
        var vfx = resourceManager.VisualEffect("RedSlash2X");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_GodRays()
    {
        var vfx = resourceManager.VisualEffect("GodRays");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_AcidSplash()
    {
        var vfx = resourceManager.VisualEffect("AcidSplash");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }
    public void VFXTest_GreenBuff()
    {
        var vfx = resourceManager.VisualEffect("GreenBuff");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_GoldBuff()
    {
        var vfx = resourceManager.VisualEffect("GoldBuff");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_HexShield()
    {
        var vfx = resourceManager.VisualEffect("HexShield");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_ToxicCloud()
    {
        var vfx = resourceManager.VisualEffect("ToxicCloud");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_OrangeSlash()
    {
        var vfx = resourceManager.VisualEffect("OrangeSlash");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_MoonFeather()
    {
        var vfx = resourceManager.VisualEffect("MoonFeather");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_PinkSpark()
    {
        var vfx = resourceManager.VisualEffect("PinkSpark");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_BlueYellowSword()
    {
        var vfx = resourceManager.VisualEffect("BlueYellowSword");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_BlueYellowSword3X()
    {
        var vfx = resourceManager.VisualEffect("BlueYellowSword3X");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }

    public void VFXTest_RedSword()
    {
        var vfx = resourceManager.VisualEffect("RedSword");
        vfxManager.TriggerSpawn(vfx, paladin.position);
        vfxManager.TriggerSpawn(vfx, barbarian.position);
    }


    public void AlignTest()
    {
        //Spawn exactly nine slimes
        for (int i = 0; i < 9; i++)
            SpawnSlime();

        //Assign specific enemies for teleportation
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
        var group = new[] { paladin, barbarian, cleric, ninja, enemy1, enemy2, enemy3, enemy4, enemy5, enemy6, enemy7, enemy8, enemy9 };

        //Teleport actors in the group to specific positions
        paladin?.Teleport(new Vector2Int(1, 1));
        enemy1?.Teleport(new Vector2Int(1, 2));
        enemy2?.Teleport(new Vector2Int(1, 3));
        barbarian?.Teleport(new Vector2Int(1, 4));
        enemy3?.Teleport(new Vector2Int(2, 4));
        enemy4?.Teleport(new Vector2Int(3, 4));
        enemy5?.Teleport(new Vector2Int(4, 4));
        enemy6?.Teleport(new Vector2Int(5, 4));
        cleric?.Teleport(new Vector2Int(6, 4));
        enemy7?.Teleport(new Vector2Int(6, 5));
        enemy8?.Teleport(new Vector2Int(6, 6));
        enemy9?.Teleport(new Vector2Int(6, 7));
        ninja?.Teleport(new Vector2Int(6, 8));

        //Move all other actors to unoccupied locations
        actors.Except(group).ToList().ForEach(x => x.Teleport(Random.UnoccupiedLocation));
    }



    public void CoinTest()
    {
        var vfx = resourceManager.VisualEffect("YellowHit");


        IEnumerator spawnTenCoins()
        {
            var i = 0;
            do
            {
                coinManager.Spawn(paladin.position);
                i++;
            } while (i < 10);

            yield return true;
        }
        var trigger = new Trigger(spawnTenCoins());

        vfxManager.TriggerSpawn(vfx, paladin.position, trigger);
    }

    public void SpawnSlime()
    {
        stageManager.AddEnemy(Character.Slime);
    }

    public void SpawnBat()
    {
        stageManager.AddEnemy(Character.Bat);
    }

    public void SpawnScorpion()
    {
        stageManager.AddEnemy(Character.Scorpion);
    }

    public void SpawnYeti()
    {
        stageManager.AddEnemy(Character.Yeti);
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
        var source = paladin;
        var target = enemies.FirstOrDefault();
        spellManager.EnqueueFireball(source, target);
        actionManager.TriggerExecute();
    }

    public void HealTest()
    {
        var source = paladin;
        var target = barbarian;

        spellManager.EnqueueHeal(source, target);
        actionManager.TriggerExecute();
    }
}
