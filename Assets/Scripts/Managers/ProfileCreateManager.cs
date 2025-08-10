using Assets.Helper;
using System.Collections;
using UnityEngine;
using c = Assets.Helpers.CanvasHelper;
using f = Assets.Helpers.FadeOverlayHelper;

public class ProfileCreateManager : MonoBehaviour
{
    //Fields
    private RectTransform background;
    private float screenWidth;
    private float screenHeight;

    private void Awake()
    {
        background = GameObject.Find(GameObjectHelper.ProfileCreate.Background).GetComponent<RectTransform>();

        screenWidth = c.CanvasRect.rect.width;
        screenHeight = c.CanvasRect.rect.height;

        background.sizeDelta = new Vector2(screenWidth, screenHeight);

        IEnumerator showKeyboardRoutine()
        {
            KeyboardDialog.Show("Who are you?", onSubmit: (value) =>
            {
                ProfileRepo.CreateProfile(value);
                f.Overlay.FadeOut(SceneHelper.LoadTitleScreen());
            });

            yield return Wait.None();
        }

        f.Overlay.FadeIn(showKeyboardRoutine());
    }

}
