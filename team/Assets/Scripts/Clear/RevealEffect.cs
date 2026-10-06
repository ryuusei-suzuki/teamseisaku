using UnityEngine;
using DG.Tweening;

public class RevealEffect : MonoBehaviour
{
    [Header("黒い画面")]
    [SerializeField] private GameObject black;
    [SerializeField] private CanvasGroup blackCanvasGroup;

    [Header("中央の光")]
    [SerializeField] private GameObject Light;
    [SerializeField] private Transform lightTransform;
    [SerializeField] private CanvasGroup lightCanvasGroup;

    [Header("トンネル設定")]
    [SerializeField] private float lightFadeDuration = 0.5f;
    [SerializeField] private float lightStartScale = 0.5f;
    [SerializeField] private float lightExpandDuration = 3.0f;
    [SerializeField] private float lightEndScale = 25f;
    [SerializeField] private float exitScale = 40f;
    [SerializeField] private float exitDuration = 0.3f;

    [Header("外に出たときの白い光")]
    [SerializeField] private GameObject white;
    [SerializeField] private CanvasGroup whiteCanvasGroup;
    [SerializeField] private float whiteFlashDuration = 0.1f;

    [SerializeField] private float eyeAdaptDuration = 3.0f;

    [SerializeField] private Congratulation Congratulation;
    private void Awake()
    {
        gameObject.SetActive(true);
        black.SetActive(true);
        Light.SetActive(true);
        blackCanvasGroup.alpha = 1f;
        lightCanvasGroup.alpha = 0f;
        lightTransform.localScale = Vector3.zero;
        white.SetActive(true);
        whiteCanvasGroup.alpha = 0f;
    }


    private void Start()
    {
        Play();
    }


    public void Play()
    {
        black.SetActive(true);
        white.SetActive(true);

        blackCanvasGroup.alpha = 1f;

        lightCanvasGroup.alpha = 0f;
        lightTransform.localScale = Vector3.zero;

        whiteCanvasGroup.alpha = 0f;

        lightTransform.DOKill();
        lightCanvasGroup.DOKill();
        blackCanvasGroup.DOKill();
        whiteCanvasGroup.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(lightCanvasGroup.DOFade(1f,lightFadeDuration));

        sequence.Join(lightTransform.DOScale( lightStartScale, lightFadeDuration));

        sequence.Append(lightTransform.DOScale(lightEndScale,lightExpandDuration).SetEase(Ease.InCubic));

        sequence.Append(lightTransform.DOScale(exitScale, exitDuration).SetEase(Ease.OutQuad));

        sequence.Append(whiteCanvasGroup.DOFade( 1f,whiteFlashDuration ));

        sequence.AppendCallback(() =>
        {
            black.SetActive(false);
            lightTransform.localScale = Vector3.zero;lightCanvasGroup.alpha = 0f;
        });

        sequence.Append(whiteCanvasGroup.DOFade(0f,eyeAdaptDuration).SetEase(Ease.OutSine));
        sequence.OnComplete(() =>
        {
            white.SetActive(false);
            Congratulation.Play();

        });
    }
}