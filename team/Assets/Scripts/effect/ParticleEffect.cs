using UnityEngine;
using System.Collections;

public class ParticleEffect: MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;

    private Coroutine effectCoroutine;

    public void PlayEffect(int index)
    {
        if (index < 0 || index >= particles.Length) return;
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }
        gameObject.SetActive(true);
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        particles[index].Play();
        effectCoroutine = StartCoroutine(WaitForEffectEnd(particles[index]));
    }

    private IEnumerator WaitForEffectEnd(ParticleSystem particle)
    {
        yield return new WaitWhile(() => particle.IsAlive(true));

        gameObject.SetActive(false);
        effectCoroutine = null;
    }

}
