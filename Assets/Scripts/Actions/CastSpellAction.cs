using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Actions
{
    public class CastSpellAction : TurnAction
    {
        protected SpellManager spellManager => GameManager.instance.spellManager;
        private SpellSettings spell;


        public CastSpellAction(SpellSettings spell)
        {
            this.spell = spell;
        }

        public override IEnumerator Execute()
        {
            yield return spellManager.Spawn(spell);
        }
    }
}
