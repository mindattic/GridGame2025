using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using static Assets.Helper.Intermission.Before;
using g = Assets.Helpers.GameHelper;

public class DebugManager : MonoBehaviour
{

    //DEBUG: No gaurentee these values exist, define and use inside tests...
    ActorInstance hero1 => g.Actors.Heroes.Skip(0).Take(1).First();
    ActorInstance hero2 => g.Actors.Heroes.Skip(1).Take(1).First();
    ActorInstance hero3 => g.Actors.Heroes.Skip(2).Take(1).First();
    ActorInstance hero4 => g.Actors.Heroes.Skip(3).Take(1).First();

    ActorInstance enemy1 => g.Actors.Enemies.Skip(0).Take(1).First();
    ActorInstance enemy2 => g.Actors.Enemies.Skip(1).Take(1).First();
    ActorInstance enemy3 => g.Actors.Enemies.Skip(2).Take(1).First();
    ActorInstance enemy4 => g.Actors.Enemies.Skip(3).Take(1).First();
    ActorInstance enemy5 => g.Actors.Enemies.Skip(4).Take(1).First();
    ActorInstance enemy6 => g.Actors.Enemies.Skip(5).Take(1).First();


    //Fields
    [SerializeField] private TMP_Dropdown Dropdown;
    public bool showActorNameTag = false;
    public bool showActorFrame = false;
    public bool showTutorials = false;
    public bool isHeroInvincible = false;
    public bool isEnemyInvincible = false;
    public bool isTimerInfinite = false;
    public bool isEnemyStunned = false;

    public void ArrangeSingleCombo()
    {
        //Show exactly nine slimes
        for (int i = 0; i < 6; i++)
            SpawnSlime();

        //SelectProfile specific enemies for teleportation
        var enemy1 = g.Actors.Enemies.ElementAtOrDefault(0);
        var enemy2 = g.Actors.Enemies.ElementAtOrDefault(1);
        var enemy3 = g.Actors.Enemies.ElementAtOrDefault(2);
        var enemy4 = g.Actors.Enemies.ElementAtOrDefault(3);
        var enemy5 = g.Actors.Enemies.ElementAtOrDefault(4);
        var enemy6 = g.Actors.Enemies.ElementAtOrDefault(5);

        //Define the group to remain aligned
        var group = new[] { hero1, hero2, enemy1, enemy2, enemy3, enemy4, enemy5, enemy6 };

        //Teleport actors in the group to specific positions
        hero1.Teleport(new Vector2Int(3, 1));
        enemy1.Teleport(new Vector2Int(3, 2));
        enemy2.Teleport(new Vector2Int(3, 3));
        enemy3.Teleport(new Vector2Int(3, 4));
        enemy4.Teleport(new Vector2Int(3, 5));
        enemy5.Teleport(new Vector2Int(3, 6));
        enemy6.Teleport(new Vector2Int(3, 7));
        hero2.Teleport(new Vector2Int(3, 8));

        //Seek all other actors to unoccupied locations
        g.Actors.All.Except(group).ToList().ForEach(x => x.Teleport(RNG.UnoccupiedLocation));
    }

    public void ArrangeTripleCombo()
    {
        //Show exactly nine slimes
        for (int i = 0; i < 9; i++)
            SpawnSlime();

        //SelectProfile specific enemies for teleportation
        var enemy1 = g.Actors.Enemies.ElementAtOrDefault(0);
        var enemy2 = g.Actors.Enemies.ElementAtOrDefault(1);
        var enemy3 = g.Actors.Enemies.ElementAtOrDefault(2);
        var enemy4 = g.Actors.Enemies.ElementAtOrDefault(3);
        var enemy5 = g.Actors.Enemies.ElementAtOrDefault(4);
        var enemy6 = g.Actors.Enemies.ElementAtOrDefault(5);
        var enemy7 = g.Actors.Enemies.ElementAtOrDefault(6);
        var enemy8 = g.Actors.Enemies.ElementAtOrDefault(7);
        var enemy9 = g.Actors.Enemies.ElementAtOrDefault(8);

        //Define the group to remain aligned
        var group = new[] { hero1, hero2, hero3, hero4, enemy1, enemy2, enemy3, enemy4, enemy5, enemy6, enemy7, enemy8, enemy9 };

        //Teleport actors in the group to specific positions
        hero1.Teleport(new Vector2Int(1, 1));
        enemy1.Teleport(new Vector2Int(1, 2));
        enemy2.Teleport(new Vector2Int(1, 3));
        hero2.Teleport(new Vector2Int(1, 4));
        enemy3.Teleport(new Vector2Int(2, 4));
        enemy4.Teleport(new Vector2Int(3, 4));
        enemy5.Teleport(new Vector2Int(4, 4));
        enemy6.Teleport(new Vector2Int(5, 4));
        hero3.Teleport(new Vector2Int(6, 4));
        enemy7.Teleport(new Vector2Int(6, 5));
        enemy8.Teleport(new Vector2Int(6, 6));
        enemy9.Teleport(new Vector2Int(6, 7));
        hero4.Teleport(new Vector2Int(6, 8));

        //Seek all other actors to unoccupied locations
        g.Actors.All.Except(group).ToList().ForEach(x => x.Teleport(RNG.UnoccupiedLocation));
    }

    public void Bump()
    {
        var hero = RNG.Hero;
        hero.Teleport(RNG.UnoccupiedLocation);

        // 3) try to find an attacker already adjacent
        var enemy = Geometry.GetAdjacentOpponent(hero);
        if (!enemy.Exists())
            enemy = RNG.Enemy;

        var location = Geometry.GetClosestUnoccupiedAdjacentTileByLocation(hero.location).location;
        if (!location.Exists())
            location = Geometry.GetAdjacentLocationInDirection(hero.location, RNG.AdjacentDirection);

        enemy.Teleport(location);
        hero.action.Bump(enemy);
    }

    public void Dodge()
    {
        hero1.action.Dodge();
    }

    public void KillEnemies()
    {
        var playingEnemies = g.Actors.Enemies.Where(x => x.isPlaying).ToList();
        foreach (var enemy in playingEnemies)
        {
            var attackResult = new AttackResult(RNG.Hero, enemy, 9999, HitType.CriticalHit);
            enemy.Damage(attackResult);
        }
        StartCoroutine(DeathHelper.Process());
    }

    public void Portrait2DSlideIn()
    {
        var hero = RNG.Hero;
        var direction = RNG.AdjacentDirection;
        g.Portrait2DManager.TriggerSlideIn(hero, direction);
    }

    public void Portrait3DSlideIn()
    {
        var hero = RNG.Hero;
        var direction = RNG.AdjacentDirection;
        g.Portrait3DManager.TriggerSlideIn(hero, direction);
    }

    public void PortraitPopIn()
    {
        var hero = RNG.Hero;
        g.SequenceManager.Add(new PortraitPopInSequence(hero));
        g.SequenceManager.Add(new PortraitPopOutSequence(hero));
        StartCoroutine(g.SequenceManager.ExecuteTrigger());
    }

    public void SpawnDamageText()
    {
        var hero = RNG.Hero;
        var text = $"{RNG.Int(1, 100)}";
        g.CombatTextManager.Spawn(text, hero.position, "Damage");
    }

    public void SpawnHealText()
    {
        var hero = RNG.Hero;
        var text = $"{RNG.Int(1, 100)}";
        g.CombatTextManager.Spawn(text, hero.position, "Heal");
    }


    public void Shake()
    {
        var intensity = RNG.ShakeIntensityLevel();
        var duration = RNG.Float(Interval.HalfSecond, Interval.TwoSeconds);
        hero1.action.Shake(intensity, duration);
    }

    public void Spin()
    {
        hero1.action.Spin360();
    }

    public void SpawnSupportLines()
    {
        foreach (var attacker in g.Actors.Heroes)
        {
            var supporters = g.PincerAttackManager.FindSupporters(attacker);
            foreach (var supporter in supporters)
            {
                var newest = g.SupportLineManager.Spawn(supporter, attacker);
                newest.isStatic = true;
            }
        }


        //IEnumerator _()
        //{
        //    yield return Wait.For(Interval.ThreeSeconds);

        //    foreach (var supportLine in g.SupportLineManager.supportLines.Values)
        //    {
        //        supportLine.TriggerDespawn();
        //    }
        //}

        //StartCoroutine(_());
    }
    public void SpawnTooltip()
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

    public void TriggerEnemyMoveAttack()
    {
        var attackingEnemies = g.Actors.Enemies.Where(x => x.isPlaying).ToList();
        attackingEnemies.ForEach(x => x.SetReady());

        if (g.TurnManager.isHeroTurn)
            g.TurnManager.NextTurn();

    }

    public void TriggerEnemyAttack()
    {
        if (g.TurnManager.isHeroTurn)
            g.TurnManager.NextTurn();           // switch to attacker turn
    }


    public void TitleTest()
    {
        var text = DateTime.UtcNow.Ticks.ToString();
        g.CanvasOverlay.FadeIn();
        g.CanvasOverlay.FadeOut();

    }

    public void TutorialTest()
    {
        var tutorial = TutorialRepo.Tutorials["Tutorial1"];
        g.TutorialPopup.Load(tutorial);
    }

    public void SpawnCoints()
    {
        var vfx = VisualEffectRepo.VisualEffects["YellowHit"];


        IEnumerator spawnTenCoins()
        {
            var i = 0;
            do
            {
                g.CoinManager.Spawn(hero1.position);
                i++;
            } while (i < 10);

            yield return true;
        }
        g.VfxManager.Spawn(vfx, hero1.position, spawnTenCoins());
    }

    public void SpawnSlime()
    {
        g.StageManager.AddEnemy(CharacterHelper.Slime);
    }

    public void SpawnBat()
    {
        g.StageManager.AddEnemy(CharacterHelper.Bat);
    }

    public void SpawnScorpion()
    {
        g.StageManager.AddEnemy(CharacterHelper.Scorpion);
    }

    public void SpawnYeti()
    {
        g.StageManager.AddEnemy(CharacterHelper.Yeti);
    }
    public void SpawnRandomEnemy()
    {
        var r = RNG.Int(1, 10);
        if (r <= 7) SpawnSlime();
        else if (r == 8) SpawnBat();
        else if (r == 9) SpawnScorpion();
        else if (r == 10) SpawnYeti();
    }

    public void Fireball()
    {
        var startPosition = hero1.position;
        var target = g.Actors.Enemies.FirstOrDefault();
        g.ProjectileManager.EnqueueFireball(startPosition, target);
        g.SequenceManager.Execute();
    }

    public void Heal()
    {
        var source = hero1.position;
        var target = hero2;

        g.ProjectileManager.EnqueueHeal(source, target);
        g.SequenceManager.Execute();
    }
    public void RandomizeBackground()
    {
        g.Background.Randomize();
    }

    public void VFXTest_BlueSlash1()
    {
        var attackResult = new AttackResult(hero1, g.Actors.Enemies.First(), 3, HitType.Normal);
        if (attackResult.HitType == HitType.CriticalHit)
        {
            var crit = VisualEffectRepo.VisualEffects["YellowHit"];
            g.VfxManager.Spawn(crit, hero1.position);
            attackResult.Damage = (int)Math.Round(attackResult.Damage * 1.5f);
        }

        var vfx = VisualEffectRepo.VisualEffects["BlueSlash1"];
        g.VfxManager.Spawn(vfx, hero1.position, hero1.DamageTrigger(attackResult));
    }

    public void VFXTest_BlueSlash2()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueSlash2"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueSlash3()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueSlash3"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueSword()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueSword"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueSword4X()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueSword4X"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BloodClaw()
    {
        var vfx = VisualEffectRepo.VisualEffects["BloodClaw"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_LevelUp()
    {
        var vfx = VisualEffectRepo.VisualEffects["LevelUp"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_YellowHit()
    {
        var vfx = VisualEffectRepo.VisualEffects["YellowHit"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_DoubleClaw()
    {
        var vfx = VisualEffectRepo.VisualEffects["DoubleClaw"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_LightningExplosion()
    {
        var vfx = VisualEffectRepo.VisualEffects["LightningExplosion"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BuffLife()
    {
        var vfx = VisualEffectRepo.VisualEffects["BuffLife"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_RotaryKnife()
    {
        var vfx = VisualEffectRepo.VisualEffects["RotaryKnife"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_AirSlash()
    {
        var vfx = VisualEffectRepo.VisualEffects["AirSlash"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_FireRain()
    {
        var vfx = VisualEffectRepo.VisualEffects["FireRain"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_RayBlast()
    {
        var vfx = VisualEffectRepo.VisualEffects["RayBlast"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_LightningStrike()
    {
        var vfx = VisualEffectRepo.VisualEffects["LightningStrike"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_PuffyExplosion()
    {
        var vfx = VisualEffectRepo.VisualEffects["PuffyExplosion"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_RedSlash2X()
    {
        var vfx = VisualEffectRepo.VisualEffects["RedSlash2X"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_GodRays()
    {
        var vfx = VisualEffectRepo.VisualEffects["GodRays"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_AcidSplash()
    {
        var vfx = VisualEffectRepo.VisualEffects["AcidSplash"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }
    public void VFXTest_GreenBuff()
    {
        var vfx = VisualEffectRepo.VisualEffects["GreenBuff"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_GoldBuff()
    {
        var vfx = VisualEffectRepo.VisualEffects["GoldBuff"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_HexShield()
    {
        var vfx = VisualEffectRepo.VisualEffects["HexShield"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_ToxicCloud()
    {
        var vfx = VisualEffectRepo.VisualEffects["ToxicCloud"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_OrangeSlash()
    {
        var vfx = VisualEffectRepo.VisualEffects["OrangeSlash"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_MoonFeather()
    {
        var vfx = VisualEffectRepo.VisualEffects["MoonFeather"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_PinkSpark()
    {
        var vfx = VisualEffectRepo.VisualEffects["PinkSpark"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueYellowSword()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueYellowSword"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueYellowSword3X()
    {
        var vfx = VisualEffectRepo.VisualEffects["BlueYellowSword3X"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_RedSword()
    {
        var vfx = VisualEffectRepo.VisualEffects["RedSword"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

}
