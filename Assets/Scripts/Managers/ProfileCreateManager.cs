using Assets.Helper;
using System.Collections;
using UnityEngine;
using c = Assets.Helpers.CanvasHelper;

public class ProfileCreateManager : MonoBehaviour
{
    //Fields
    private RectTransform background;
    private FadeInstance fade;
    private float screenWidth;
    private float screenHeight;

    private void Awake()
    {
        background = GameObject.Find(GameObjectHelper.ProfileCreate.Background).GetComponent<RectTransform>();
        fade = GameObject.Find(GameObjectHelper.ProfileCreate.Fade).GetComponent<FadeInstance>();

        screenWidth = c.CanvasRect.rect.width;
        screenHeight = c.CanvasRect.rect.height;

        background.sizeDelta = new Vector2(screenWidth, screenHeight);

        IEnumerator showKeyboardRoutine()
        {
            KeyboardDialog.Show("Who are you?", onSubmit: (value) =>
            {
                ProfileRepo.CreateProfile(value);
                StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadTitleScreen()));
            });

            yield return Wait.None();
        }

        StartCoroutine(fade.FadeInRoutine(showKeyboardRoutine()));
    }

}
