using System;
using System.Collections;
using UnityEngine;
using game = GameManagerHelper;

namespace Game.Instances
{
    public class AttackLineInstance : MonoBehaviour
    {
        // Quick Reference Properties
        protected float tileSize => GameManager.instance.tileSize;
        protected BoardInstance board => GameManager.instance.board;
        public Transform parent { get => gameObject.transform.parent; set => gameObject.transform.SetParent(value, true); }
        public Vector3 position { get => gameObject.transform.position; set => gameObject.transform.position = value; }
        public int sortingOrder { get => lineRenderer.sortingOrder; set => lineRenderer.sortingOrder = value; }

        // Fields
        public float alpha;

        [SerializeField] private float fadeDuration = 0.5f;

        private Vector3 startPosition;
        private Vector3 endPosition;
        private float thickness;
        private float maxAlpha;
        private Color baseColor;
        private Color color;
        private LineRenderer lineRenderer;

        private void Awake()
        {
            thickness = tileSize * 0.02f;
            alpha = 0f;
            maxAlpha = 1f;
            baseColor = ColorHelper.RGBA(100, 195, 200, 0);
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        private void Start()
        {
            lineRenderer.startWidth = thickness;
            lineRenderer.endWidth = thickness;
        }

        public void Spawn(ActorPair actorPair)
        {
            parent = board.transform;
            name = $"AttackLine_{Guid.NewGuid():N}";

            startPosition = actorPair.startActor.position;
            endPosition = actorPair.endActor.position;

            Vector3 ul, ur, lr, ll;
            float offset = tileSize / 2;
            Vector3[] points = { };

            if (actorPair.axis == Axis.Vertical)
            {
                ul = new Vector3(startPosition.x - offset, startPosition.y - offset, 0);
                ur = new Vector3(startPosition.x + offset, startPosition.y - offset, 0);
                lr = new Vector3(endPosition.x + offset, endPosition.y + offset, 0);
                ll = new Vector3(endPosition.x - offset, endPosition.y + offset, 0);
                points = new Vector3[] { ul, ur, lr, ll, ul };
            }
            else if (actorPair.axis == Axis.Horizontal)
            {
                ul = new Vector3(endPosition.x - offset, endPosition.y - offset, 0);
                ur = new Vector3(startPosition.x + offset, startPosition.y - offset, 0);
                lr = new Vector3(startPosition.x + offset, startPosition.y + offset, 0);
                ll = new Vector3(endPosition.x - offset, endPosition.y + offset, 0);
                points = new Vector3[] { ul, ur, lr, ll, ul };
            }

            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);

            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            float startAlpha = 0f;
            float targetAlpha = maxAlpha;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
                SetAlpha(alpha);
                yield return null;
            }

            alpha = maxAlpha;
            SetAlpha(alpha);
        }

        public void TriggerDespawn()
        {
            StartCoroutine(Despawn());
        }

        public IEnumerator Despawn()
        {
            //Before:
            float startAlpha = maxAlpha;
            float targetAlpha = 0f;
            float elapsedTime = 0f;

            //During:
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
                SetAlpha(alpha);
                yield return null;
            }

            //After:
            alpha = 0f;
            SetAlpha(alpha);
            StopAllCoroutines();
        }

        private void SetAlpha(float a)
        {
            color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
}
