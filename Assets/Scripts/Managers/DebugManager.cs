using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Events;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

    /// <summary>
    /// Lays out a single horizontal pincer lane for quick debugging.
    /// Spawns six slimes, destroys all other enemies, teleports up to two heroes and the six slimes
    /// to fixed positions, and moves all other playing actors to random unoccupied tiles.
    /// By keeping the newly spawned slimes alive while removing other enemies,
    /// the wave does not advance and the stage does not restart.
    /// </summary>
    public void ArrangeSingleCombo()
    {
        // Spawn six slimes for this debug layout and keep references
        var keptSlimes = new List<ActorInstance>(6);
        for (int i = 0; i < 6; i++)
            keptSlimes.Add(SpawnSlime());

        // Destroy all existing enemies except the six slimes we just spawned
        foreach (var enemy in g.Actors.Enemies.ToArray())
        {
            if (enemy == null) continue;
            if (keptSlimes.Contains(enemy)) continue;

            UnityEngine.Object.Destroy(enemy.gameObject);
        }

        // Horizontal lane positions
        hero1?.Teleport(new Vector2Int(3, 1));
        keptSlimes[0]?.Teleport(new Vector2Int(3, 2));
        keptSlimes[1]?.Teleport(new Vector2Int(3, 3));
        keptSlimes[2]?.Teleport(new Vector2Int(3, 4));
        keptSlimes[3]?.Teleport(new Vector2Int(3, 5));
        keptSlimes[4]?.Teleport(new Vector2Int(3, 6));
        keptSlimes[5]?.Teleport(new Vector2Int(3, 7));
        hero2?.Teleport(new Vector2Int(3, 8));

        // Build alignment group
        var group = new List<ActorInstance> { hero1, hero2 };
        group.AddRange(keptSlimes.Where(s => s != null));

        // Move every other playing actor to an unoccupied location
        foreach (var actor in g.Actors.All)
        {
            if (actor == null) continue;
            if (!actor.isPlaying) continue;
            if (group.Contains(actor)) continue;

            actor.Teleport(RNG.UnoccupiedLocation);
        }
    }


    public void ArrangeDoubleCombo()
    {
        // Spawn either slimes used by this debug layout
        for (int i = 0; i < 8; i++)
            SpawnSlime();

        // Collect up to 9 enemies, some may be missing
        var enemies = g.Actors.Enemies.Take(8).ToArray();

        // Utility to teleport only when the actor exists
        void SafeTeleport(ActorInstance a, Vector2Int pos)
        {
            if (a != null) a.Teleport(pos);
        }

        // Heroes may be assigned in SpawnSlime; guard in case any are missing
        SafeTeleport(hero1, new Vector2Int(1, 1));
        SafeTeleport(enemies[0], new Vector2Int(1, 2));
        SafeTeleport(enemies[1], new Vector2Int(1, 3));
        SafeTeleport(enemies[2], new Vector2Int(1, 4));
        SafeTeleport(enemies[3], new Vector2Int(1, 5));
        SafeTeleport(hero2, new Vector2Int(1, 6));
        SafeTeleport(enemies[4], new Vector2Int(2, 6));
        SafeTeleport(enemies[5], new Vector2Int(3, 6));
        SafeTeleport(enemies[6], new Vector2Int(4, 6));
        SafeTeleport(enemies[7], new Vector2Int(5, 6));

        // Build the alignment group without nulls
        var group = new List<ActorInstance> { hero1, hero2, hero3, hero4 };
        group.AddRange(enemies.Where(e => e != null));
        group = group.Where(x => x != null).ToList();

        // Move every other playing actor to an unoccupied location
        foreach (var actor in g.Actors.All)
        {
            if (actor == null) continue;
            if (!actor.isPlaying) continue;
            if (group.Contains(actor)) continue;

            actor.Teleport(RNG.UnoccupiedLocation);
        }
    }

    public void ArrangeTripleCombo()
    {
        // Spawn nine slimes used by this debug layout
        for (int i = 0; i < 9; i++)
            SpawnSlime();

        // Collect up to 9 enemies, some may be missing
        var enemies = g.Actors.Enemies.Take(9).ToArray();

        // Utility to teleport only when the actor exists
        void SafeTeleport(ActorInstance a, Vector2Int pos)
        {
            if (a != null) a.Teleport(pos);
        }

        // Heroes may be assigned in SpawnSlime; guard in case any are missing
        SafeTeleport(hero1, new Vector2Int(1, 1));
        SafeTeleport(enemies[0], new Vector2Int(1, 2));
        SafeTeleport(enemies[1], new Vector2Int(1, 3));
        SafeTeleport(hero2, new Vector2Int(1, 4));
        SafeTeleport(enemies[2], new Vector2Int(2, 4));
        SafeTeleport(enemies[3], new Vector2Int(3, 4));
        SafeTeleport(enemies[4], new Vector2Int(4, 4));
        SafeTeleport(enemies[5], new Vector2Int(5, 4));
        SafeTeleport(hero3, new Vector2Int(6, 4));
        SafeTeleport(enemies[6], new Vector2Int(6, 5));
        SafeTeleport(enemies[7], new Vector2Int(6, 6));
        SafeTeleport(enemies[8], new Vector2Int(6, 7));
        SafeTeleport(hero4, new Vector2Int(6, 8));

        // Build the alignment group without nulls
        var group = new List<ActorInstance> { hero1, hero2, hero3, hero4 };
        group.AddRange(enemies.Where(e => e != null));
        group = group.Where(x => x != null).ToList();

        // Move every other playing actor to an unoccupied location
        foreach (var actor in g.Actors.All)
        {
            if (actor == null) continue;
            if (!actor.isPlaying) continue;
            if (group.Contains(actor)) continue;

            actor.Teleport(RNG.UnoccupiedLocation);
        }
    }


    /// <summary>
    /// Arranges a surround combo for debug testing.
    /// Spawns a slime in the center and positions up to four heroes
    /// around it (above, right, below, left).
    /// </summary>
    public void ArrangeSurroundCombo()
    {
        var center = new Vector2Int(3, 3);
        var above = new Vector2Int(3, 2);
        var right = new Vector2Int(4, 3);
        var below = new Vector2Int(3, 4);
        var left = new Vector2Int(2, 3);

        // Ensure at least one slime exists
        SpawnSlime();

        var slime = g.Actors.Enemies.FirstOrDefault(x => x != null && x.characterName == CharacterHelper.Slime);
        if (slime == null)
        {
            Debug.LogError("ArrangeSurroundCombo: No slime found to place in center.");
            return;
        }

        // Safe teleport helper
        void SafeTeleport(ActorInstance actor, Vector2Int pos)
        {
            if (actor != null) actor.Teleport(pos);
        }

        // Place slime and heroes
        SafeTeleport(slime, center);
        SafeTeleport(hero1, above);
        SafeTeleport(hero2, right);
        SafeTeleport(hero3, below);
        SafeTeleport(hero4, left);
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
        hero.Animation.Bump(enemy);
    }

    public void Dodge()
    {
        hero1.Animation.Dodge();
    }

    public void KillEnemies()
    {
        var playingEnemies = g.Actors.Enemies.Where(x => x.isPlaying).ToList();
        foreach (var enemy in playingEnemies)
        {
            var attackResult = new AttackResult(RNG.Hero, enemy, 9999, HitOutcome.Critical);
            enemy.Damage(attackResult);
        }
        StartCoroutine(DeathHelper.ProcessRoutine());
        //DeathHelper.Process(GameManager.instance);
    }

    public void Portrait2DSlideIn()
    {
        var hero = RNG.Hero;
        var direction = RNG.AdjacentDirection;
        g.Portrait2DManager.SlideIn(hero, direction);
    }

    public void Portrait3DSlideIn()
    {
        var hero = RNG.Hero;
        var direction = RNG.AdjacentDirection;
        g.Portrait3DManager.SlideIn(hero, direction);
    }

    public void PortraitPopIn()
    {
        var hero = RNG.Hero;
        g.SequenceManager.Add(new PortraitPopInSequence(hero));
        g.SequenceManager.Add(new PortraitPopOutSequence(hero));
        StartCoroutine(g.SequenceManager.ExecuteRoutine());
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
        hero1.Animation.Shake(intensity, duration);
    }

    public void Spin()
    {
        hero1.Animation.Spin360();
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
    }

    public void SpawnSynergyLines()
    {
        foreach (var attacker in g.Actors.Heroes)
        {
            var supporters = g.PincerAttackManager.FindSupporters(attacker);
            foreach (var supporter in supporters)
            {
                g.SynergyLineManager.Spawn(supporter, attacker);
            }
        }
    }

    public void SpawnTooltip1()
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

    public void SpawnTooltip2()
    {
        var tt = new TooltipSettings()
        {
            message = "Tap here to confirm",
            target = hero1.transform,
            placement = TooltipPlacement.Top,
            useFade = false,
            useTypewriter = false,
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

    }

    public void TutorialTest()
    {
        var tutorial = TutorialLibrary.Tutorials["Tutorial1"];
        g.TutorialPopup.Load(tutorial);
    }

    //TODO: Should be controlled by CoinManager
    public void SpawnCoins()
    {
        var vfx = VfxLibrary.VisualEffects["YellowHit"];


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

    public ActorInstance SpawnSlime()
    {
        return g.StageManager.AddEnemy(CharacterHelper.Slime);
    }

    public ActorInstance SpawnBat()
    {
        return g.StageManager.AddEnemy(CharacterHelper.Bat);
    }

    public ActorInstance SpawnScorpion()
    {
        return g.StageManager.AddEnemy(CharacterHelper.Scorpion);
    }

    public ActorInstance SpawnYeti()
    {
        return g.StageManager.AddEnemy(CharacterHelper.Yeti);
    }

    public ActorInstance SpawnSoldier()
    {
        return SpawnRandomByGroup(ActorGroup.Soldier | ActorGroup.Soldier);
    }


    public void SpawnRandomEnemy()
    {
        var r = RNG.Int(1, 10);
        if (r <= 7) SpawnSlime();
        else if (r == 8) SpawnBat();
        else if (r == 9) SpawnScorpion();
        else if (r == 10) SpawnYeti();
    }

    /// <summary>
    /// Spawns a random enemy whose ActorData matches all requested groups.
    /// Example: SpawnRandomByGroup(ActorGroup.Soldier | ActorGroup.Elite)
    /// </summary>
    public ActorInstance SpawnRandomByGroup(ActorGroup requiredGroups)
    {
        var actorData = ActorLibrary.Actors
            .Where(x => x.Value.InGroups(requiredGroups)).ToList()
            .Shuffle().FirstOrDefault().Value;

        if (actorData == null) return null;

        return g.StageManager.AddEnemy(actorData.Character);
    }


    public void Fireball()
    {
        var startPosition = hero1.position;
        var target = hero2;
   
        // Use ProjectileManager helper which sets MotionStyle and pacing
        g.ProjectileManager.EnqueueFireball(startPosition, target);
        g.SequenceManager.Execute();
    }

    public void Heal()
    {
        var source = hero1.position;
        var target = hero2;

        // Use ProjectileManager helper which sets MotionStyle and pacing
        g.ProjectileManager.EnqueueHeal(source, target);
        g.SequenceManager.Execute();
    }

    public void HomingSpiral()
    {
        var source = hero1.position;
        var target = hero2;

        // Use ProjectileManager helper which sets MotionStyle and pacing
        g.ProjectileManager.EnqueueHomingSpiral(source, target);
        g.SequenceManager.Execute();
    }


    public void RandomizeBackground()
    {
        g.Background.Randomize();
    }

    public void VFXTest_BlueSlash1()
    {
        var attackResult = new AttackResult(hero1, g.Actors.Enemies.First(), 3, HitOutcome.Normal);
        if (attackResult.HitType == HitOutcome.Critical)
        {
            var crit = VfxLibrary.VisualEffects["YellowHit"];
            g.VfxManager.Spawn(crit, hero1.position);
            attackResult.Damage = (int)Math.Round(attackResult.Damage * 1.5f);
        }

        var vfx = VfxLibrary.VisualEffects["BlueSlash1"];
        g.VfxManager.Spawn(vfx, hero1.position, hero1.DamageRoutine(attackResult));
    }

    public void VFXTest_BlueSlash2()
    {
        var vfx = VfxLibrary.VisualEffects["BlueSlash2"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueSlash3()
    {
        var vfx = VfxLibrary.VisualEffects["BlueSlash3"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueSword()
    {
        var vfx = VfxLibrary.VisualEffects["BlueSword"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueSword4X()
    {
        var vfx = VfxLibrary.VisualEffects["BlueSword4X"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BloodClaw()
    {
        var vfx = VfxLibrary.VisualEffects["BloodClaw"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_LevelUp()
    {
        var vfx = VfxLibrary.VisualEffects["LevelUp"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_YellowHit()
    {
        var vfx = VfxLibrary.VisualEffects["YellowHit"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_DoubleClaw()
    {
        var vfx = VfxLibrary.VisualEffects["DoubleClaw"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_LightningExplosion()
    {
        var vfx = VfxLibrary.VisualEffects["LightningExplosion"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BuffLife()
    {
        var vfx = VfxLibrary.VisualEffects["BuffLife"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_RotaryKnife()
    {
        var vfx = VfxLibrary.VisualEffects["RotaryKnife"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_AirSlash()
    {
        var vfx = VfxLibrary.VisualEffects["AirSlash"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_FireRain()
    {
        var vfx = VfxLibrary.VisualEffects["FireRain"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_RayBlast()
    {
        var vfx = VfxLibrary.VisualEffects["RayBlast"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_LightningStrike()
    {
        var vfx = VfxLibrary.VisualEffects["LightningStrike"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_PuffyExplosion()
    {
        var vfx = VfxLibrary.VisualEffects["PuffyExplosion"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_RedSlash2X()
    {
        var vfx = VfxLibrary.VisualEffects["RedSlash2X"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_GodRays()
    {
        var vfx = VfxLibrary.VisualEffects["GodRays"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_AcidSplash()
    {
        var vfx = VfxLibrary.VisualEffects["AcidSplash"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }
    public void VFXTest_GreenBuff()
    {
        var vfx = VfxLibrary.VisualEffects["GreenBuff"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_GoldBuff()
    {
        var vfx = VfxLibrary.VisualEffects["GoldBuff"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_HexShield()
    {
        var vfx = VfxLibrary.VisualEffects["HexShield"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_ToxicCloud()
    {
        var vfx = VfxLibrary.VisualEffects["ToxicCloud"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_OrangeSlash()
    {
        var vfx = VfxLibrary.VisualEffects["OrangeSlash"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_MoonFeather()
    {
        var vfx = VfxLibrary.VisualEffects["MoonFeather"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_PinkSpark()
    {
        var vfx = VfxLibrary.VisualEffects["PinkSpark"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueYellowSword()
    {
        var vfx = VfxLibrary.VisualEffects["BlueYellowSword"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_BlueYellowSword3X()
    {
        var vfx = VfxLibrary.VisualEffects["BlueYellowSword3X"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }

    public void VFXTest_RedSword()
    {
        var vfx = VfxLibrary.VisualEffects["RedSword"];
        g.VfxManager.Spawn(vfx, hero1.position);
        g.VfxManager.Spawn(vfx, hero2.position);
    }



}
