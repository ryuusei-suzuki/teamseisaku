using UnityEngine;
using UnityEngine.Audio;

public class TitleSound : MonoBehaviour
{
    [SerializeField] private AudioClip titleBGMSound;
    [SerializeField] private AudioClip onclickSound;

    void Start()
    {
        AudioManager.Instance.PlayBGM(titleBGMSound);

    }
    public void OnClick()
    {
        AudioManager.Instance.PlaySE(onclickSound);
    }
}
