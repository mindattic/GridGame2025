using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Helpers;
using Assets.Scripts.Managers;
using c = Assets.Helpers.CanvasHelper;
using g = Assets.Helpers.GameHelper;
using scene = Assets.Helpers.SceneHelper;

public class VictoryManager : MonoBehaviour
{
    [Header("Wiring")]
    public RectTransform ScrollContent;
    public Button NextButton;

    [Header("Prefabs")] public GameObject HeroExperiencePanePrefab;

    [Header("Behavior")] public string NextSceneName = SceneHelper.Overworld;
    [Header("Behavior")] public float AutoEnableDelay = 0.25f; // small delay after last fill

    private const string NewScrollPath = "ScrollView/Viewport/Content";   // renamed hierarchy
    private const string LegacyScrollPath = "HeroScroll/Viewport/Content"; // backward compatibility

    private readonly List<HeroExperiencePane> _panes = new List<HeroExperiencePane>();
    private bool _monitoring;

    private void Awake()
    {
        // If we were launched directly without a battle (no save or participants) just go to TitleScreen
        var save = ProfileHelper.CurrentProfile?.CurrentSave;
        if (save == null || save.Party == null || save.Party.Members == null || save.Party.Members.Count == 0)
        {
            SceneHelper.Switch.ToTitleScreen();
            return;
        }

        // Auto-find typical hierarchy if fields aren't wired in the scene
        if (ScrollContent == null)
        {
            Transform contentTf = null;
            if (c.CanvasRect != null)
            {
                contentTf = c.CanvasRect.Find(NewScrollPath);
                if (contentTf == null) // fallback to legacy name if scene not updated
                    contentTf = c.CanvasRect.Find(LegacyScrollPath);
            }
            if (contentTf != null)
                ScrollContent = contentTf.GetComponent<RectTransform>();
        }

        if (NextButton == null)
        {
            var nextTf = c.CanvasRect != null ? c.CanvasRect.Find("BottomBar/NextButton") : null;
            if (nextTf != null)
                NextButton = nextTf.GetComponent<Button>();
        }

        if (HeroExperiencePanePrefab == null)
            HeroExperiencePanePrefab = PrefabLibrary.Get("HeroExperiencePane");

        if (NextButton != null)
        {
            NextButton.onClick.AddListener(OnNext);
            NextButton.gameObject.SetActive(false); // hide until fill complete
        }

        if (string.IsNullOrEmpty(NextSceneName))
            NextSceneName = ExperienceTracker.NextSceneAfterVictory;

        BuildPanes();
    }

    void Start()
    {
        scene.FadeIn();
    }

    private void OnDestroy()
    {
        if (NextButton != null)
            NextButton.onClick.RemoveListener(OnNext);
    }

    private void Update()
    {
        if (!_monitoring) return;
        if (_panes.Count == 0) return;
        if (_panes.All(p => p != null && p.IsFillComplete))
        {
            _monitoring = false;
            StartCoroutine(EnableNextSoon());
        }
    }

    private IEnumerator EnableNextSoon()
    {
        yield return new WaitForSeconds(AutoEnableDelay);
        if (NextButton != null)
            NextButton.gameObject.SetActive(true);
    }

    private void BuildPanes()
    {
        if (ScrollContent == null || HeroExperiencePanePrefab == null) return;

        // Build panes for party members (participants first), then roster others.
        var save = ProfileHelper.CurrentProfile?.CurrentSave;
        if (save == null) return;

        var party = save.Party?.Members?.Select(m => m.Character).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>();
        var roster = save.Roster?.Members?.Select(m => m.Character).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>();

        _panes.Clear();

        // Participants first
        foreach (var ch in party)
        {
            var xp = ExperienceTracker.GetXPGained(ch);
            CreatePane(ch, inParty: true, xpGained: xp);
        }

        // Roster not in party
        foreach (var ch in roster.Where(c => !party.Contains(c)))
        {
            var xp = ExperienceTracker.GetXPGained(ch);
            CreatePane(ch, inParty: false, xpGained: xp);
        }

        _monitoring = true; // begin monitoring fill completion
        if (_panes.All(p => p.IsFillComplete)) // edge case: no gains
            StartCoroutine(EnableNextSoon());
    }

    private void CreatePane(string character, bool inParty, int xpGained)
    {
        if (HeroExperiencePanePrefab == null || ScrollContent == null) return;
        var go = Instantiate(HeroExperiencePanePrefab, ScrollContent);
        go.name = $"Pane_{character}";
        var pane = go.GetComponent<HeroExperiencePane>();
        if (pane != null)
        {
            pane.Build(character, xpGained, inParty);
            _panes.Add(pane);
        }
    }

    private void OnNext()
    {
        // Apply the accumulated XP to the save using ExperienceHelper rules, then clear session.
        var save = ProfileHelper.CurrentProfile?.CurrentSave;
        if (save != null)
        {
            foreach (var kv in ExperienceTracker.AllGains)
            {
                var c = kv.Key; var gained = kv.Value;
                // Update the party/roster save entries
                var entry = save.Party.Members.FirstOrDefault(m => m.Character == c) ?? save.Roster.Members.FirstOrDefault(m => m.Character == c);
                if (entry != null)
                {
                    int level = Mathf.Max(1, entry.Level);
                    int cur = Mathf.Max(0, entry.CurrentXP);
                    int total = Mathf.Max(0, entry.TotalXP);

                    cur += gained; total += gained;
                    while (cur >= Assets.Helpers.ExperienceHelper.NextLevel(level))
                    {
                        cur -= Assets.Helpers.ExperienceHelper.NextLevel(level);
                        level += 1;
                    }

                    entry.Level = level;
                    entry.CurrentXP = cur;
                    entry.TotalXP = total;
                }
            }

            ProfileHelper.Save(true);
        }

        ExperienceTracker.Clear();
        SceneHelper.Fade.To(NextSceneName);
    }
}
