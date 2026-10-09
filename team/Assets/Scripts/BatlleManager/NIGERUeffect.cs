using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NIGERUeffect : MonoBehaviour
{
    [Header("右から左へ移動するオブジェクト")]
    [SerializeField] private Transform triangle;

    [Header("移動設定")]
    [SerializeField] private float startX = 6000f;
    [SerializeField] private float endX = -6000f;
    [SerializeField] private float moveDuration = 3f;
    public string backSceneName;
    public bool isTransitioning = false;

    void Start()
    {
        // 初期位置を右端に設定
        triangle.localPosition = new Vector3(startX, 0f, 0f);
        isTransitioning = false;
    }

    public void ScreenTransition()
    {
        if (isTransitioning) return; // すでに遷移中なら何もしない
        isTransitioning = true;
        triangle.DOLocalMoveX(endX, moveDuration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(backSceneName);
            });
    }
}
