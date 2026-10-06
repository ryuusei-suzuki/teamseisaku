using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class GameOverData
{
    [TextArea(2, 5)]
    public string text;
}

public class GameOverTextWriter : MonoBehaviour
{
    [SerializeField] private GameOverData[] gameOverData;

    public GameOverText GameOverText;
    public GameObject text;

    [SerializeField] private GameObject Gemeoverobj;

    private int currentIndex = 0;
    private bool ONtext = false;

    [Header("位置設定")]
    [SerializeField] private float centerY = 0f;
    [SerializeField] private float topY = 300f;

    [Header("大きさ設定")]
    [SerializeField] private float centerScale = 1.0f;
    [SerializeField] private float topScale = 0.7f;

    [Header("アニメーション設定")]
    [SerializeField] private float moveDuration = 0.5f;

    [Header("スタート表示")]
    [SerializeField] private float fadeDuration = 2.0f;

    [Header("ボタンの拡大設定")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float hoverDuration = 0.2f;

    private RectTransform gameOverRect;
    private CanvasGroup canvasGroup;

    [SerializeField] private GameObject TitleButton;
    [SerializeField] private GameObject retrybutton;

    private void Awake()
    {
        text.SetActive(false);

        TitleButton.SetActive(false);
        retrybutton.SetActive(false);

        gameOverRect = Gemeoverobj.GetComponent<RectTransform>();

        // CanvasGroupを取得
        canvasGroup = Gemeoverobj.GetComponent<CanvasGroup>();

        // CanvasGroupがなければ自動追加
        if (canvasGroup == null)
        {
            canvasGroup = Gemeoverobj.AddComponent<CanvasGroup>();
        }

        // 最初は中央
        gameOverRect.anchoredPosition = new Vector2(
            gameOverRect.anchoredPosition.x,
            centerY
        );

        // 大きさは最初から通常サイズ
        gameOverRect.localScale = Vector3.one * centerScale;

        // 最初は透明
        canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        // GameOverObjを表示
        Gemeoverobj.SetActive(true);

        // 透明 → 徐々に表示
        canvasGroup
            .DOFade(1f, fadeDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // フェードインが完全に終わったらボタンを表示
                TitleButton.SetActive(true);
                retrybutton.SetActive(true);
            });
    }

    private void ShowGameOverText(int index)
    {
        if (index < 0 || index >= gameOverData.Length)
        {
            Debug.LogError(
                $"GameOverDataの範囲外です。index={index}, Size={gameOverData.Length}"
            );
            return;
        }

        GameOverText.DrawText(gameOverData[index].text);
    }

    private void SetPosition(bool move)
    {
        if (gameOverRect == null)
            return;

        float targetY = ONtext ? topY : centerY;
        float targetScale = ONtext ? topScale : centerScale;

        if (move)
        {
            DOTween.Kill(gameOverRect);

            Sequence sequence = DOTween.Sequence();

            sequence.Join(
                gameOverRect.DOAnchorPosY(
                    targetY,
                    moveDuration
                )
            );

            sequence.Join(
                gameOverRect.DOScale(
                    targetScale,
                    moveDuration
                )
            );

            sequence.SetEase(Ease.OutCubic);
        }
        else
        {
            gameOverRect.anchoredPosition = new Vector2(
                gameOverRect.anchoredPosition.x,
                targetY
            );

            gameOverRect.localScale = Vector3.one * targetScale;
        }
    }

    public void GotoTitle()
    {
        ONtext = true;

        text.SetActive(true);

        SetPosition(true);

        currentIndex = 0;
        ShowGameOverText(currentIndex);
    }

    public void backtoTitle()
    {
        ONtext = false;

        SetPosition(true);

        text.SetActive(false);
    }

    public void GotoBattle()
    {
        ONtext = true;

        text.SetActive(true);

        SetPosition(true);

        currentIndex = 1;
        ShowGameOverText(currentIndex);
    }

    public void backtoBattle()
    {
        ONtext = false;

        SetPosition(true);

        text.SetActive(false);
    }

    public void ButtonPointerEnter(GameObject button)
    {
        button.transform.DOKill();

        button.transform.DOScale(hoverScale, hoverDuration).SetEase(Ease.OutCubic);
    }

    public void ButtonPointerExit(GameObject button)
    {
        button.transform.DOKill();

        button.transform.DOScale(5f, hoverDuration).SetEase(Ease.OutCubic);
    }
}
