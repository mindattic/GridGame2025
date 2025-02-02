using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GUI
{
    public class DebugButtonPanel : MonoBehaviour
    {
        //External properties
        protected BoardInstance board => GameManager.instance.board;
        protected StageManager stageManager => GameManager.instance.stageManager;
        protected DebugManager debugManager => GameManager.instance.debugManager;

        //Fields
        [SerializeField] private RectTransform PanelRect;
        [SerializeField] private Button ReloadButton;
        [SerializeField] private Button PreviousStageButton;
        [SerializeField] private Button NextStageButton;
        [SerializeField] private Button SpawnRandomEnemyButton;

        private void Start()
        {
            ////Set anchors and pivot to center
            //PanelRect.anchorMin = new Vector2(0.5f, 0.5f);
            //PanelRect.anchorMax = new Vector2(0.5f, 0.5f);
            //PanelRect.pivot = new Vector2(0.5f, 0.5f);

            ////Set the anchored position to (0, 0) for centering
            //PanelRect.anchoredPosition = Vector2.zero;

            //Assign button click listeners
            ReloadButton.onClick.AddListener(OnLoadButtonClicked);
            PreviousStageButton.onClick.AddListener(OnPreviousStageButtonClicked);
            NextStageButton.onClick.AddListener(OnNextStageButtonClicked);
            SpawnRandomEnemyButton.onClick.AddListener(OnSpawnRandomEnemyButtonClicked);
        }


        private void OnLoadButtonClicked()
        {
            stageManager.LoadStage("Stage1");
        }

        private void OnPreviousStageButtonClicked()
        {
            //stageManager.Previous();
        }

        private void OnNextStageButtonClicked()
        {
            //stageManager.Next();
        }

        private void OnSpawnRandomEnemyButtonClicked()
        {
            debugManager.SpawnRandomEnemy();
        }
    }
}
