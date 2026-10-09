using UnityEngine;
using UnityEngine.Audio;

public class AudioSet : MonoBehaviour
{
    public static AudioSet Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    // ★ Sceneをまたいで保存する音量
    public float MasterVolume { get; private set; }
    public float BGMVolume { get; private set; }
    public float SEVolume { get; private set; }


    private void Awake()
    {
        // ★ シングルトン
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // ★ Sceneが変わっても残す
        DontDestroyOnLoad(gameObject);

        // 初期値
        MasterVolume = 20f;
        BGMVolume = 5f;
        SEVolume = 5f;

        // AudioMixerに反映
        audioMixer.SetFloat("Master", MasterVolume);
        audioMixer.SetFloat("BGM", BGMVolume);
        audioMixer.SetFloat("SE", SEVolume);
    }


    // Master
    public void SetMaster(float volume)
    {
        MasterVolume = volume;
        audioMixer.SetFloat("Master", volume);
    }


    // BGM
    public void SetBGM(float volume)
    {
        BGMVolume = volume;
        audioMixer.SetFloat("BGM", volume);
    }


    // SE
    public void SetSE(float volume)
    {
        SEVolume = volume;
        audioMixer.SetFloat("SE", volume);
    }
}