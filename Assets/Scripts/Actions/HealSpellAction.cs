//using Assets.Scripts.Models;
//using System.Collections;

//namespace Assets.Scripts.Actions
//{
//    public class HealSpellAction : TurnAction
//    {
//        protected SpellManager spellManager => GameManager.instance.spellManager;

//        private SpellSettings spell;

//        public HealSpellAction(SpellSettings spell)
//        {
//            this.spell = spell;
//        }

//        public override IEnumerator Execute()
//        {
//            //bool completed = false;
//            //spellManager.Spawn(source, target, MagicBallType.GreenSparkle, () =>
//            //{
//            //    // When the spell reaches the target, heal the actor.
//            //    target.Heal(10);
//            //    completed = true;
//            //});
//            yield return spellManager.Spawn(spell);
//            //yield return new WaitUntil(() => completed);
//        }
//    }
//}
