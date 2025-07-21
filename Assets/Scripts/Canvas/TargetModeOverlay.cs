using UnityEngine;
using UnityEngine.UI;
using g = GameManagerHelper;

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
        g.InputManager.OnInputModeChanged += HandleModeChanged;

        // initial state
        HandleModeChanged(g.InputManager.inputMode);
    }

    private void OnDestroy()
    {
        // unsubscribe
        if (g.InputManager != null)
            g.InputManager.OnInputModeChanged -= HandleModeChanged;
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
