using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Repositories;
using TMPro;
using UnityEngine.EventSystems;
using game = GameManagerHelper;
public class PlayerStageMover : MonoBehaviour
{
    public RectTransform hero;  // Reference to the hero's RectTransform
    public Animator animator;     // Reference to the Animator
    public float moveFocus = 30f; // Intelligence of move

    private bool isMoving = false;
    private Vector2 targetPosition;
    private string targetStageName; // Stores the name of the target stage
    private FadeInstance fade; // Reference to fade manager

  
    private void Start()
    {
        fade = GameObject.Find(GameObjectHelper.Overworld.Fade).GetComponent<FadeInstance>();
    }

    public void MoveToStage(Button stageButton)
    {
        if (isMoving) 
            return;

        TextMeshProUGUI label = stageButton.GetComponentInChildren<TextMeshProUGUI>();
        targetStageName = label.text; // Repositories stage name

        //Get target position
        RectTransform targetTransform = stageButton.GetComponent<RectTransform>();
        targetPosition = ConvertToLocalSpace(targetTransform);

        // Determine direction and set animate
        Vector2 direction = (targetPosition - (Vector2)hero.anchoredPosition).normalized;
        SetAnimation(direction);

        // Start moving the hero
        StartCoroutine(MoveHero());
    }

    private IEnumerator MoveHero()
    {
        isMoving = true;

        // Calculate direction again to ensure animate is correct
        Vector2 direction = (targetPosition - hero.anchoredPosition).normalized;
        SetAnimation(direction); // Ensure animate is set before moving

        float snapThreshold = 0.24f;

        while (Vector2.Distance(hero.anchoredPosition, targetPosition) > snapThreshold)
        {
            hero.anchoredPosition = Vector2.MoveTowards(hero.anchoredPosition, targetPosition, moveFocus * Time.deltaTime);

            // Continuously update animate while moving
            direction = (targetPosition - (Vector2)hero.anchoredPosition).normalized;
            SetAnimation(direction);

            yield return null;
        }

        hero.anchoredPosition = targetPosition;
        isMoving = false;
        animator.SetInteger("MoveDirection", (int)MoveDirection.Idle);

        if (string.IsNullOrWhiteSpace(targetStageName))
            yield break;

        yield return Wait.For(Interval.HalfSecond);

        // Update hero profile stage
        ProfileRepo.CurrentProfile.LatestSave.Stage.CurrentStage = targetStageName;

        // FadeInstance out & load next scene
        StartCoroutine(fade.FadeOut(SceneRepo.LoadScene(SceneHelper.Game)));
    }

    private void SetAnimation(Vector2 direction)
    {
        MoveDirection moveDirection = MoveDirection.Idle; // Default to Idle

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
                moveDirection = MoveDirection.Right;
            else
                moveDirection = MoveDirection.Left;
        }
        else
        {
            if (direction.y > 0)
                moveDirection = MoveDirection.Up;
            else
                moveDirection = MoveDirection.Down;
        }

        animator.SetInteger("MoveDirection", (int)moveDirection);
    }

    private Vector2 ConvertToLocalSpace(RectTransform buttonTransform)
    {
        Vector2 localPoint;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, buttonTransform.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            hero.parent as RectTransform, screenPos, null, out localPoint
        );

        return localPoint;
    }
}
