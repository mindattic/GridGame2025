using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.Behaviors
{
    public class LogManager : MonoBehaviour
    {
        private string log;
        private List<string> messages = new List<string>();

        const int MaxMessages = 10;

        #region Components

        public string text
        {
            get => log;
            set => log = value;
        }

        #endregion
        public void Info(string message)
        {
            Debug.Log(message);
            messages.Add($@"<color=""white"">{message}</color>");
        }

        public void Success(string message)
        {
            Debug.Log(message);
            messages.Add($@"<color=""green"">{message}</color>");
        }

        public void Warning(string message)
        {
            Debug.LogWarning(message);
            messages.Add($@"<color=""orange"">{message}</color>");
        }


        public void Error(string message)
        {
            Debug.LogError(message);
            messages.Add($@"<color=""red"">{message}</color>");
        }


        public void Exception(UnityException ex)
        {
            Debug.LogError(ex.Message.ToString());
            messages.Add($@"<color=""red"">{ex.Message.ToString()}</color>");
        }

        private void Update()
        {
            if (messages.Count < MaxMessages)
                return;

            //Truncate messages
            messages.RemoveAt(0);

            //Print in descending order
            log = string.Join(Environment.NewLine, messages.OrderByDescending(x => x.ToString()));
        }



    }
}