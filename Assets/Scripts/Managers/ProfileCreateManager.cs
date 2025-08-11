using Assets.Helper;
using System.Collections;
using UnityEngine;
using c = Assets.Helpers.CanvasHelper;
using scene = Assets.Helpers.SceneHelper;

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
                scene.Change.ToTitleScreen();
            });

            yield return Wait.None();
        }

        scene.FadeIn(showKeyboardRoutine());
    }

}
