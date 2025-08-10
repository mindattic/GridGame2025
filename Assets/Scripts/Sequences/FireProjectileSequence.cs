using Assets.Scripts.Models;
using System.Collections;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Events
{
    public class FireProjectileSequence : SequenceEvent
    {
        //Fields
        private ProjectileSettings projectile;


        //Constructor
        public FireProjectileSequence(ProjectileSettings projectile)
        {
            this.projectile = projectile;
        }

        public override IEnumerator ProcessRoutine()
        {

            yield return g.ProjectileManager.SpawnRoutine(projectile);
        }
    }
}
