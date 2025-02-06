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

        //Internal properties
        bool hasPages => pages != null && pages.Count > 0;

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
            panel.SetActive(false);
        }

        public void Load(Tutorial tutorial, bool show = true)
        {
            if (tutorial == null || tutorial.Pages.Count < 1) return;

            this.pages = tutorial.Pages;
            currentPage = 0;

            if (show)
                Show();
        }

        public void Show(int currentPage = 0)
        {
            if (!hasPages) return;

            Time.timeScale = 0f;
            this.currentPage = currentPage;       
            panel.SetActive(true);
            Navigate();
        }

        private void Navigate()
        {
            if (!hasPages) return;

            image.sprite = resourceManager.Texture(pages[currentPage].ImageKey).Value.ToSprite();
            title.text = pages[currentPage].Title;
            content.text = pages[currentPage].Content;

            //Manage Button Visibility
            previousButton.gameObject.SetActive(currentPage > 0);
            nextButton.gameObject.SetActive(currentPage < pages.Count - 1);
            closeButton.gameObject.SetActive(currentPage == pages.Count - 1);
        }

        public void PreviousPage()
        {
            Debug.Log("Previous button clicked!");
            if (currentPage > 0)
            {
                currentPage--;
                Navigate();
            }
        }

        public void NextPage()
        {
            if (currentPage < pages.Count - 1)
            {
                currentPage++;
                Navigate();
            }
        }

       

        public void Close()
        {
            Time.timeScale = 1f;
            panel.SetActive(false);
        }

    }

}
