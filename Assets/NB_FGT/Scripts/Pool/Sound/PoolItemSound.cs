using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NB_FGT.Assets;

public enum SoundType
{ 
    ATK,
    HIT,
    BLOCK,
    FOOT
}
public class PoolItemSound : PoolItemBase
{
    private AudioSource _audioSource;
    [SerializeField] private AssetsSoundSO _soundAssets;
    [SerializeField]private SoundType _type;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public override void Spawn()
    {
        //1.����������ʱ����������
        //2.����������Ὺʼ��ʱ��0.3s�����������
        PlaySound(); 
    }
    private void PlaySound()
    {
        _audioSource.clip=_soundAssets.GetAudioClip(_type);
        _audioSource.Play();
        StartRecycle();
    }
    private void StartRecycle()
    {
        TimeManager.MainInstance.TryGetOneTimer(0.3f,DisableSelf);
    }
    private void DisableSelf()
    {
        _audioSource.Stop();
        gameObject.SetActive(false);
    }
}
