using UnityEngine;

public class ClearSound : MonoBehaviour
{
    [SerializeField] private AudioClip ClearSoundBGMSound;

    void Start()
    {
        AudioManager.Instance.PlayBGM(ClearSoundBGMSound);

    }
}
