using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class RockInstance : MonoBehaviour
{
    [Header("Sorting")]
    [Tooltip("Match hero's sorting layer and go behind when hero is below, in front when above.")]
    public bool followHeroSorting = true;

    private SpriteRenderer spriteRenderer;

    // Cache hero and its SpriteRenderer once for all rocks
    private static OverworldHero hero;
    private static SpriteRenderer heroSR;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position.SetZ(0f); // ensure on Z=0 plane

        // Apply initial sort
        if (followHeroSorting) YSortUtility.Apply(spriteRenderer);
    }

    private void OnEnable()
    {
        TryCacheHero();
        if (followHeroSorting) YSortUtility.Apply(spriteRenderer);
    }

    private void Update()
    {
        if (!followHeroSorting) return;
        YSortUtility.Apply(spriteRenderer);
    }

    private static void TryCacheHero()
    {
        if (hero == null) hero = Object.FindObjectOfType<OverworldHero>();
        if (hero != null && (heroSR == null || heroSR.gameObject != hero.gameObject))
            heroSR = hero.GetComponent<SpriteRenderer>();
    }
}
