using UnityEngine;

public class GameOverEffect : MonoBehaviour
{
    [Header("ˆÚ“®‚·‚éUI")]
    [SerializeField] private RectTransform moveObject;

    [Header("ˆÊ’u")]
    [SerializeField] private Vector2 hidePosition;
    [SerializeField] private Vector2 showPosition;

    [Header("‘å‚«‚³")]
    [SerializeField] private Vector2 hideScale;
    [SerializeField] private Vector2 showScale;

    [Header("ˆÚ“®‘¬“x")]
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] public GameOverTextWriter isDisplayed;

    private void Awake()
    {
        moveObject.anchoredPosition = hidePosition;
    }

    private void Update()
    {
        Vector2 targetPosition;
        Vector2 targetScale;

        if (isDisplayed.isDisplayed)
        {
            targetPosition = showPosition;
            targetScale = showScale;

        }
        else
        {
            targetPosition = hidePosition;
            targetScale = hideScale;
        }

        moveObject.anchoredPosition = Vector2.Lerp(moveObject.anchoredPosition,targetPosition,moveSpeed * Time.deltaTime);
        moveObject.localScale = Vector2.Lerp(moveObject.localScale, targetScale, moveSpeed * Time.deltaTime);
    }
}
