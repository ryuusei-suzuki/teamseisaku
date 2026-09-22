using UnityEngine;
using DG.Tweening;
public class ChangeAfterAnim : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform Mask;
    [SerializeField] private Transform Square;

    void Awake()
    {
        canvas.gameObject.SetActive(false);
        Mask.localScale = new Vector3(-50f, 0, 0);
        Square.localScale = new Vector3(0, 0, 0);
        Mask.gameObject.SetActive(false);
        Square.gameObject.SetActive(false);
    }
    void Start()
    {
        InScreenTransition();
    }

    public void InScreenTransition()
    {
        DOTween.Sequence()

            .Append(Mask.DOLocalMoveX(-5f, 2f))

            .AppendCallback(() =>
            {
                Mask.gameObject.SetActive(false);
                Square.gameObject.SetActive(false);
                canvas.gameObject.SetActive(true);
            });
    }
}
