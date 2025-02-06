using System.Collections.Generic;
using System;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class Tutorial
    {
        public string Key;
        public List<TutorialPage> Pages = new List<TutorialPage>();
    }

    [Serializable]
    public class TutorialPage
    {
        public string ImageKey;
        public string Title;
        public string Content;
    }
}
