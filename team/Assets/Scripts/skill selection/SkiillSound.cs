using UnityEngine;

public class SkiillSound : MonoBehaviour
{
    [SerializeField] private AudioClip skillsereBGMSound;
    [SerializeField] private AudioClip skillsereonclickSound;

    void Start()
    {
        AudioManager.Instance.PlayBGM(skillsereBGMSound);

    }
    public void OnClick()
    {
        AudioManager.Instance.PlaySE(skillsereonclickSound);
    }
}
