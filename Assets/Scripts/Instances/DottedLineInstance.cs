using Game.Behaviors;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DottedLineInstance : MonoBehaviour
{

    protected Vector3 tileScale => GameManager.instance.tileScale;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected DottedLineManager dottedLineManager => GameManager.instance.dottedLineManager;
    protected LogManager logManager => GameManager.instance.logManager;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    protected bool hasSelectedPlayer => selectedPlayer != null;
   



    SpriteRenderer spriteRenderer;
    public Vector2Int location;
    public DottedLineSegment segment;
    //public bool isOccupied => hasSelectedPlayer && selectedPlayer.location == currentLocation;

    public Vector2Int top => location + new Vector2Int(0, -1);
    public Vector2Int right => location + new Vector2Int(1, 0);
    public Vector2Int bottom => location + new Vector2Int(0, 1);
    public Vector2Int left => location + new Vector2Int(-1, 0);

    public List<Vector2Int> connectedLocations = new List<Vector2Int>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();


    }

    private void Start()
    {

    }


    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    public Vector3 position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }

    public Quaternion rotation
    {
        get => gameObject.transform.rotation;
        set => gameObject.transform.rotation = value;
    }



    public Sprite sprite
    {
        get => spriteRenderer.sprite;
        set => spriteRenderer.sprite = value;
    }

    public void SetColor()
    {
        spriteRenderer.color = ColorHelper.Translucent.Yellow;
    }

    public void ResetColor()
    {
        spriteRenderer.color = ColorHelper.Translucent.White;
    }

    public void Spawn(DottedLineSegment segment, Vector2Int location)
    {
        this.segment = segment;
        this.location = location;
        this.position = Geometry.GetPositionByLocation(this.location);
        this.transform.localScale = tileScale;

        //Load resources
        var line = resourceManager.Sprite("DottedLine").Value;
        var turn = resourceManager.Sprite("DottedLineTurn").Value;
        var arrow = resourceManager.Sprite("DottedLineArrow").Value;

        connectedLocations.Clear(); //Ensure connectedLocations are reset
        connectedLocations.Add(location); //SpawnActor self-currentLocation to connectedLocations

        switch (this.segment)
        {
            case DottedLineSegment.Vertical:
                sprite = line;
                rotation = Quaternion.identity;
                connectedLocations.AddRange(new[] { top, bottom });
                break;

            case DottedLineSegment.Horizontal:
                sprite = line;
                rotation = Quaternion.Euler(0, 0, 90);
                connectedLocations.AddRange(new[] { left, right });
                break;

            case DottedLineSegment.TurnTopLeft:
                sprite = turn;
                rotation = Quaternion.Euler(0, 0, -180);
                connectedLocations.AddRange(new[] { top, left });
                break;

            case DottedLineSegment.TurnTopRight:
                sprite = turn;
                rotation = Quaternion.Euler(0, 0, 90);
                connectedLocations.AddRange(new[] { top, right });
                break;

            case DottedLineSegment.TurnBottomLeft:
                sprite = turn;
                rotation = Quaternion.Euler(0, 0, -90);
                connectedLocations.AddRange(new[] { bottom, left });
                break;

            case DottedLineSegment.TurnBottomRight:
                sprite = turn;
                rotation = Quaternion.identity;
                connectedLocations.AddRange(new[] { bottom, right });
                break;

            case DottedLineSegment.ArrowUp:
                sprite = arrow;
                rotation = Quaternion.identity;
                connectedLocations.Add(bottom);
                break;

            case DottedLineSegment.ArrowDown:
                sprite = arrow;
                rotation = Quaternion.Euler(0, 0, 180);
                connectedLocations.Add(top);
                break;

            case DottedLineSegment.ArrowLeft:
                sprite = arrow;
                rotation = Quaternion.Euler(0, 0, 90);
                connectedLocations.Add(right);
                break;

            case DottedLineSegment.ArrowRight:
                sprite = arrow;
                rotation = Quaternion.Euler(0, 0, -90);
                connectedLocations.Add(left);
                break;

            default:
                logManager.Warning($"Unhandled segment type: {segment}");
                break;
        }
    }

    public void Despawn()
    {
        dottedLineManager.Despawn(this);
    }
}


