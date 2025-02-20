//using Assets.Scripts.Models;
//using System.Collections;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;

//namespace Assets.Scripts.Actions
//{
//    public class FireballSpellAction : TurnAction
//    {
//        protected SpellManager spellManager => GameManager.instance.spellManager;
//        private SpellSettings spell;
      

//        public FireballSpellAction(SpellSettings spell)
//        {
//            this.spell = spell;
//        }

//        public override IEnumerator Execute()
//        {
//            //bool completed = false;
//            //source, target, MagicBallType.Fireball, () =>
//            //{
//            //    // When the spell reaches the enemy, apply flame damage.
//            //    target.FireDamage(10);
//            //    completed = true;
//            //}

//            yield return spellManager.Spawn(spell);
//            //yield return new WaitUntil(() => completed);
//        }
//    }
//}
