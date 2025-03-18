using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialStore", menuName = "Stores/TutorialStore")]
public class TutorialStore : ScriptableObject
{
    //Singleton
    private static TutorialStore _instance;

    public static TutorialStore instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("TutorialStore accessed before being initialized! Ensure it's assigned in Awake().");
            return _instance;
        }
    }

    //Initialize
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<TutorialStore>("Stores/TutorialStore");
            if (_instance == null)
                Debug.LogError("TutorialStore asset not found in Resources/Stores/TutorialStore");
        }
    }

    //Serialized fields
    [SerializeField] public Dictionary<string, Tutorial> Tutorials;

    private void OnEnable()
    {
        Load();
    }

    private void Load()
    {
        Tutorials = new Dictionary<string, Tutorial>
        {
            { "Tutorial1", new Tutorial
                {
                    Key = "Tutorial1",
                    Pages = new List<TutorialPage>
                    {
                        new TutorialPage { TextureKey = "Tutorial.1-1", Title = "Tutorial 1-1", Content = "This is the first page of the tutorial." },
                        new TutorialPage { TextureKey = "Tutorial.1-2", Title = "Tutorial 1-2", Content = "This is the second page of the tutorial." },
                        new TutorialPage { TextureKey = "Tutorial.1-3", Title = "Tutorial 1-3", Content = "This is the third page of the tutorial." }
                    }
                }
            }
        };
    }

}
