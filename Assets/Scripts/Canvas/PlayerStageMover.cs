using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Repositories;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerStageMover : MonoBehaviour
{
    public RectTransform hero;  // Reference to the hero's RectTransform
    public Animator animator;     // Reference to the Animator
    public float moveSpeed = 30f; // Speed of movement

    private bool isMoving = false;
    private Vector2 targetPosition;
    private string targetStageName; // Stores the name of the target stage
    private FadeInstance fade; // Reference to fade manager

    private enum MoveDirection
    {
        Idle = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }

    private void Start()
    {
        fade = GameObject.Find(ComponentHelper.Overworld.Fade).GetComponent<FadeInstance>();
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

        // Determine direction and set animation
        Vector2 direction = (targetPosition - (Vector2)hero.anchoredPosition).normalized;
        SetAnimation(direction);

        // Start moving the hero
        StartCoroutine(MoveHero());
    }

    private IEnumerator MoveHero()
    {
        isMoving = true;

        // Calculate direction again to ensure animation is correct
        Vector2 direction = (targetPosition - hero.anchoredPosition).normalized;
        SetAnimation(direction); // Ensure animation is set before moving

        float snapThreshold = 0.24f;

        while (Vector2.Distance(hero.anchoredPosition, targetPosition) > snapThreshold)
        {
            hero.anchoredPosition = Vector2.MoveTowards(hero.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);

            // Continuously update animation while moving
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
        ProfileRepo.instance.CurrentProfile.LatestSave.Stage.CurrentStage = targetStageName;

        // FadeInstance out & load next scene
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.Game)));
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
