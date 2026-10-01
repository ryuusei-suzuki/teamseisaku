using UnityEngine;
using DG.Tweening;

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

    // 下から上へ黒い画面を出す
    public void ShowBlackCover()
    {
        blackObj.SetActive(true);

        // 現在のDOTweenを停止
        blackRect.DOKill();

        // 最初は画面の下
        blackRect.anchoredPosition = new Vector2(
            blackRect.anchoredPosition.x,
            startY
        );

        // 下から上へ移動
        blackRect
            .DOAnchorPosY(endY, moveDuration)
            .SetEase(Ease.InOutCubic);
    }

    // 黒い画面を下へ戻す
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