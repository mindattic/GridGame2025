﻿using Assets.Scripts.Behaviors.Actor;
using Game.Behaviors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts.Instances.Actor
{
    public class ActorMovement
    {
        //Quick Reference Properties
        protected float percent33 => Constants.percent33;
        protected Vector3 tileScale => GameManager.instance.tileScale;
        protected ActorInstance focusedActor => GameManager.instance.focusedActor;
        protected List<ActorInstance> actors => GameManager.instance.actors;
        protected AudioManager audioManager => GameManager.instance.audioManager;
        protected BoardInstance board => GameManager.instance.board;
        protected float moveSpeed => GameManager.instance.moveSpeed;
        protected float snapThreshold => GameManager.instance.snapThreshold;
        protected float tileSize => GameManager.instance.tileSize;
        protected Vector3 mousePosition3D => GameManager.instance.mousePosition3D;
        protected Vector3 mouseOffset => GameManager.instance.mouseOffset;
        protected ActorFlags flags => instance.flags;
        protected ActorRenderers render => instance.render;
        protected ActorStats stats => instance.stats;
        private bool isActive => instance.isActive;
        private bool isAlive => instance.isAlive;
        private int sortingOrder { get => instance.sortingOrder; set => instance.sortingOrder = value; }
        private Quaternion rotation { get => instance.rotation; set => instance.rotation = value; }
        protected Vector2Int previousLocation { get => instance.previousLocation; set => instance.previousLocation = value; }
        private Vector2Int location { get => instance.location; set => instance.location = value; }
        protected Vector3 previousPosition { get => instance.previousPosition; set => instance.previousPosition = value; }
        private Vector3 position { get => instance.position; set => instance.position = value; }
        private Vector3 scale { get => instance.scale; set => instance.scale = value; }
        protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
        protected bool hasSelectedPlayer => GameManager.instance.hasSelectedPlayer;
        protected bool isSelectedPlayer => hasSelectedPlayer && selectedPlayer == instance;
        protected FocusIndicator focusIndicator => GameManager.instance.focusIndicator;
        protected Card card => GameManager.instance.card;



        protected UnityEvent<Vector2Int> onSelectedPlayerLocationChanged => GameManager.instance.onSelectedPlayerLocationChanged;


        //Fields
        private ActorInstance instance;


        public void Start()
        {
        }

        public void Initialize(ActorInstance parentInstance)
        {
            this.instance = parentInstance;
        }





        ///<summary>
        ///Moves the actor toward the cursor while the actor is focused or selected.
        ///If a swap is initiated (via overlap), the movement exits immediately.
        ///</summary>
        public IEnumerator MoveTowardCursor()
        {
            //Before: set a high sorting order.
            flags.IsMoving = true;
            instance.sortingOrder = SortingOrder.Max;
            float tiltFactor = 25f;   //How much tilt to apply based on movement
            float rotationSpeed = 10f; //Speed at which the tilt adjusts
            float resetSpeed = 5f;     //Speed at which the rotation resets

            //During: while the actor is focused or selected and not swapping.
            while (flags.IsMoving)
            {
                previousPosition = instance.position;
                instance.position = mousePosition3D + mouseOffset;
                //ApplyTilt(instance.position - previousPosition, tiltFactor, rotationSpeed, resetSpeed, Vector3.zero);
                CheckLocationChanged();
                yield return Wait.UntilNextFrame();
            }

            //After: clean up.
            flags.IsMoving = false;
            instance.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }


        ///<summary>
        ///Moves the actor toward its grid destination using right-angle (non-diagonal) movement.
        ///</summary>
        public IEnumerator MoveTowardDestination()
        {
            //Before: movement begins
            flags.IsMoving = true;
            audioManager.Play("Slide");
            instance.sortingOrder = SortingOrder.Moving;

            //Determine the destination based on the actor's grid location.
            Vector3 destination = Geometry.GetPositionByLocation(location);

            //--- Horizontal Movement ---
            if (Mathf.Abs(position.x - destination.x) > snapThreshold)
            {
                //Opponent position for horizontal movement.
                Vector3 horizontalTarget = new Vector3(destination.x, position.y, position.z);
                while (Mathf.Abs(position.x - destination.x) > snapThreshold)
                {
                    position = Vector3.MoveTowards(position, horizontalTarget, moveSpeed);

                    //if (flags.IsSwapping)
                    //{
                    //    //Apply tilt effect along the horizontal axis.
                    //    Vector3 velocity = horizontalTarget - position;
                    //    ApplyTilt(velocity, 25f, 10f, 5f, Vector3.zero);
                    //}

                    CheckLocationChanged();
                    yield return Wait.UntilNextFrame();
                }

                //Snap X coordinate into place.
                position = new Vector3(destination.x, position.y, position.z);
            }

            //--- Vertical Movement ---
            if (Mathf.Abs(position.y - destination.y) > snapThreshold)
            {
                //Opponent position for vertical movement.
                Vector3 verticalTarget = new Vector3(position.x, destination.y, position.z);
                while (Mathf.Abs(position.y - destination.y) > snapThreshold)
                {
                    position = Vector3.MoveTowards(position, verticalTarget, moveSpeed);

                    //if (flags.IsSwapping)
                    //{
                    //    //Apply tilt effect along the vertical axis.
                    //    Vector3 velocity = verticalTarget - position;
                    //    ApplyTilt(velocity, 25f, 10f, 5f, Vector3.zero);
                    //}

                    CheckLocationChanged();
                    yield return Wait.UntilNextFrame();
                }

                //Snap Y coordinate into place.
                position = new Vector3(position.x, destination.y, position.z);
            }

            //After: finished moving.
            flags.IsMoving = false;
            flags.IsSwapping = false;
            scale = tileScale;
            rotation = Geometry.Rotation(0, 0, 0);
        }


        public void SnapToLocation()
        {
            flags.IsMoving = false;
            var closestTile = GameManager.instance.tileMap.GetTile(selectedPlayer.location);
            selectedPlayer.location = closestTile.location;
            selectedPlayer.position = closestTile.position;
        }

        ///<summary>
        ///Checks if the actor's current position has moved to a new grid tile.
        ///If so, it either updates the logical location or, if the target tile is already occupied,
        ///triggers an overlap event so that a swap can occur.
        ///</summary>
        private void CheckLocationChanged()
        {
            // Ignore if the change is due to selection, not movement
            if (!flags.IsMoving)
                return;

            if (Vector3.Distance(position, instance.currentTile.position) <= tileSize / 2)
                return;

            if (flags.IsSwapping)
                return;

            var closestTile = Geometry.GetClosestTile(position);

            if (location == closestTile.location)
                return;

            //Debug.Log($"OnSelectedPlayerLocationChanged triggered for {instance.name} to {closestTile.location}, isMoving: {flags.IsMoving}");

            previousLocation = location;
            location = closestTile.location;

            instance.onSelectedPlayerLocationChanged?.Invoke(previousLocation, closestTile.location);

            ActorInstance overlappingActor = actors.FirstOrDefault(x =>
                x != instance &&
                x.isPlaying &&
                x.location == location);

            if (overlappingActor != null)
                overlappingActor.onOverlapDetected.Invoke(previousLocation);
        }


        public void OnDragDetected()
        {
            instance.StartCoroutine(MoveTowardCursor());
        }




        ///<summary>
        ///Called when an overlap with another actor is detected.
        ///Initiates a swap movement if not already in progress.
        ///</summary>
        public void OnOverlapDetected(Vector2Int newLocation)
        {
            //Ignore this event if the actors is already in the middle of swapping
            if (flags.IsSwapping)
                return;

            var currentTile = GameManager.instance.tileMap.GetTile(newLocation);
            if (currentTile.IsOccupied)
            {
                //Debug.Log($"Tile ${currentTile.location.x}x{currentTile.location.y} is occupied.");

            }
            else
            {
                flags.IsSwapping = true;
                location = currentTile.location;
                instance.StartCoroutine(MoveTowardDestination());
            }

        }


        ///<summary>
        ///Applies a tilt effect to the actor based on its movement velocity.
        ///</summary>
        public void ApplyTilt(Vector3 velocity, float tiltFactor, float rotationSpeed, float resetSpeed, Vector3 baseRotation)
        {
            if (velocity.magnitude > 0.01f) //Only apply tilt if there's noticeable movement.
            {
                //Determine whether the movement is primarily vertical or horizontal.
                bool isMovingVertical = Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x);
                float velocityFactor = isMovingVertical ? velocity.y : velocity.x;
                float tiltZ = velocityFactor * tiltFactor;
                instance.transform.localRotation = Quaternion.Slerp(
                    instance.transform.localRotation,
                    Quaternion.Euler(0, 0, tiltZ),
                    Time.deltaTime * rotationSpeed
                );
            }
            else
            {
                //Smoothly reset the rotation when the movement slows/stops.
                instance.transform.localRotation = Quaternion.Slerp(
                    instance.transform.localRotation,
                    Quaternion.Euler(baseRotation),
                    Time.deltaTime * resetSpeed
                );
            }
        }
    }
}
