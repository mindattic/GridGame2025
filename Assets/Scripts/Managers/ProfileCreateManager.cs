using Assets.Scripts.Repositories;
using System.Collections;
using UnityEngine;

public class ProfileCreateManager : MonoBehaviour
{
    //Fields
    private RectTransform canvas2D;
    private RectTransform background;
    private FadeInstance fade;
    private float screenWidth;
    private float screenHeight;

    private void Awake()
    {
        canvas2D = GameObject.Find(ComponentHelper.ProfileCreate.Canvas2D).GetComponent<RectTransform>();
        background = GameObject.Find(ComponentHelper.ProfileCreate.Background).GetComponent<RectTransform>();
        fade = GameObject.Find(ComponentHelper.ProfileCreate.Fade).GetComponent<FadeInstance>();

        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

        background.sizeDelta = new Vector2(screenWidth, screenHeight);

        IEnumerator showKeyboard()
        {
            KeyboardDialog.Show(canvas2D, "Who are you?", onSubmit: (value) =>
            {
                ProfileRepo.instance.CreateProfile(value);
                StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.TitleScreen)));
            });

            yield return null;
        }

        StartCoroutine(fade.FadeIn(showKeyboard()));
    }

}
