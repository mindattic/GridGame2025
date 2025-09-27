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
        public static IEnumerator SingleAttackRoutine(AttackResult attackResult)
        {
            var opp = attackResult?.Opponent;
            if (opp == null || !opp.IsPlaying)           // guard: opponent might have died/deactivated earlier this frame
                yield break;

            // New: handle clean Miss with dodge feedback
            if (attackResult.HitType == HitOutcome.Miss)
            {
                yield return opp.AttackMissRoutine();
            }
            else
            {
                opp.Damage(attackResult);
            }

            // Preserve original yield
            yield return Wait.None();
        }

        /// <summary>
        /// Runs multiple attacks in sequence. Each attack finishes before the next starts,
        /// with a brief delay in between.
        /// </summary>
        public static IEnumerator MultiAttackRoutine(List<AttackResult> attackResults)
        {
            if (attackResults == null || attackResults.Count == 0)
                yield break;

            foreach (var attackResult in attackResults)
            {
                yield return SingleAttackRoutine(attackResult);
                yield return Wait.For(Interval.TenthSecond); // Short delay to produce domino effect
            }
        }
    }
}
