using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFX
{
    void Play();
}
public class CharacterHitFXHelper : MonoBehaviour, IFX
{
    private ParticleSystem _particle;
    private void Start()
    {
        _particle = GetComponentInChildren<ParticleSystem>();
    }
    public void Play()
    {
        _particle.Play();
    }
}
