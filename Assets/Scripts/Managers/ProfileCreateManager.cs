using Assets.Helper;
using Assets.Helpers;
using System.Collections;
using UnityEngine;
using c = Assets.Helpers.CanvasHelper;
using scene = Assets.Helpers.SceneHelper;

public class ProfileCreateManager : MonoBehaviour
{
    // Background panel that is sized to the full canvas.
    private RectTransform background;

    private void Awake()
    {
        // Locate the background object safely.
        GameObject bgGO = GameObject.Find(GameObjectHelper.ProfileCreate.Background);
        if (bgGO == null)
        {
            Debug.LogError($"ProfileCreate background not found: {GameObjectHelper.ProfileCreate.Background}");
            return;
        }

        // Extract RectTransform and validate it exists.
        background = bgGO.GetComponent<RectTransform>();
        if (background == null)
        {
            Debug.LogError("ProfileCreate background is missing a RectTransform component.");
            return;
        }

        // Validate canvas rect is available.
        if (c.CanvasRect == null)
        {
            Debug.LogError("CanvasHelper.CanvasRect is null. Cannot size background.");
            return;
        }

        // Read canvas dimensions and size the background to match.
        float screenWidth = c.CanvasRect.rect.width;
        float screenHeight = c.CanvasRect.rect.height;

        background.sizeDelta = new Vector2(screenWidth, screenHeight);

        // Local coroutine to show the keyboard dialog after fade-in.
        IEnumerator showKeyboardRoutine()
        {
            // Show a prompt to create a profile.
            KeyboardDialog.Show(
                "Who are you?",
                onSubmit: (value) =>
                {
                    try
                    {
                        // Create the profile with the provided name.
                        ProfileHelper.CreateProfile(value);

                        // Navigate back to the title screen once created.
                        scene.Fade.ToTitleScreen();
                    }
                    catch (System.SystemException ex)
                    {
                        Debug.LogError($"Failed to create profile: {ex.Message}");
                    }
                }
            );

            // Yield once to allow UI flow to continue.
            yield return Wait.None();
        }

        // Begin scene fade-in, then present the keyboard dialog.
        scene.FadeIn(showKeyboardRoutine());
    }
}
