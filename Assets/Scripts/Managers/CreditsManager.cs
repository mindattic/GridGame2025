using Assets.Scripts.Store;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ComponentHelper;

public class CreditsManager : MonoBehaviour
{
    //Fields
    private Fade fade;
    private TextMeshProUGUI credits;

    private void Awake()
    {
        fade = GameObject.Find(ComponentHelper.Settings.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");
        credits = GameObject.Find(ComponentHelper.Settings.Credits).GetComponent<TextMeshProUGUI>() ?? throw new UnityException("Credits is null");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        const string NL = "\r\n";
        string text 
            = $"{NL}{NL}" 
            + $"<u>Game Design & Development</u>{NL}" 
            + $"<size=150%>Ryan DeBraal</size>{NL}{NL}" 
            + $"<u>Typography</u>{NL}" 
            + $"<size=150%>Brian Willson</size> <size=50%>(Attic)</size>{NL}" 
            + $"<size=150%>Jonas Hecksher</size> <size=50%>(Play)</size>{NL}{NL}" 
            + $"<u>Visual Effects</u>{NL}" 
            + $"<size=150%>Eric Wang</size>{NL}" + 
            "";
        credits.text = text;

        StartCoroutine(fade.FadeIn());
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneHub.LoadPreviousScene()));
    }

  
}
