using UnityEngine;

public class TutorialSoundSound : MonoBehaviour
{
    [SerializeField] private AudioClip TutorialSoundBGMSound;
    [SerializeField] private AudioClip TutorialSoundonclickSound;
    [SerializeField] private AudioClip TutorialSoundTTRSound;

    void Start()
    {
        AudioManager.Instance.PlayBGM(TutorialSoundBGMSound);

    }
    public void TTR()
    {
        AudioManager.Instance.PlaySE(TutorialSoundTTRSound);   
    }
    public void OnClick()
    {
        AudioManager.Instance.PlaySE(TutorialSoundonclickSound);
    }
}
