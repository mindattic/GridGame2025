using System.Collections;
using UnityEngine;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine.VFX;

namespace Assets.Scripts.Events
{

    public class SingleAttackTrigger : TriggerEvent
    {
        protected VFXManager vfxManager => GameManager.instance.vfxManager;

        private readonly AttackResult attackResult;

        public SingleAttackTrigger(AttackResult attackResult)
        {
            this.attackResult = attackResult;
        }

        public override IEnumerator Run()
        {

            //var vfx = attackResult.Attacker.vfx.Attack;
            //var opponent = attackResult.Opponent;
            var damage = attackResult.Damage;
            //var trigger = new TriggerEvent(opponent.TakeDamage(damage));
            //vfxManager.SpawnAsync(vfx, opponent.position, trigger);

            attackResult.Opponent.TakeDamageAsync(damage);

            //if (attackResult.IsHit)
            //    attackResult.Opponent.TakeDamageAsync(attackResult);
            //else
            //    attackResult.Opponent.animate.DodgeAsync();

            yield return null;
            HasExecuted = true;
        }
    }
}
