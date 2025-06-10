using Assets.Scripts.Models;
using System.Collections;

namespace Assets.Scripts.Actions
{
    public class FireProjectileAction : PhaseAction
    {
       //Quick Reference Properties
        protected ProjectileManager projectileManager => GameManager.instance.projectileManager;
        private ProjectileSettings projectile;


        public FireProjectileAction(ProjectileSettings projectile)
        {
            this.projectile = projectile;
        }

        public override IEnumerator Execute()
        {
            yield return projectileManager.Spawn(projectile);
        }
    }
}
