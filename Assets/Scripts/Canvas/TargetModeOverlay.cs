using UnityEngine;
using UnityEngine.UI;
using game = GameManagerHelper;

public class TargetModeOverlay : MonoBehaviour
{

    //Components
    private Image image;
    
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Initialize()
    {
        // subscribe
        game.Input.OnInputModeChanged += HandleModeChanged;

        // initial state
        HandleModeChanged(game.Input.inputMode);
    }

    private void OnDestroy()
    {
        // unsubscribe
        if (game.Input != null)
            game.Input.OnInputModeChanged -= HandleModeChanged;
    }

    private void HandleModeChanged(InputMode mode)
    {
        switch (mode)
        {
            case InputMode.HeroTurn:
                this.gameObject.SetActive(false);
                break;

            case InputMode.AbilityTarget:
                this.gameObject.SetActive(true);
                break;
        }
    }
}
