using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BlackCover : MonoBehaviour
{
    [Header("黒いオブジェクト")]
    [SerializeField] private GameObject blackObj;

    [Header("位置")]
    [SerializeField] private float startY = -1200f;
    [SerializeField] private float endY = 0f;

    [Header("移動時間")]
    [SerializeField] private float moveDuration = 1.0f;

    private RectTransform blackRect;

    private void Awake()
    {
        blackRect = blackObj.GetComponent<RectTransform>();

        // 最初は非表示
        blackObj.SetActive(false);
    }
    public void ShowBlackCover()
    {
        // 呼び出されてから2秒待つ
        DOVirtual.DelayedCall(2f, () =>
        {
            blackObj.SetActive(true);
            blackRect.DOKill();

            blackRect.anchoredPosition = new Vector2(
                blackRect.anchoredPosition.x,
                startY
            );

            // 黒いカバーを移動
            blackRect
                .DOAnchorPosY(endY, moveDuration)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() =>
                {
                    // アニメーション終了後にシーン切り替え
                    SceneManager.LoadScene("GameOver");
                });
        });
    }
    public void HideBlackCover()
    {
        blackRect.DOKill();

        blackRect
            .DOAnchorPosY(startY, moveDuration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                blackObj.SetActive(false);
            });
    }
}