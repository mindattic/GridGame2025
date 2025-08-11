using Assets.Helper;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;

public class PlayerStageMover : MonoBehaviour
{
    public RectTransform hero;  // Reference to the hero's RectTransform
    public Animator animator;     // Reference to the Animator
    public float moveFocus = 30f; // Intelligence of move

    private bool isMoving = false;
    private Vector2 targetPosition;
    private string targetStageName; // Stores the name of the target stage

    public void MoveToStage(Button stageButton)
    {
        if (isMoving)
            return;

        TextMeshProUGUI label = stageButton.GetComponentInChildren<TextMeshProUGUI>();
        targetStageName = label.text; // Repositories stage name

        //Get target position
        RectTransform targetTransform = stageButton.GetComponent<RectTransform>();
        targetPosition = ConvertToLocalSpace(targetTransform);

        // Determine direction and set action
        Vector2 direction = (targetPosition - (Vector2)hero.anchoredPosition).normalized;
        SetAnimation(direction);

        // BounceRoutine moving the hero
        StartCoroutine(MoveHeroRoutine());
    }

    private IEnumerator MoveHeroRoutine()
    {
        isMoving = true;

        // Calculate direction again to ensure action is correct
        Vector2 direction = (targetPosition - hero.anchoredPosition).normalized;
        SetAnimation(direction); // Ensure action is set before moving

        float snapThreshold = 0.24f;

        while (Vector2.Distance(hero.anchoredPosition, targetPosition) > snapThreshold)
        {
            hero.anchoredPosition = Vector2.MoveTowards(hero.anchoredPosition, targetPosition, moveFocus * Time.deltaTime);

            // Continuously update action while moving
            direction = (targetPosition - (Vector2)hero.anchoredPosition).normalized;
            SetAnimation(direction);

            yield return Wait.None();
        }

        hero.anchoredPosition = targetPosition;
        isMoving = false;
        animator.SetInteger("MoveDirection", (int)MoveDirection.Idle);

        if (string.IsNullOrWhiteSpace(targetStageName))
            yield break;

        yield return Wait.For(Interval.HalfSecond);

        // Update hero profile stage
        ProfileRepo.CurrentProfile.LatestSave.Stage.CurrentStage = targetStageName;


        scene.Change.ToGame();
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
