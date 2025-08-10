using UnityEngine;
using f = Assets.Helpers.FadeOverlayHelper;

public class LoadingScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        f.Overlay.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
