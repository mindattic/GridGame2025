using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class HealSpellAction : TurnAction
    {
        protected SpellManager spellManager => GameManager.instance.spellManager;

        private ActorInstance caster;
        private ActorInstance target;

        public HealSpellAction(ActorInstance caster, ActorInstance target)
        {
            this.caster = caster;
            this.target = target;
        }

        public override IEnumerator Execute()
        {
            bool completed = false;
            spellManager.Spawn(caster, target, MagicBallType.GreenSparkle, () =>
            {
                // When the spell reaches the target, heal the actor.
                target.Heal(10);
                completed = true;
            });
            yield return new WaitUntil(() => completed);
        }
    }
}
