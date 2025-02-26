using System;
using System.Collections;
using UnityEngine;

namespace Game.Instances
{
    public class AttackLineInstance : MonoBehaviour
    {
        //External properties
        protected float tileSize => GameManager.instance.tileSize;
        protected BoardInstance board => GameManager.instance.board;
        public Transform parent { get => gameObject.transform.parent; set => gameObject.transform.SetParent(value, true); }
        public Vector3 position { get => gameObject.transform.position; set => gameObject.transform.position = value; }
        public int sortingOrder { get => lineRenderer.sortingOrder; set => lineRenderer.sortingOrder = value; }

        //Fields
        public float alpha;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private float thickness;
        private float maxAlpha;
        private Color baseColor;
        private Color color;
        private LineRenderer lineRenderer;

        //Method which is used for initialization tasks that need to occur before the game starts 
        private void Awake()
        {
            thickness = tileSize * 0.02f;
            alpha = 0f;
            maxAlpha = 1f;
            baseColor = ColorHelper.RGBA(100, 195, 200, 0);

            lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.sortingOrder = SortingOrder.AttackLine;
        }

        //Method which is automatically called before the first frame update  
        void Start()
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
            
            Vector3[] points = { };
            Vector3 ul;
            Vector3 ur;
            Vector3 lr;
            Vector3 ll;
            float offset = tileSize / 2;

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

            lineRenderer.sortingOrder = SortingOrder.AttackLine;
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);

            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            //Before:
            alpha = 0f;
            color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            //During:
            while (alpha < maxAlpha)
            {
                alpha += Increment.OnePercent;
                alpha = Mathf.Clamp(alpha, Opacity.Transparent, maxAlpha);
                color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;

                yield return Wait.OneTick();
            }

            //After:
            alpha = maxAlpha;
            color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        public void TriggerDespawn()
        {
            StartCoroutine(Despawn());
        }

        public IEnumerator Despawn()
        {
            //Before:
            alpha = maxAlpha;
            color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            //During:
            while (alpha > 0)
            {
                alpha -= Increment.OnePercent;
                alpha = Mathf.Clamp(alpha, Opacity.Transparent, maxAlpha);
                color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;

                yield return Wait.OneTick();
            }

            //After:
            alpha = 0;
            color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

    }
}


