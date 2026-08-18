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
        //Master
        audioMixer.GetFloat("Master", out float masterVolume);
        MasterSlider.value = masterVolume;
        //BGM
        audioMixer.GetFloat("BGM", out float bgmVolume);
        BGMSlider.value = bgmVolume;
        //SE
        audioMixer.GetFloat("SE", out float seVolume);
        SESlider.value = seVolume;
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
