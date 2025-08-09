// --- File: Assets/Scripts/Helpers/FadeHelper.cs ---
using UnityEngine;
using UnityEngine.SceneManagement;
using c = Assets.Helpers.CanvasHelper;

namespace Assets.Helpers
{
    /// <summary>
    /// Caches the current scene's FadeInstance so callers always get the correct reference.
    /// Looks up the Fade GameObject by name using GameObjectHelper.Overworld.Fade once per scene load.
    /// </summary>
    public static class FadeHelper
    {
        // Cached component reference for the active scene
        private static FadeInstance fade;

        /// <summary>
        /// Fast access to the cached FadeInstance.
        /// If the cache is empty, performs a one time lookup for the current scene.
        /// </summary>
        public static FadeInstance Fade
        {
            get
            {
                if (fade == null) Cache();
                return fade;
            }
        }

        /// <summary>
        /// Initialize on first scene after load and refresh cache when scenes change.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            fade = null;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Cache();
        }

        /// <summary>
        /// Scene change callback that refreshes the cached reference.
        /// </summary>
        private static void OnSceneLoaded(Scene _, LoadSceneMode __)
        {
            Cache();
        }

        /// <summary>
        /// Performs the actual lookup and caches the FadeInstance.
        /// Safe if the GameObject is missing.
        /// </summary>
        private static void Cache()
        {
            if (c.Canvas == null)
            {
                fade = null;
                return;
            }

            var go = c.Canvas.transform.Find("Fade");
            if (go == null)
            {
                fade = null;
                return;
            }

            fade = go.GetComponent<FadeInstance>();
        }

    }
}
