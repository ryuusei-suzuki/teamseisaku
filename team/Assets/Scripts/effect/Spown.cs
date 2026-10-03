using UnityEngine;

public class Spown : MonoBehaviour
{
    [SerializeField] private ParticleEffect effectPrefab;
    [SerializeField] private AudioClip fireball;
    [SerializeField] private AudioClip rain;
    [SerializeField] private AudioClip wind;
    [SerializeField] private AudioClip heel;
    public void UseFireBall()
    {
        ParticleEffect effect = Instantiate(effectPrefab);
        AudioManager.Instance.PlaySE(fireball);
        effect.PlayEffect(0);
    }

    public void UseRain()
    {
        ParticleEffect effect = Instantiate(effectPrefab);
        AudioManager.Instance.PlaySE(rain);
        effect.PlayEffect(1);
    }

    public void UseWind()
    {
        ParticleEffect effect = Instantiate(effectPrefab);
        AudioManager.Instance.PlaySE(wind);
        effect.PlayEffect(2);
    }

    public void Useheel()
    {
        ParticleEffect effect = Instantiate(effectPrefab);
        AudioManager.Instance.PlaySE(heel);
        effect.PlayEffect(3);
    }
}
