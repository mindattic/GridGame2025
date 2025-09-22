using Assets.Helper;
using Assets.Scripts.Libraries;
using Assets.Scripts.Models;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.GUI
{
    public class TutorialPopup : MonoBehaviour
    {
        //Components
        private GameObject panel;
        private Image image;
        private TextMeshProUGUI title;
        private TextMeshProUGUI content;
        private Button previousButton;
        private Button nextButton;
        private Button closeButton;

        //Fields
        private List<TutorialPage> pages = new List<TutorialPage>();
        private int currentPage = 0;

        //Properties
        bool hasPages => pages != null && pages.Count > 0;
        int lastPage => pages?.Count - 1 ?? 0;

        private bool initialized;

        // Awake intentionally empty; initialization driven by GameManager.Start via Initialize().
        private void Awake() { }

        public void Initialize()
        {
            if (initialized) return;

            panel = GameObjectHelper.Game.TutorialPopup.Panel;
            image = GameObjectHelper.Game.TutorialPopup.Image;
            title = GameObjectHelper.Game.TutorialPopup.TitleTextX;
            content = GameObjectHelper.Game.TutorialPopup.ContentTextX;
            previousButton = GameObjectHelper.Game.TutorialPopup.PreviousButton;
            nextButton = GameObjectHelper.Game.TutorialPopup.NextButton;
            closeButton = GameObjectHelper.Game.TutorialPopup.CloseButton;

            initialized = true;
        }

        private void Start()
        {
            if (!initialized) Initialize();
            panel.SetActive(GameManager.instance.debugManager.showTutorials);
        }

        public void Load(Tutorial tutorial, bool show = true)
        {
            if (!g.DebugManager.showTutorials || tutorial == null || tutorial.Pages.Count < 1) return;

            this.pages = tutorial.Pages;
            currentPage = 0;

            if (show)
                Show();
        }

        public void Show(int currentPage = 0)
        {
            if (!g.DebugManager.showTutorials || !hasPages) return;

            //Time.timeScale = 0f;
            this.currentPage = currentPage;
            panel.SetActive(true);
            Navigate();
        }

        private void Navigate()
        {
            if (!g.DebugManager.showTutorials || !hasPages) return;

            image.sprite = SpriteLibrary.TutorialPages[pages[currentPage].TextureKey];
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
