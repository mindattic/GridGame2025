using Assets.Helper;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Helpers
{

    public static class AttackHelper
    {
        /// <summary>
        /// Applies damage for a single attack result, then yields once.
        /// </summary>
        public static IEnumerator SingleAttackTrigger(AttackResult attackResult)
        {
            if (attackResult == null 
                || attackResult.Opponent == null 
                || attackResult.Opponent.isDying 
                || attackResult.Opponent.isDead)
                yield break;


            attackResult.Opponent.Damage(attackResult);

            // Preserve original yield
            yield return Wait.None();
        }

        /// <summary>
        /// Runs multiple attacks in sequence. Each attack finishes before the next starts,
        /// with a brief delay in between.
        /// </summary>
        public static IEnumerator MultiAttackTrigger(ActorInstance attacker, List<AttackResult> attackResults)
        {
            if (attackResults == null || attackResults.Count == 0)
                yield break;

            foreach (var attackResult in attackResults)
            {
                yield return SingleAttackTrigger(attackResult);
                yield return Wait.For(Interval.TenthSecond);
            }
        }
    }

}
