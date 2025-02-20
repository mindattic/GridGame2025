using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Models;
using TMPro;

namespace Assets.Scripts.GUI
{
    public class TutorialPopup : MonoBehaviour
    {
        //External properties
        protected float previousGameSpeed { get => GameManager.instance.previousGameSpeed; set => GameManager.instance.previousGameSpeed = value; }
        protected ResourceManager resourceManager => GameManager.instance.resourceManager;
        protected DebugManager debugManager => GameManager.instance.debugManager;

        //Internal properties
        bool hasPages => pages != null && pages.Count > 0;
        int lastPage => pages?.Count - 1 ?? 0;

        //Fields
        [SerializeField] public GameObject panel;
        [SerializeField] public Image image;
        [SerializeField] public TextMeshProUGUI title;
        [SerializeField] public TextMeshProUGUI content;
        [SerializeField] public Button previousButton;
        [SerializeField] public Button nextButton;
        [SerializeField] public Button closeButton;
        private List<TutorialPage> pages = new List<TutorialPage>();
        private int currentPage = 0;

        private void Start()
        {
            //RectTransform rect;
            
            //rect = panel.GetComponent<RectTransform>();
            //rect.sizeDelta = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            //rect = image.GetComponent<RectTransform>();
            //rect.sizeDelta = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            panel.SetActive(debugManager.showTutorials);
        }

        public void Load(Tutorial tutorial, bool show = true)
        {
            if (!debugManager.showTutorials || tutorial == null || tutorial.Pages.Count < 1) return;

            this.pages = tutorial.Pages;
            currentPage = 0;

            if (show)
                Show();
        }

        public void Show(int currentPage = 0)
        {
            if (!debugManager.showTutorials || !hasPages) return;

            //Time.timeScale = 0f;
            this.currentPage = currentPage;       
            panel.SetActive(true);
            Navigate();
        }

        private void Navigate()
        {
            if (!debugManager.showTutorials || !hasPages) return;

            image.sprite = resourceManager.Texture(pages[currentPage].TextureKey).Value.ToSprite();
            title.text = pages[currentPage].Title;
            content.text = pages[currentPage].Content;

            //Manage Button Visibility
            previousButton.gameObject.SetActive(currentPage > 0);
            nextButton.gameObject.SetActive(currentPage < lastPage);
            closeButton.gameObject.SetActive(currentPage == lastPage);
        }

        public void PreviousPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
                Navigate();
            }
        }

        public void NextPage()
        {
            if (currentPage < lastPage)
            {
                currentPage++;
                Navigate();
            }
        }

       

        public void Close()
        {
            //Time.timeScale = 1f;
            panel.SetActive(false);
        }

    }

}
