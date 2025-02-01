using Assets.Scripts.Behaviors.Actor;
using System;
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
        protected float percent33 => Constants.percent33;
        protected Vector3 tileScale => GameManager.instance.tileScale;

        protected ActorInstance focusedActor => GameManager.instance.focusedActor;

        protected List<ActorInstance> actors => GameManager.instance.actors;
        protected AudioManager audioManager => GameManager.instance.audioManager;
        protected BoardInstance board => GameManager.instance.board;
        protected float moveSpeed => GameManager.instance.moveSpeed;
        protected float snapDistance => GameManager.instance.snapDistance;
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
        private Vector3 position { get => instance.position; set => instance.position = value; }
        private Vector3 scale { get => instance.scale; set => instance.scale = value; }
        protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
        protected bool hasSelectedPlayer => GameManager.instance.hasSelectedPlayer;
        protected bool isSelectedPlayer => hasSelectedPlayer && selectedPlayer == instance;
        protected UnityEvent<Vector2Int> onSelectedPlayerLocationChanged => GameManager.instance.onSelectedPlayerLocationChanged;

        private ActorInstance instance;

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
            instance.sortingOrder = SortingOrder.Max;
            Vector3 prevPosition = instance.position; //Store the initial position
            float tiltFactor = 25f;   //How much tilt to apply based on movement
            float rotationSpeed = 10f; //Speed at which the tilt adjusts
            float resetSpeed = 5f;     //Speed at which the rotation resets

            //During: while the actor is focused or selected and not swapping.
            while ((instance.isFocusedPlayer || instance.isSelectedPlayer) && !flags.IsSwapping)
            {
                flags.IsMoving = true;
                instance.sortingOrder = SortingOrder.Max;

                //Calculate cursor position and clamp within board bounds.
                Vector3 cursorPosition = mousePosition3D + mouseOffset;
                cursorPosition.x = Mathf.Clamp(cursorPosition.x, board.bounds.Left, board.bounds.Right);
                cursorPosition.y = Mathf.Clamp(cursorPosition.y, board.bounds.Bottom, board.bounds.Top);

                //Snap the actor to the cursor.
                instance.position = cursorPosition;

                //Calculate velocity and apply tilt effect.
                Vector3 velocity = instance.position - prevPosition;
                ApplyTilt(velocity, tiltFactor, rotationSpeed, resetSpeed, Vector3.zero);

                //Update the previous position.
                prevPosition = instance.position;

                //Update the actor's grid location.
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
            audioManager.Play("Slide");
            instance.sortingOrder = SortingOrder.Moving;
            float moveSpeed = 7f;
            float snapThreshold = 0.1f;

            //Determine the destination based on the actor's grid location.
            Vector3 destination = Geometry.GetPositionByLocation(location);

            //--- Horizontal Movement ---
            if (Mathf.Abs(position.x - destination.x) > snapThreshold)
            {
                //Target position for horizontal movement.
                Vector3 horizontalTarget = new Vector3(destination.x, position.y, position.z);
                while (Mathf.Abs(position.x - destination.x) > snapThreshold)
                {
                    flags.IsMoving = true;
                    sortingOrder = SortingOrder.Moving;
                    position = Vector3.MoveTowards(position, horizontalTarget, moveSpeed * Time.deltaTime);

                    if (flags.IsSwapping)
                    {
                        //Apply tilt effect along the horizontal axis.
                        Vector3 velocity = horizontalTarget - position;
                        ApplyTilt(velocity, 25f, 10f, 5f, Vector3.zero);
                    }

                    CheckLocationChanged();
                    yield return Wait.UntilNextFrame();
                }

                //Snap X coordinate into place.
                Vector3 tempPos = position;
                tempPos.x = destination.x;
                position = tempPos;
            }

            //--- Vertical Movement ---
            if (Mathf.Abs(position.y - destination.y) > snapThreshold)
            {
                //Target position for vertical movement.
                Vector3 verticalTarget = new Vector3(position.x, destination.y, position.z);
                while (Mathf.Abs(position.y - destination.y) > snapThreshold)
                {
                    flags.IsMoving = true;
                    sortingOrder = SortingOrder.Moving;
                    position = Vector3.MoveTowards(position, verticalTarget, moveSpeed * Time.deltaTime);

                    if (flags.IsSwapping)
                    {
                        //Apply tilt effect along the vertical axis.
                        Vector3 velocity = verticalTarget - position;
                        ApplyTilt(velocity, 25f, 10f, 5f, Vector3.zero);
                    }

                    CheckLocationChanged();
                    yield return Wait.UntilNextFrame();
                }

                //Snap Y coordinate into place.
                Vector3 tempPos = position;
                tempPos.y = destination.y;
                position = tempPos;
            }

            //After: finished moving.
            flags.IsMoving = false;
            flags.IsSwapping = false;
            scale = tileScale;
            rotation = Geometry.Rotation(0, 0, 0);
        }

        ///<summary>
        ///Checks if the actor's current position has moved to a new grid tile.
        ///If so, it either updates the logical location or, if the target tile is already occupied,
        ///triggers an overlap event so that a swap can occur.
        ///</summary>
        private void CheckLocationChanged()
        {
            //Do not update the grid location if a swap is in progress.
            if (flags.IsSwapping)
                return;

            var closestTile = Geometry.GetClosestTile(position);
            Vector2Int closestLocation = closestTile.location;

            if (location != closestLocation)
            {
                //Check if any other active and alive actor (except this one) already occupies the tile.
                ActorInstance overlappingActor = actors.FirstOrDefault(x =>
                    x != null &&
                    x != instance &&
                    x.isActive &&
                    x.isAlive &&
                    x.location == closestLocation);

                if (overlappingActor != null)
                {
                    //If the overlapping actor is not already swapping, trigger its overlap event.
                    if (!overlappingActor.flags.IsSwapping)
                    {
                        overlappingActor.OnOverlapDetected.Invoke(instance);
                    }
                    //Do not update location until the swap resolves.
                    return;
                }

                //Otherwise, update the grid location.
                previousLocation = location;
                location = closestLocation;
            }
        }

        ///<summary>
        ///Called when an overlap with another actor is detected.
        ///Initiates a swap movement if not already in progress.
        ///</summary>
        public void HandleOnOverlapDetected(ActorInstance other)
        {
            //If already swapping, ignore additional overlap events.
            if (flags.IsSwapping)
                return;

            //Snap to the overlapping actor's location and start swapping.
            location = other.location;
            flags.IsSwapping = true;
            instance.StartCoroutine(MoveTowardDestination());
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
