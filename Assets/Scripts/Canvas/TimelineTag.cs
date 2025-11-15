using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Canvas
{
 [DisallowMultipleComponent]
 [RequireComponent(typeof(RectTransform))]
 public sealed class TimelineTag : MonoBehaviour
 {
 [Header("Parts")]
 [SerializeField] private Image body;
 [SerializeField] private CanvasGroup canvasGroup; // for fade-out

 [Header("Runtime")]
 public ActorInstance Owner; // enemy owning this tag
 public RectTransform Rect { get; private set; }

 private float moveSpeedPxPerSec =200f;
 private float leftEdgeX;
 private System.Action<TimelineTag> onReached;
 private bool isFading;
 private bool paused;

 private void Awake()
 {
 Rect = GetComponent<RectTransform>();
 if (canvasGroup == null)
 canvasGroup = GetComponent<CanvasGroup>();
 }

 public void Wire(Image bodyImage, CanvasGroup group)
 {
 if (bodyImage != null) body = bodyImage;
 if (group != null) canvasGroup = group;
 }

 public void Initialize(ActorInstance owner, float leftEdgeX, float startX, float moveSpeedPxPerSec, System.Action<TimelineTag> onReached)
 {
 Owner = owner;
 this.leftEdgeX = leftEdgeX;
 this.moveSpeedPxPerSec = Mathf.Max(1f, moveSpeedPxPerSec);
 this.onReached = onReached;
 var p = Rect.anchoredPosition;
 Rect.anchoredPosition = new Vector2(startX, p.y);
 if (canvasGroup != null) canvasGroup.alpha =1f;
 isFading = false;
 paused = true; // start paused; TimelineBar controls advance
 }

 public void Pause() => paused = true;
 public void Resume() => paused = false;
 public void SetAlpha(float a) { if (canvasGroup != null) canvasGroup.alpha = Mathf.Clamp01(a); }
 public void SetX(float x)
 {
 var p = Rect.anchoredPosition;
 Rect.anchoredPosition = new Vector2(x, p.y);
 }

 private void Update()
 {
 if (isFading || paused) return;
 float x = Rect.anchoredPosition.x;
 float step = moveSpeedPxPerSec * Time.deltaTime;
 x = Mathf.MoveTowards(x, leftEdgeX, step);
 Rect.anchoredPosition = new Vector2(x, Rect.anchoredPosition.y);
 if (Mathf.Approximately(x, leftEdgeX) || x <= leftEdgeX)
 {
 onReached?.Invoke(this);
 }
 }

 public void FadeAndDestroy(float duration =0.25f)
 {
 if (isFading) return;
 isFading = true;
 StartCoroutine(FadeOutAndDestroy(duration));
 }

 private IEnumerator FadeOutAndDestroy(float duration)
 {
 float t =0f;
 float start = canvasGroup != null ? canvasGroup.alpha :1f;
 while (t < duration)
 {
 t += Time.deltaTime;
 float a = Mathf.Lerp(start,0f, Mathf.Clamp01(t / duration));
 if (canvasGroup != null) canvasGroup.alpha = a;
 else if (body != null) body.color = new Color(body.color.r, body.color.g, body.color.b, a);
 yield return null;
 }
 Destroy(gameObject);
 }
 }
}
