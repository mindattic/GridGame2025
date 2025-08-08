// --- File: Assets/Scripts/Instances/Actor/ActorMovement.cs ---
using Assets.Scripts.Behaviors.Actor;
using Assets.Scripts.Models;
using Game.Behaviors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

namespace Assets.Scripts.Instances.Actor
{
    /// <summary>
    /// Handles movement and tilt effects for an ActorInstance.
    /// Adds a watchdog to prevent infinite movement loops that can stall the sequence queue.
    /// </summary>
    public class ActorMovement
    {
        // Shortcut accessors into the owning instance
        protected ActorFlags flags => instance.flags;
        protected ActorRenderers render => instance.render;
        protected ActorStats stats => instance.stats;
        private bool isActive => instance.isActive;
        private bool isAlive => instance.isAlive;
        private Quaternion rotation { get => instance.rotation; set => instance.rotation = value; }
        protected Vector2Int previousLocation { get => instance.previousLocation; set => instance.previousLocation = value; }
        private Vector2Int location { get => instance.location; set => instance.location = value; }
        protected Vector3 previousPosition { get => instance.previousPosition; set => instance.previousPosition = value; }
        private Vector3 position { get => instance.position; set => instance.position = value; }
        private Vector3 scale { get => instance.scale; set => instance.scale = value; }

        protected bool isSelectedHero => g.Actors.HasSelectedHero && g.Actors.SelectedHero == instance;

        // The owning actor instance reference set during Initialize
        private ActorInstance instance;

        // --------------------------------------------------------------------
        // Lifecycle
        // --------------------------------------------------------------------

        public void Start()
        {
            // Intentionally empty. Movement is driven by explicit calls.
        }

        public void Initialize(ActorInstance parentInstance)
        {
            this.instance = parentInstance;
        }

        // --------------------------------------------------------------------
        // Cursor-follow movement
        // --------------------------------------------------------------------

        /// <summary>
        /// Moves the actor toward the cursor while the actor is focused or selected.
        /// If a swap is initiated (via overlap), the move exits immediately.
        /// </summary>
        public IEnumerator TowardCursor()
        {
            // Before: set a high sorting order if needed, then mark moving
            flags.IsMoving = true;

            float tiltFactor = 25f;     // How much tilt to apply based on move
            float rotationFocus = 10f;  // Speed at which the tilt adjusts
            float resetFocus = 5f;      // Speed at which the rotation resets

            // During: while we are in a moving state
            while (flags.IsMoving)
            {
                previousPosition = instance.position;
                instance.position = g.TouchPosition3D + g.TouchOffset;

                ApplyTilt(instance.position - previousPosition, tiltFactor, rotationFocus, resetFocus, Vector3.zero);
                CheckLocationChanged();

                yield return Wait.UntilNextFrame();
            }

            // After: clean up
            flags.IsMoving = false;
            instance.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        // --------------------------------------------------------------------
        // Grid destination movement with watchdog
        // --------------------------------------------------------------------

        /// <summary>
        /// Moves the actor toward its grid destination using right-angle (non-diagonal) move.
        /// Includes a watchdog to prevent infinite loops if MoveFocus or SnapThreshold are misconfigured.
        /// </summary>
        public IEnumerator TowardDestination()
        {
            // Before: begin move
            flags.IsMoving = true;
            g.AudioManager.Play("Slide");

            // Compute the world position for the current logical location.
            // Note: calling code should have set desired 'location' before starting the move.
            Vector3 destination = Geometry.GetPositionByLocation(location);

            // Watchdog config: hard cap duration and iteration count
            const float MaxSeconds = 5.0f;   // Maximum time allowed for this move
            const int MaxIterations = 2000;  // Maximum frames allowed for this move
            float elapsed = 0f;
            int iterations = 0;

            // --- Horizontal Movement ---
            if (Mathf.Abs(position.x - destination.x) > g.SnapThreshold)
            {
                Vector3 horizontalTarget = new Vector3(destination.x, position.y, position.z);

                while (Mathf.Abs(position.x - destination.x) > g.SnapThreshold)
                {
                    // Move along X only
                    position = Vector3.MoveTowards(position, horizontalTarget, g.MoveFocus);

                    // Update grid state and potential overlap behaviors
                    CheckLocationChanged();

                    // Advance time and frame counts for watchdog
                    elapsed += Time.deltaTime;
                    iterations++;

                    // Watchdog: break if something is wrong
                    if (elapsed > MaxSeconds || iterations > MaxIterations)
                    {
                        Debug.LogWarning($"[ActorMovement] TowardDestination X watchdog fired. Forcing snap. Actor={instance?.name}");
                        break;
                    }

                    yield return Wait.UntilNextFrame();
                }

                // Snap X into place to guarantee loop exit
                position = new Vector3(destination.x, position.y, position.z);
            }

            // Reset per-axis watchdog counters before vertical leg
            elapsed = 0f;
            iterations = 0;

            // --- Vertical Movement ---
            if (Mathf.Abs(position.y - destination.y) > g.SnapThreshold)
            {
                Vector3 verticalTarget = new Vector3(position.x, destination.y, position.z);

                while (Mathf.Abs(position.y - destination.y) > g.SnapThreshold)
                {
                    // Move along Y only
                    position = Vector3.MoveTowards(position, verticalTarget, g.MoveFocus);

                    // Update grid state and potential overlap behaviors
                    CheckLocationChanged();

                    // Advance time and frame counts for watchdog
                    elapsed += Time.deltaTime;
                    iterations++;

                    // Watchdog: break if something is wrong
                    if (elapsed > MaxSeconds || iterations > MaxIterations)
                    {
                        Debug.LogWarning($"[ActorMovement] TowardDestination Y watchdog fired. Forcing snap. Actor={instance?.name}");
                        break;
                    }

                    yield return Wait.UntilNextFrame();
                }

                // Snap Y into place to guarantee loop exit
                position = new Vector3(position.x, destination.y, position.z);
            }

            // After: finished moving
            flags.IsMoving = false;
            flags.IsSwapping = false;
            scale = g.TileScale;
            rotation = Geometry.Rotation(0, 0, 0);
        }

        /// <summary>
        /// Force the selected hero to the nearest tile. Utility for UI.
        /// </summary>
        public void ToLocation()
        {
            flags.IsMoving = false;

            var closestTile = g.TileMap.GetTile(g.Actors.SelectedHero.location);
            g.Actors.SelectedHero.location = closestTile.location;
            g.Actors.SelectedHero.position = closestTile.position;
        }

        // --------------------------------------------------------------------
        // Grid change and overlap handling
        // --------------------------------------------------------------------

        /// <summary>
        /// Checks if the actor's position crossed into a new tile.
        /// If so, updates logical location and handles overlap rules.
        /// </summary>
        private void CheckLocationChanged()
        {
            // Ignore if the change is due to selection, not move
            if (!flags.IsMoving)
                return;

            // Ignore if currently swapping location
            if (flags.IsSwapping)
                return;

            // Determine closest tile to the current position
            var closestTile = Geometry.GetClosestTile(position);

            // If location is unchanged, nothing to do
            if (location == closestTile.location)
                return;

            // Record change for highlighting and sorting
            previousLocation = location;
            location = closestTile.location;

            if (isSelectedHero)
                g.TileManager.Hightlight(previousLocation, location);

            // Determine if another actor occupies the new location
            ActorInstance overlappingActor = g.Actors.All.FirstOrDefault(x =>
                x != instance &&
                x.isPlaying &&
                x.location == location);

            if (overlappingActor == null)
            {
                // Reorder sorting while moving
                g.SortingManager.OnActorMoving(this.instance);
            }
            else
            {
                // Signal overlap and let the other actor react
                g.SortingManager.OnActorOverlap(this.instance, overlappingActor);
                overlappingActor.move.HandleOverlap(previousLocation);
            }
        }

        /// <summary>
        /// Public entry to start TowardCursor as a coroutine.
        /// </summary>
        public void TriggerMoveTowardsCursor()
        {
            instance.StartCoroutine(TowardCursor());
        }

        /// <summary>
        /// Handles swap movement after detecting an overlap.
        /// </summary>
        public void HandleOverlap(Vector2Int targetLocation)
        {
            if (flags.IsSwapping)
                return;

            var currentTile = g.TileMap.GetTile(targetLocation);

            if (currentTile.IsOccupied)
            {
                // Cannot move into an occupied tile
                Debug.Log($"Tile {currentTile.location.x},{currentTile.location.y} is occupied.");
            }
            else
            {
                // Mark swapping, update target location, and move there
                flags.IsSwapping = true;
                location = currentTile.location;
                instance.StartCoroutine(TowardDestination());
            }
        }

        // --------------------------------------------------------------------
        // Tilt effect
        // --------------------------------------------------------------------

        /// <summary>
        /// Applies a tilt effect to the actor based on its move velocity.
        /// </summary>
        public void ApplyTilt(Vector3 velocity, float tiltFactor, float rotationFocus, float resetFocus, Vector3 baseRotation)
        {
            if (velocity.magnitude > 0.01f)
            {
                // Determine whether the move is primarily vertical or horizontal
                bool isMovingVertical = Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x);
                float velocityFactor = isMovingVertical ? velocity.y : velocity.x;

                float tiltZ = velocityFactor * tiltFactor;

                instance.transform.localRotation = Quaternion.Slerp(
                    instance.transform.localRotation,
                    Quaternion.Euler(0, 0, tiltZ),
                    Time.deltaTime * rotationFocus
                );
            }
            else
            {
                // Smoothly reset rotation when the move slows or stops
                instance.transform.localRotation = Quaternion.Slerp(
                    instance.transform.localRotation,
                    Quaternion.Euler(baseRotation),
                    Time.deltaTime * resetFocus
                );
            }
        }
    }
}
