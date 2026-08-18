using UnityEngine;

public class TitleVolumeBoard : MonoBehaviour
{
    [SerializeField] private TitleUIManager TitleUIManager;
    [SerializeField] private GameObject[] AudioUI;
    private RectTransform rect;

    private Vector2 targetPosition;
    private float hideTargetY;

    [Header("落下")]
    [SerializeField] private float fallSpeed = 1000f;

    [Header("揺れ")]
    [SerializeField] private float swingAngle = 10f;
    [SerializeField] private float swingSpeed = 8f;
    [SerializeField] private float damping = 2f;

    [Header("戻る")]
    [SerializeField] private float hideSpeed = 1000f;

    private bool isFalling = false;
    private bool isSwinging = false;
    private bool isHiding = false;

    private float swingTime = 0f;

    private void Awake()
    {
        foreach (GameObject obj in AudioUI)
        {
            obj.SetActive(false);
        }

        rect = GetComponent<RectTransform>();

        //位置を保存
        targetPosition = rect.anchoredPosition;
    }


    public void Show()
    {
        // 画面上から開始
        rect.anchoredPosition = targetPosition + Vector2.up * 800f;

        // 回転をリセット
        rect.localRotation = Quaternion.identity;

        // 落下開始
        isFalling = true;
        isSwinging = false;
        swingTime = 0f;
    }

    public void Hide()
    {
        isFalling = false;
        isSwinging = false;

        rect.localRotation = Quaternion.identity;

        hideTargetY = targetPosition.y + 800f;

        foreach (GameObject obj in AudioUI)
        {
            obj.SetActive(false);
        }

        isHiding = true;
    }

    private void Update()
    {
        // 落下
        if (isFalling)
        {
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition,targetPosition,fallSpeed * Time.deltaTime);
            // 到着
            if (rect.anchoredPosition == targetPosition)
            {
                isFalling = false;
                isSwinging = true;
                swingTime = 0f;
                foreach (GameObject obj in AudioUI)
                {
                    obj.SetActive(true);
                }
            }
        }

        // 揺れ
        if (isSwinging)
        {
            swingTime += Time.deltaTime;

            float angle = Mathf.Sin(swingTime * swingSpeed) * swingAngle * Mathf.Exp(-damping * swingTime);

            rect.localRotation = Quaternion.Euler(0f, 0f, angle);

            // 揺れ終了
            if (swingTime > 2.5f)
            {
                rect.localRotation = Quaternion.identity;
                isSwinging = false;
            }
        }

        if (isHiding)
        {
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition,new Vector2(rect.anchoredPosition.x, hideTargetY),hideSpeed * Time.deltaTime);

            if (rect.anchoredPosition.y == hideTargetY)
            {
                isHiding = false;
                TitleUIManager.BackHome();
            }
        }
    }
}