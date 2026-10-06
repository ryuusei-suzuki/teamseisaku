using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class SlideObject : MonoBehaviour
{
    [Header("動かすオブジェクト")]
    [SerializeField] private GameObject Obj;

    [Header("移動設定")]
    [SerializeField] private float moveDuration = 2.0f;

    [Header("右端の位置")]
    [SerializeField] private float rightX = 1000f;

    [Header("左端の位置")]
    [SerializeField] private float leftX = -1000f;

    void Start()
    {
        Play();
    }
    public void Play()
    {
        Obj.transform.localPosition = new Vector3( rightX,Obj.transform.localPosition.y,Obj.transform.localPosition.z);
        Obj.transform.DOLocalMoveX(leftX,moveDuration).SetEase(Ease.Linear);
        SceneManager.LoadScene("GameClear");

    }
}
