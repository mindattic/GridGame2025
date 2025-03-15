using UnityEngine;
using UnityEngine.UI;

public class StageLineRenderer : CanvasLineRenderer
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button endButton;

    private void Update()
    {
        if (startButton != null && endButton != null)
        {
            UpdateLine(startButton, endButton);
        }
    }
}
