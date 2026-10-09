using UnityEngine;

public class BattleSound : MonoBehaviour
{
    [SerializeField] private AudioClip BattleBGMSound;
    [SerializeField] private AudioClip BattleonclickSound;

    void Start()
    {
        AudioManager.Instance.PlayBGM(BattleBGMSound);

    }
    public void OnClick()
    {
        AudioManager.Instance.PlaySE(BattleonclickSound);
    }
}
