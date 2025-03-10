using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Store
{
    public static class SceneHub
    {
        public static string PreviousScene;
        public static string CurrentScene;

        public static IEnumerator LoadScene(string sceneName)
        {
            PreviousScene = SceneManager.GetActiveScene().name;
            CurrentScene = sceneName;
            SceneManager.LoadScene(CurrentScene);
            yield break;
        }

        public static IEnumerator LoadPreviousScene()
        {
            CurrentScene = PreviousScene;
            PreviousScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(CurrentScene);
            yield break;
        }

    }


  

}
