using UnityEngine;
using DG.Tweening;
public class Starteffct : MonoBehaviour
{
    [SerializeField] private GameObject triangle;
    [SerializeField] private Transform transtriangle;
    [SerializeField] private GameObject square;
    [SerializeField] private TextWriter textWriter;
    [SerializeField] private GameObject EscExit;

    void Awake()
    {
        square.SetActive(true);
        triangle.SetActive(true);
        // ‰ŠúˆÊ’u‚ðÝ’è
        transtriangle.localPosition = new Vector3(-38f, 0, 0f);
    }
    void Start()
    {
        ScreenTransition();
    }
    public void ScreenTransition()
    {
        DOTween.Sequence()
            .Append(transtriangle.DOLocalMoveX(-5f, 3f).SetEase(Ease.InOutCubic))
            .AppendCallback(() =>
            {
                triangle.SetActive(false);
                square.SetActive(false);
                EscExit.SetActive(true);
                textWriter.effectfin();
            });
    }
}