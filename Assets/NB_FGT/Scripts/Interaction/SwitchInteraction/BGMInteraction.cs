using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMInteraction : InteractionBehaviour
{

    private AudioSource _audioSource;
    public AudioClip[] _clips;
    private int _index = -1;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    protected override void Interaction()
    {
        if (GameInputManager.MainInstance.TakeOut)
        {
            _index++;
            if (_index == _clips.Length)
            {
                _index = 0;
            }
            //Debug.Log(_index);
            _audioSource.clip = _clips[_index];
            _audioSource.Play();
        }
    }
}
