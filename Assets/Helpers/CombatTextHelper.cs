using Assets.Scripts.Models;

namespace Assets.Helpers
{
    public static class CombatTextHelper
    {
        /// <summary>
        /// Returns the appropriate combat text style key based on hit type.
        /// </summary>
        public static string GetStyle(AttackResult attackResult)
        {
            if (attackResult.HitType == HitType.CriticalHit)
                return "CriticalHit"; // big, yellow

            if (attackResult.HitType == HitType.GlancingBlow)
                return "GlancingBlow"; // small, gray

            return "Damage"; // normal damage style
        }
    }

}
