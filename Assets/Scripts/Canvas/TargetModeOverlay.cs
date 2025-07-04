using UnityEngine;
using UnityEngine.UI;

public class TargetModeOverlay : MonoBehaviour
{
    #region Game Properies
    protected InputManager inputManager => GameManager.instance.inputManager;
    #endregion

    //Components
    private Image image;
    
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Initialize()
    {
        // subscribe
        inputManager.OnInputModeChanged += HandleModeChanged;

        // initial state
        HandleModeChanged(inputManager.inputMode);
    }

    private void OnDestroy()
    {
        // unsubscribe
        if (inputManager != null)
            inputManager.OnInputModeChanged -= HandleModeChanged;
    }

    private void HandleModeChanged(InputMode mode)
    {
        switch (mode)
        {
            case InputMode.Gameplay:
                this.gameObject.SetActive(false);
                break;

            case InputMode.AbilityTarget:
                this.gameObject.SetActive(true);
                break;
        }
    }
}
