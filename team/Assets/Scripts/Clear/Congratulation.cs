using DG.Tweening;
using UnityEngine;

public class Congratulation : MonoBehaviour
{
    [Header("動かすオブジェクト")]
    [SerializeField] private RectTransform target;

    [Header("開始位置")]
    [SerializeField] private Vector2 startPosition = new Vector2(0f, 0f);

    [Header("上の位置")]
    [SerializeField] private Vector2 topPosition = new Vector2(0f, 300f);

    [Header("開始時の大きさ")]
    [SerializeField] private float startScale = 1.5f;

    [Header("通常の大きさ")]
    [SerializeField] private float normalScale = 1.0f;

    [Header("待つ時間")]
    [SerializeField] private float waitDuration = 2.0f;

    [Header("移動時間")]
    [SerializeField] private float moveDuration = 1.0f;


    private void Start()
    {
        target.anchoredPosition = startPosition;
        target.localScale = Vector3.one * startScale;
    }


    public void Play()
    {
        target.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(waitDuration);
        sequence.Append(target.DOAnchorPos(topPosition,moveDuration).SetEase(Ease.OutCubic));
        sequence.Join(target.DOScale(normalScale,moveDuration).SetEase(Ease.OutCubic));
    }
}

