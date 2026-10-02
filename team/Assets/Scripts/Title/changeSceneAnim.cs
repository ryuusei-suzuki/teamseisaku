using UnityEngine;
using DG.Tweening;

public class ChangeSceneAnim : MonoBehaviour
{
    [SerializeField] private TitleManager titleManager;

    [SerializeField] private Transform triangleA;
    [SerializeField] private Transform triangleB;
    [SerializeField] private Transform triangleC;
    [SerializeField] private Transform triangleMask;
    [SerializeField] private GameObject blocker;
    void Start()
    {
        // ‰ŠúˆÊ’u‚ğİ’è
        triangleA.localPosition = new Vector3(-6000f, 0, 0f);
        triangleB.localPosition = new Vector3(-6000f, 0, 0f);
        triangleC.localPosition = new Vector3(-6000f, 0, 0f);
        triangleMask.localPosition = new Vector3(-2808, 0, 0f);
        blocker.SetActive(false);
    }
    public void ScreenTransition()
    {
        blocker.SetActive(true);   
        DOTween.Sequence()

            .Append(triangleA.DOLocalMoveX(41f, 3f)).SetEase(Ease.InOutCubic)

            .Join(
                triangleB
                    .DOLocalMoveX(41f, 3f).SetEase(Ease.InOutCubic)
                    .SetDelay(1f)
            )

            .Join(
                triangleC
                    .DOLocalMoveX(41f, 3f).SetEase(Ease.InOutCubic)
                    .SetDelay(1f)
            )

            .AppendCallback(() =>
            {
                titleManager.StartGame();
            });
    }
}