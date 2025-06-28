using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class FireProjectileSequence : SequenceEvent
    {
        //Quick Reference Properties
        protected ProjectileManager projectileManager => GameManager.instance.projectileManager;
        private ProjectileSettings projectile;


        public FireProjectileSequence(ProjectileSettings projectile)
        {
            this.projectile = projectile;
        }

        public override IEnumerator Execute()
        {
            yield return projectileManager.Spawn(projectile);
        }
    }
}
