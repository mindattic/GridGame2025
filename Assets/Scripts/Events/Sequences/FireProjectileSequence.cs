using Assets.Scripts.Models;
using System.Collections;
using game = GameManagerHelper;

namespace Assets.Scripts.Events
{
    public class FireProjectileSequence : SequenceEvent
    {
        #region Game Properies
        protected ProjectileManager projectileManager => GameManager.instance.projectileManager;
        #endregion

        //Fields
        private ProjectileSettings projectile;


        //Constructor
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
