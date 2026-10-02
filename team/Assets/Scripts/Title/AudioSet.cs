using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSet : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    [Header("スライダー")]
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    private void Start()
    {
        // 初期音量を設定
        SetInitialVolume("Master", MasterSlider, 5f);
        SetInitialVolume("BGM", BGMSlider, 5f);
        SetInitialVolume("SE", SESlider, 0f);
    }

    private void SetInitialVolume(string parameter, Slider slider, float volume)
    {
        // スライダーの位置を設定
        slider.SetValueWithoutNotify(volume);

        // AudioMixerの音量を設定
        audioMixer.SetFloat(parameter, volume);
    }

    public void SetMaster(float volume)
    {
        Debug.Log("Master Volume : " + volume);
        audioMixer.SetFloat("Master", volume);

    }
    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
    }
}
