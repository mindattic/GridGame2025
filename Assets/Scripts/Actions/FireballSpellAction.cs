using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class FireballSpellAction : TurnAction
    {
        protected SpellManager spellManager => GameManager.instance.spellManager;

        private ActorInstance caster;
        private ActorInstance target;

        public FireballSpellAction(ActorInstance caster, ActorInstance target)
        {
            this.caster = caster;
            this.target = target;
        }

        public override IEnumerator Execute()
        {
            bool completed = false;
            spellManager.Spawn(caster, target, MagicBallType.Fireball, () =>
            {
                // When the spell reaches the enemy, apply flame damage.
                target.FireDamage(10);
                completed = true;
            });
            yield return new WaitUntil(() => completed);
        }
    }
}
