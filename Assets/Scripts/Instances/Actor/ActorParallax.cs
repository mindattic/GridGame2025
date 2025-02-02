using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Instances.Actor
{
    public class ActorParallax
    {
        //External properies
        protected float tileSize => GameManager.instance.tileSize;

        //Fields
        private ActorInstance instance;
        public float maxSpeed = 0f;
        public float targetX = 0f;
        public float targetY = 0f;
        public Direction attackerDirection = Direction.None;
        public float transitionDuration = 2f;

        public void Initialize(ActorInstance parentInstance)
        {
            this.instance = parentInstance;

            maxSpeed = tileSize * 10f;
        }

        public void Assign(Direction attackerDirection)
        {       
            this.attackerDirection = attackerDirection;


            switch (attackerDirection)
            {
                case Direction.North:
                    targetX = 0f;
                    targetY = maxSpeed;
                    break;
                case Direction.East:
                    targetX = maxSpeed;
                    targetY = 0f;
                    break;
                case Direction.South:
                    targetX = 0f;
                    targetY = -maxSpeed;
                    break;
                case Direction.West:
                    targetX = -maxSpeed;
                    targetY = 0f;
                    break;
                case Direction.None:
                    targetX = 1f;
                    targetY = 1f;
                    break;
            }

            instance.render.parallax.material.SetFloat("_XScroll", targetX);
            instance.render.parallax.material.SetFloat("_YScroll", targetY);
        }



    }

}
