using UnityEngine;

public class TutorialSoundSound : MonoBehaviour
{
    [SerializeField] private AudioClip TutorialSoundBGMSound;
    [SerializeField] private AudioClip TutorialSoundonclickSound;
    [SerializeField] private AudioClip TutorialSoundTTRSound;

    [Header("ëÆê´çUåÇSE")]
    [SerializeField] private AudioClip fireAttackSE;
    [SerializeField] private AudioClip waterAttackSE;
    [SerializeField] private AudioClip windAttackSE;
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

    public void PlayAttributeAttackSE(AttributeType attribute)
    {
        AudioClip sound = null;

        switch (attribute)
        {
            case AttributeType.Fire:
                sound = fireAttackSE;
                break;

            case AttributeType.Water:
                sound = waterAttackSE;
                break;

            case AttributeType.Wind:
                sound = windAttackSE;
                break;
        }

        if (sound != null)
        {
            AudioManager.Instance.PlaySE(sound);
        }
    }
}
