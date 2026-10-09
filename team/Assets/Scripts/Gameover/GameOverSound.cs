using UnityEngine;

public class GameOverSound : MonoBehaviour
{
    [SerializeField] private AudioClip GameOverBGMSound;
    [SerializeField] private AudioClip GameOveronclickSound;

    void Start()
    {
        AudioManager.Instance.PlayBGM(GameOverBGMSound);
    }
    public void OnClick()
    {
        AudioManager.Instance.PlaySE(GameOveronclickSound);
    }
}
