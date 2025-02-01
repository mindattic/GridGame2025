using Game.Instances.Actor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActorRenderers
{
    public ActorRenderers() { }

    public Color opaqueColor = ColorHelper.Solid.White;
    public Color qualityColor = ColorHelper.Solid.White;
    public float qualityAlphaMax = Opacity.Opaque;
    public Color glowColor = ColorHelper.Solid.White;
    public Color parallaxColor = ColorHelper.Solid.White;
    public float parallaxAlphaMax = Opacity.Percent50;
    public Color thumbnailColor = ColorHelper.Solid.White;
    public Color frameColor = ColorHelper.Solid.White;
    public Color healthBarColor = ColorHelper.HealthBar.Green;
    public Color actionBarColor = ColorHelper.ActionBar.Blue;
    public Color turnDelayColor = ColorHelper.Solid.Red;
    public Color weaponIconColor = ColorHelper.Solid.White;
    public Color armorColor = ColorHelper.Solid.White;
    public Color overlayColor = ColorHelper.Transparent.White;
    public Color selectionColor = ColorHelper.Solid.White;

    public Transform front;
    public Transform back;

    public SpriteRenderer opaque;
    public SpriteRenderer quality;
    public SpriteRenderer glow;
    public SpriteRenderer parallax;
    public SpriteRenderer thumbnail;
    public SpriteRenderer frame;
    public SpriteRenderer statusIcon;
    public GameObject healthBar;
    public SpriteRenderer healthBarBack;
    public SpriteRenderer healthBarDrain;
    public SpriteRenderer healthBarFill;
    public TextMeshPro healthBarText;
    public GameObject actionBar;
    public SpriteRenderer actionBarBack;
    public SpriteRenderer actionBarDrain;
    public SpriteRenderer actionBarFill;
    public TextMeshPro actionBarText;
    public SpriteMask mask;
    public SpriteRenderer radialBack;
    public SpriteRenderer radial;
    public TextMeshPro radialText;
    public TextMeshPro turnDelayText;
    public TextMeshPro nameTagText;
    public SpriteRenderer weaponIcon;
    public SpriteRenderer armorNorth;
    public SpriteRenderer armorEast;
    public SpriteRenderer armorSouth;
    public SpriteRenderer armorWest;
    public SpriteRenderer overlay;
    public SpriteRenderer selectionBox;

    private ActorInstance instance;
    public void Initialize(ActorInstance parentInstance)
    {
        this.instance = parentInstance;

        front = instance.transform.GetChild(ActorLayer.Name.Front);

        opaque = front.GetChild(ActorLayer.Name.Opaque).GetComponent<SpriteRenderer>();
        quality = front.GetChild(ActorLayer.Name.Quality).GetComponent<SpriteRenderer>();
        glow = front.GetChild(ActorLayer.Name.Glow).GetComponent<SpriteRenderer>();
        parallax = front.GetChild(ActorLayer.Name.Parallax).GetComponent<SpriteRenderer>();
        thumbnail = front.GetChild(ActorLayer.Name.Thumbnail).GetComponent<SpriteRenderer>();
        frame = front.GetChild(ActorLayer.Name.Frame).GetComponent<SpriteRenderer>();
        statusIcon = front.GetChild(ActorLayer.Name.StatusIcon).GetComponent<SpriteRenderer>();
        healthBarBack = front.GetChild(ActorLayer.Name.HealthBar.Root).GetChild(ActorLayer.Name.HealthBar.Back).GetComponent<SpriteRenderer>();
        healthBarDrain = front.GetChild(ActorLayer.Name.HealthBar.Root).GetChild(ActorLayer.Name.HealthBar.Drain).GetComponent<SpriteRenderer>();
        healthBarFill = front.GetChild(ActorLayer.Name.HealthBar.Root).GetChild(ActorLayer.Name.HealthBar.Fill).GetComponent<SpriteRenderer>();
        healthBarText = front.GetChild(ActorLayer.Name.HealthBar.Root).GetChild(ActorLayer.Name.HealthBar.Text).GetComponent<TextMeshPro>();      
        actionBarBack = front.GetChild(ActorLayer.Name.ActionBar.Root).GetChild(ActorLayer.Name.ActionBar.Back).GetComponent<SpriteRenderer>();
        actionBarDrain = front.GetChild(ActorLayer.Name.ActionBar.Root).GetChild(ActorLayer.Name.ActionBar.Drain).GetComponent<SpriteRenderer>();
        actionBarFill = front.GetChild(ActorLayer.Name.ActionBar.Root).GetChild(ActorLayer.Name.ActionBar.Fill).GetComponent<SpriteRenderer>();
        actionBarText = front.GetChild(ActorLayer.Name.ActionBar.Root).GetChild(ActorLayer.Name.ActionBar.Text).GetComponent<TextMeshPro>();      
        mask = front.GetChild(ActorLayer.Name.Mask).GetComponent<SpriteMask>();
        radialBack = front.GetChild(ActorLayer.Name.RadialBack).GetComponent<SpriteRenderer>();
        radial = front.GetChild(ActorLayer.Name.RadialFill).GetComponent<SpriteRenderer>();
        radialText = front.GetChild(ActorLayer.Name.RadialText).GetComponent<TextMeshPro>();
        turnDelayText = front.GetChild(ActorLayer.Name.TurnDelayText).GetComponent<TextMeshPro>();
        nameTagText = front.GetChild(ActorLayer.Name.NameTagText).GetComponent<TextMeshPro>();
        weaponIcon = front.GetChild(ActorLayer.Name.WeaponIcon).GetComponent<SpriteRenderer>();
        armorNorth = front.GetChild(ActorLayer.Name.Armor.Root).GetChild(ActorLayer.Name.Armor.ArmorNorth).GetComponent<SpriteRenderer>();
        armorEast = front.GetChild(ActorLayer.Name.Armor.Root).GetChild(ActorLayer.Name.Armor.ArmorEast).GetComponent<SpriteRenderer>();
        armorSouth = front.GetChild(ActorLayer.Name.Armor.Root).GetChild(ActorLayer.Name.Armor.ArmorSouth).GetComponent<SpriteRenderer>();
        armorWest = front.GetChild(ActorLayer.Name.Armor.Root).GetChild(ActorLayer.Name.Armor.ArmorWest).GetComponent<SpriteRenderer>();
        overlay = front.GetChild(ActorLayer.Name.Overlay).GetComponent<SpriteRenderer>();
        selectionBox = front.GetChild(ActorLayer.Name.SelectionBox).GetComponent<SpriteRenderer>();

        back = instance.transform.GetChild(ActorLayer.Name.Back);
    }


    public void SetAlpha(float alpha)
    {
        SetOpaqueAlpha(alpha);
        SetQualityAlpha(alpha);
        SetGlowAlpha(alpha);
        SetParallaxAlpha(alpha);
        SetThumbnailAlpha(alpha);
        SetFrameAlpha(alpha);
        //statusIcon.color = new color(1, 1, 1, alpha);
        SetHealthBarAlpha(alpha);
        SetActionBarAlpha(alpha);
        SetRadialAlpha(alpha);
        SetTurnDelayTextAlpha(alpha);
        SetNameTagTextAlpha(alpha);
        SetWeaponIconAlpha(alpha);
        SetArmorAlpha(alpha);
        SetSelectionAlpha(alpha);
    }

    public void SetOpaqueAlpha(float alpha)
    {
        opaqueColor.a = Mathf.Clamp(alpha, 0, 1);
        opaque.color = opaqueColor;
    }

    public void SetQualityColor(Color color)
    {
        qualityColor = new Color(color.r, color.g, color.b, Mathf.Clamp(color.a, Opacity.Transparent, qualityAlphaMax));
        quality.color = qualityColor;
    }


    public void SetQualityAlpha(float alpha)
    {
        qualityColor.a = Mathf.Clamp(alpha, Opacity.Transparent, qualityAlphaMax);
        this.quality.color = qualityColor;
    }

    public void SetGlowColor(Color color)
    {
        glowColor = new Color(color.r, color.g, color.b, color.a);
        this.glow.color = glowColor;
    }

    public void SetGlowAlpha(float alpha)
    {
        glowColor.a = Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Percent50);
        this.glow.color = qualityColor;
    }

    public void SetGlowScale(Vector3 scale)
    {
        this.glow.transform.localScale = scale;
    }

    public void SetParallaxSprite(Sprite sprite)
    {
        parallax.sprite = sprite;
    }

    public void SetParallaxMaterial(Material material)
    {
        parallax.material = material;
    }

    public void SetParallaxAlpha(float alpha)
    {
        parallaxColor.a = Mathf.Clamp(alpha, Opacity.Transparent, parallaxAlphaMax);
        this.parallax.color = parallaxColor;
    }

    public void SetParallaxSpeed(float xScroll, float yScroll)
    {
        instance.StartCoroutine(UpdateParallaxSpeed("_XScroll", xScroll));
        instance.StartCoroutine(UpdateParallaxSpeed("_YScroll", yScroll));
    }

    private IEnumerator UpdateParallaxSpeed(string scrollProperty, float targetValue)
    {
        //Fetch the current value once at the start
        float currentValue = parallax.material.GetFloat(scrollProperty);
        float duration = 1f; //Adjust the duration for the transition
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration); //Ensure t stays between 0 and 1
            float newValue = Mathf.Lerp(currentValue, targetValue, t);
            parallax.material.SetFloat(scrollProperty, newValue);
            yield return Wait.OneTick(); //Custom Wait method
        }

        //Set the final value to ensure precision
        parallax.material.SetFloat(scrollProperty, targetValue);
    }

    public void SetThumbnailAlpha(float alpha)
    {
        thumbnailColor.a = Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Opaque);
        thumbnail.color = thumbnailColor;
    }

    public void SetThumbnailMaterial(Material material)
    {
        thumbnail.material = material;
    }

    public void SetThumbnailSprite(Sprite sprite)
    {
        thumbnail.sprite = sprite;
    }

    public void SetFrameAlpha(float alpha)
    {
        frameColor.a = Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Opaque);
        frame.color = frameColor;
    }

    public void SetFrameEnabled(bool isEnabled)
    {
        frame.enabled = isEnabled;
    }

    public void SetHealthBarAlpha(float alpha)
    {
        healthBarBack.color = new Color(1, 1, 1, Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Percent70));
        healthBarDrain.color = new Color(1, 0, 0, alpha);
        healthBarColor.a = alpha;
        healthBarFill.color = healthBarColor;
        healthBarText.color = new Color(1, 1, 1, alpha);
    }

    public void SetActionBarAlpha(float alpha)
    {
        actionBarBack.color = new Color(1, 1, 1, Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Percent70));
        actionBarDrain.color = new Color(1, 0, 0, alpha);
        actionBarColor.a = alpha;
        actionBarFill.color = actionBarColor;
        actionBarText.color = new Color(1, 1, 1, alpha);
    }

    public void SetRadialEnabled(bool isEnabled)
    {
        radialBack.enabled = isEnabled;
        radial.enabled = isEnabled;
        radialText.enabled = isEnabled;
    }

    public void SetRadialAlpha(float alpha)
    {
        radialBack.color = new Color(1, 1, 1, Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Percent70));
        radial.color = new Color(1, 1, 1, alpha);
        radialText.color = new Color(1, 1, 1, alpha);
    }

    //public void SetBloomColor(color color)
    //{
    //   bloomColor = color;
    //   this.bloom.color = color;

    //   var intensity = (bloomColor.r + bloomColor.g + bloomColor.b) / 3f;
    //   var factor = 1f / intensity;
    //   var emissionColor = new color(bloomColor.r * factor, bloomColor.g * factor, bloomColor.b * factor, bloomColor.a);
    //   this.bloom.material.SetColor("_EmissionColor", emissionColor);
    //}

    //public void SetBloomAlpha(float alpha)
    //{
    //   bloomColor.a = alpha;
    //   this.bloom.color = bloomColor;

    //   var intensity = (bloomColor.r + bloomColor.g + bloomColor.b) / 3f;
    //   var factor = 1f / intensity;
    //   var emissionColor = new color(bloomColor.r * factor, bloomColor.g * factor, bloomColor.b * factor, alpha);
    //   this.bloom.material.SetColor("_EmissionColor", emissionColor);
    //}

    public void SetFrameColor(Color color)
    {
        frameColor = color;
        this.frame.color = frameColor;
    }


    public void SetSelectionBoxEnabled(bool isEnabled = true)
    {
        selectionBox.enabled = isEnabled;
    }

    public void SetOverlayColor(Color color)
    {
        overlayColor = color;
        this.overlay.color = overlayColor;
    }


    public void SetTurnDelayFontSize(int key)
    {
        var fontSizeKeyValueMap = new Dictionary<int, float>() {
            { 9, 1.0000f },
            { 8, 1.3750f },
            { 7, 1.7500f },
            { 6, 2.1250f },
            { 5, 2.5000f },
            { 4, 2.8750f },
            { 3, 3.2500f },
            { 2, 3.6250f },
            { 1, 4.0000f },
        };

        turnDelayText.fontSize = key > 9 ? 1f : fontSizeKeyValueMap[key];
    }

    public void SetTurnDelayText(string text)
    {
        turnDelayText.text = text;
    }

    public void SetTurnDelayTextEnabled(bool isEnabled)
    {
        turnDelayText.enabled = isEnabled;
    }

    public void SetTurnDelayTextAlpha(float alpha)
    {
        turnDelayColor.a = Mathf.Clamp(alpha, Opacity.Transparent, Opacity.Opaque);
        turnDelayText.color = turnDelayColor;
    }

    public void SetTurnDelayTextColor(Color color)
    {
        turnDelayColor = color;
        turnDelayText.color = turnDelayColor;
    }

    public void SetNameTagText(string text)
    {
        nameTagText.text = text;
    }

    public void SetNameTagTextAlpha(float alpha)
    {
        nameTagText.color = new Color(1, 1, 1, alpha);
    }

    public void SetNameTagEnabled(bool isEnabled)
    {
        nameTagText.enabled = isEnabled;
    }

    public void SetHealthBarColor(Color color)
    {

        healthBarColor = color;
        healthBarFill.color = actionBarColor;
    }

    public void SetActionBarEnabled(bool isEnabled)
    {
        actionBarBack.enabled = isEnabled;
        actionBarFill.enabled = isEnabled;
    }

    public void SetActionBarColor(Color color)
    {
        actionBarColor = color;
        actionBarFill.color = actionBarColor;
    }




    public void SetWeaponIconAlpha(float alpha)
    {
        weaponIconColor = new Color(1, 1, 1, alpha);
        weaponIcon.color = weaponIconColor;
    }


    public void SetArmorAlpha(float alpha)
    {
        armorColor = new Color(1, 1, 1, alpha);
        armorNorth.color = armorColor;
        armorEast.color = armorColor;
        armorSouth.color = armorColor;
        armorWest.color = armorColor;
    }


    public void SetSelectionAlpha(float alpha)
    {
        selectionColor = new Color(1, 1, 1, alpha);
        selectionBox.color = new Color(1, 1, 1, alpha);
    }


    float timer = 0.0f;
    ActionBarColorCycle cycle = ActionBarColorCycle.Phase1;

    public void CycleActionBarColor()
    {
        const float duration = 0.2f;
        timer += Time.deltaTime / duration;

        switch (cycle)
        {
            case ActionBarColorCycle.Phase1: actionBarColor = ColorHelper.ActionBar.Yellow; break;
            case ActionBarColorCycle.Phase2: actionBarColor = ColorHelper.ActionBar.Pink; break;
            case ActionBarColorCycle.Phase3: actionBarColor = ColorHelper.ActionBar.White; break;
            case ActionBarColorCycle.Phase4: actionBarColor = ColorHelper.ActionBar.Blue; break;
        }

        if (timer >= 1f)
        {
            timer = 0f;
            cycle = cycle.Next();
        }

        actionBarFill.color = actionBarColor;
    }

}


public enum ActionBarColorCycle
{
    Phase1,
    Phase2,
    Phase3,
    Phase4
}

