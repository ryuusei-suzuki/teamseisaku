using UnityEngine;
using DG.Tweening;

public class ChangeSceneAnim : MonoBehaviour
{
    [SerializeField] private TitleManager titleManager;

    [SerializeField] private Transform triangleA;
    [SerializeField] private Transform triangleB;
    [SerializeField] private Transform triangleC;
    [SerializeField] private Transform triangleMask;

    void Start()
    {
        // ‰ŠúˆÊ’u‚ğİ’è
        triangleA.localPosition = new Vector3(-25f,0, 0f);
        triangleB.localPosition = new Vector3(-25f,0, 0f);
        triangleC.localPosition = new Vector3(-25f, 0, 0f);
        triangleMask.localPosition = new Vector3(-25f, 0, 0f);
    }
    public void ScreenTransition()
    {
        DOTween.Sequence()

            .Append(triangleA.DOLocalMoveX(15f, 2f))

            .Join(
                triangleB
                    .DOLocalMoveX(15f, 2f)
                    .SetDelay(0.3f)
            )

            .Join(
                triangleC
                    .DOLocalMoveX(0.5f, 2f)
                    .SetDelay(0.4f)
            )

            .AppendCallback(() =>
            {
                titleManager.StartGame();
            });
    }
}