using Assets.Scripts.Models;
using UnityEngine;

public class ActorThumbnail
{
    //External properties
    protected DataManager dataManager => GameManager.instance.dataManager;
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

    public void Generate()
    {
        //Get the full texture from the resource manager
        texture = resourceManager.Portrait(instance.character.ToString()).Value;

        //Retrieve thumnail settings from entity
        ThumbnailSettings settings = dataManager.GetThumbnailSetting(instance.character);

        Vector2Int offset = new Vector2Int();
        offset.x = (texture.width - settings.Width) / 2;
        offset.y = texture.height - settings.Height;
        offset.Shift(settings.OffsetX, settings.OffsetY);

        //Clamp values to ensure the Rect doesn't go out of bounds
        offset.x = Mathf.Clamp(offset.x, 0, texture.width - settings.Width);
        offset.y = Mathf.Clamp(offset.y, 0, texture.height - settings.Height);

        //Define the portion to cut out
        Rect rect = new Rect(offset.x, offset.y, settings.Width, settings.Height);

        //Create a sprite from the selected portion of the texture
        var pivot = new Vector2(0.5f, 0.5f);
        sprite = Sprite.Create(texture, rect, pivot, 100f);

        //Assign the sprite to the SpriteRenderer
        render.thumbnail.sprite = sprite;
    }



}