using Assets.Scripts.Models;
using UnityEngine;

public class ActorThumbnail
{
    //Quick Reference Properties
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    protected ActorRenderers render => instance.render;
    protected ActorStats stats => instance.stats;

    //Fields
    private ActorInstance instance;
    public Texture2D texture; //The full 1024x1024 texture
    public Sprite sprite;


    public void Initialize(ActorInstance parentInstance)
    {
        this.instance = parentInstance;
    }

    public void Generate(ThumbnailSettings other = null)
    {
        // Get the full texture from the resource manager.
        texture = resourceManager.Portrait(instance.character.ToString()).Value;

        // Retrieve thumbnail settings from entity.
        ThumbnailSettings settings;
        if (other == null)
            settings = ActorStore.instance.GetThumbnailSetting(instance.character);
        else
            settings = new ThumbnailSettings(other);

        // Calculate an initial centered offset.
        Vector2Int offset = new Vector2Int();
        offset.x = (texture.width - settings.Width) / 2;
        offset.y = (texture.height - settings.Height) / 2;

        // Apply the user-defined shift.
        offset.Shift(settings.OffsetX, settings.OffsetY);

        // Clamp values to ensure the Rect doesn't go out of bounds.
        offset.x = Mathf.Clamp(offset.x, 0, texture.width - settings.Width);
        offset.y = Mathf.Clamp(offset.y, 0, texture.height - settings.Height);

        // Define the portion to cut out.
        Rect rect = new Rect(offset.x, offset.y, settings.Width, settings.Height);

        // Create a sprite from the selected portion of the texture.
        var pivot = new Vector2(0.5f, 0.5f);
        sprite = Sprite.Create(texture, rect, pivot, 100f);

        // Select the sprite for the SpriteRenderer.
        render.thumbnail.sprite = sprite;
    }



}