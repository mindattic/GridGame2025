// --- File: Assets/Scripts/Helpers/CanvasHelper.cs ---
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Helpers
{
    public static class CanvasHelper
    {
        private static Canvas canvas;
        private static RectTransform canvasRect;

        public static Canvas Canvas
        {
            get
            {
                if (canvas == null) Cache();
                return canvas;
            }
        }

        public static RectTransform CanvasRect
        {
            get
            {
                if (canvasRect == null) Cache();
                return canvasRect;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            canvas = null;
            canvasRect = null;

            SceneManager.sceneLoaded += OnSceneLoaded;
            Cache(); // first scene
        }

        private static void OnSceneLoaded(Scene _, LoadSceneMode __)
        {
            Cache();
        }

        private static void Cache()
        {
            var go = GameObject.Find("Canvas");
            if (go != null)
            {
                canvas = go.GetComponent<Canvas>();
                canvasRect = canvas != null ? canvas.GetComponent<RectTransform>() : null;
            }
            else
            {
                canvas = null;
                canvasRect = null;
            }
        }

    }
}
