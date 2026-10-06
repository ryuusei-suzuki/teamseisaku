using TMPro;
using UnityEngine;

public class EXITWindow : MonoBehaviour
{
    private RectTransform rect;

    private Vector2 initPosition;
    private Vector2 targetPosition;

    [SerializeField] private float startOffset = 500f;
    [SerializeField] private float speed = 500f;
    [SerializeField] private TitleUIManager titleuimanager;
    [SerializeField] private GameObject blocker;
    private bool isSliding = false;
    private bool hide = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        // 本来の位置を保存
        initPosition = rect.anchoredPosition;
        blocker.SetActive(false);
    }

    public void Show()
    {
        blocker.SetActive(true);
        // 画面の下から開始
        rect.anchoredPosition = initPosition + Vector2.down * startOffset;
        //真ん中を目標にする
        targetPosition = initPosition;

        isSliding = true;
    }

    public void Hide()
    {
        // 本来の位置から開始
        rect.anchoredPosition = initPosition;

        // 下を目標にする
        targetPosition = initPosition + Vector2.down * startOffset;
        hide = true;
        isSliding = true;
        blocker.SetActive(false);
    }
    private void Update()
    {
        if (!isSliding)
            return;

        // 本来の位置まで移動
        rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition,targetPosition,speed * Time.deltaTime);
        // 到着したら終了
        if (rect.anchoredPosition == targetPosition && hide == true)
        {
            isSliding = false;
            titleuimanager.BackHome();
            hide = false;

        }
        else if (rect.anchoredPosition == initPosition)
        {
            isSliding = false;
        }

    }
}