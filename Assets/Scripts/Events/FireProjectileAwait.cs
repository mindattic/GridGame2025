using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Events
{
    public class FireProjectileAwait : AwaitEvent
    {
        //Quick Reference Properties
        protected ProjectileManager projectileManager => GameManager.instance.projectileManager;
        private ProjectileSettings projectile;


        public FireProjectileAwait(ProjectileSettings projectile)
        {
            this.projectile = projectile;
        }

        public override IEnumerator Execute()
        {
            yield return projectileManager.Spawn(projectile);
        }
    }
}
