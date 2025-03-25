using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialRepo", menuName = "Repositories/TutorialRepo")]
public class TutorialRepo : ScriptableObject
{
    //Singleton
    private static TutorialRepo Instance;

    public static TutorialRepo instance
    {
        get
        {
            if (Instance == null)
                Debug.LogError("TutorialRepo accessed before being initialized! Ensure it's assigned in Awake().");
            return Instance;
        }
    }

    //Assign
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null)
        {
            Instance = Resources.Load<TutorialRepo>("Repositories/TutorialRepo");
            if (Instance == null)
                Debug.LogError("TutorialRepo asset not found in Resources/Repositories/TutorialRepo");
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
