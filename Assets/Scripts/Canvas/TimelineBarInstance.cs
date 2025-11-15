using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Canvas
{
 [DisallowMultipleComponent]
 public sealed class TimelineBarInstance : MonoBehaviour
 {
 [Header("Parts")]
 [SerializeField] private RectTransform barRect; // horizontal line rect (width drives duration)
 [SerializeField] private RectTransform spawnRect; // right spawn point inside bar
 [SerializeField] private RectTransform leftLine; // left line target (x position)
 [SerializeField] private TimelineTag tagPrefab;

 [Header("Tuning")]
 [Tooltip("Pixels per second baseline for a tag with Speed=1.")]
 [SerializeField] private float basePxPerSec =50f;
 [Tooltip("Additional px/sec added per Speed point.")]
 [SerializeField] private float perSpeedPxPerSec =20f;
 [Tooltip("Vertical spacing between multiple tags on the same x.")]
 [SerializeField] private float tagRowHeight =14f;

 private readonly List<TimelineTag> active = new List<TimelineTag>();
 private bool advancing;

 private void Awake()
 {
 if (barRect == null) barRect = GetComponent<RectTransform>();
 }

 private float LeftEdgeX => leftLine != null ? leftLine.anchoredPosition.x :0f;
 private float SpawnX => spawnRect != null ? spawnRect.anchoredPosition.x : (barRect != null ? barRect.rect.width :300f);

 private float PxPerSecFromSpeed(int speed) => Mathf.Max(1f, basePxPerSec + perSpeedPxPerSec * Mathf.Max(0, speed));

 private IEnumerable<ActorInstance> SortedEnemiesBySpeedDesc()
 {
 return g.Actors.Enemies.Where(e => e != null && e.IsPlaying).OrderByDescending(e => e.Stats.Speed.ToInt());
 }

 public void Clear()
 {
 for (int i = active.Count -1; i >=0; i--) if (active[i] != null) Destroy(active[i].gameObject);
 active.Clear();
 }

 public void SpawnInitialForAllEnemies()
 {
 Clear();
 var ordered = SortedEnemiesBySpeedDesc().ToList();
 for (int i =0; i < ordered.Count; i++)
 {
 SpawnTag(ordered[i], initialIndex:i);
 }
 PauseAll();
 }

 private void PauseAll()
 {
 foreach (var t in active) t?.Pause();
 advancing = false;
 }
 private void ResumeAll()
 {
 foreach (var t in active) t?.Resume();
 advancing = true;
 }

 public void OnHeroStartMove()
 {
 // called by gameplay when hero begins moving; begin sliding all tags left
 ResumeAll();
 }
 public void OnHeroStopMove()
 {
 PauseAll();
 }

 public void OnEnemyTurnStarted(ActorInstance enemy)
 {
 // Stop tag advance while enemy is acting
 PauseAll();
 }
 public void OnEnemyTurnFinished(ActorInstance enemy)
 {
 // Move this enemy's tag to the far right for its next turn
 var tag = active.FirstOrDefault(t => t != null && t.Owner == enemy);
 if (tag != null)
 {
 tag.SetX(SpawnX);
 tag.Pause();
 }
 }

 private void OnTagReachedLeft(TimelineTag tag)
 {
 if (tag == null) return;
 // End hero movement phase and queue this enemy
 g.InputManager.InputMode = InputMode.None;
 g.TurnManager.QueueEnemyAfterHero(tag.Owner);
 // Drop any moving hero and resolve pincers (reuses SelectionManager logic)
 g.SelectionManager.Drop();
 PauseAll();
 }

 private void SpawnTag(ActorInstance enemy)
 {
 SpawnTag(enemy, -1);
 }
 private void SpawnTag(ActorInstance enemy, int initialIndex)
 {
 if (enemy == null || !enemy.IsEnemy) return;
 if (tagPrefab == null)
 {
 Debug.LogError("TimelineBarInstance: tagPrefab not set.");
 return;
 }
 var tag = Instantiate(tagPrefab, barRect);
 tag.name = $"TimelineTag_{enemy.name}";
 int sameOwnerCount = active.Count(a => a != null && a.Owner == enemy);
 var tr = tag.GetComponent<RectTransform>();
 var start = tr.anchoredPosition;
 tr.anchoredPosition = new Vector2(SpawnX, start.y - sameOwnerCount * tagRowHeight);
 float speed = PxPerSecFromSpeed(enemy.Stats.Speed.ToInt());
 tag.Initialize(enemy, LeftEdgeX, SpawnX, speed, OnTagReachedLeft);
 active.Add(tag);

 // If arranging initially by speed: offset slightly to reflect order left-to-right
 if (initialIndex >=0)
 {
 float width = barRect != null ? barRect.rect.width :300f;
 float pad = Mathf.Max(0f, width *0.02f);
 float x = Mathf.Clamp(SpawnX - initialIndex * pad, LeftEdgeX, SpawnX);
 tag.SetX(x);
 }
 }
 }
}
