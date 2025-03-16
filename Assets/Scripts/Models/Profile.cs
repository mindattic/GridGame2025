using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Models
{
    [Serializable]
    public class Profile
    {
        //Fields
        public string Name;
        public string Folder;
        public SaveFile CurrentSave; //Reference to current save file
        public List<SaveFile> SaveFiles;

        //Properties
        public SaveFile LatestSave => SaveFiles.OrderByDescending(x => x.Timestamp).First();

        public Profile() { }

        public Profile(string name)
        {
            Name = name;
            SaveFiles = new List<SaveFile>();
        }
    }

  
}
