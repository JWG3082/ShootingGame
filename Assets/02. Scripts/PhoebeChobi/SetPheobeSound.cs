using System;
using UnityEngine;

public class SetPheobeSound : MonoBehaviour
{
    private AudioSource _source;
    // 사운드 클립 0. Idle 1. Sad 2. Gun 3. Run
    [SerializeField] 
    private AudioClip[] _clips; 
    
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }
    
    public void IdleSound()
    {
        _source.PlayOneShot(_clips[0],1f);
    }

    public void SadSound()
    {
        _source.PlayOneShot(_clips[1],1f);
    }

    public void GunSound()
    {
        _source.PlayOneShot(_clips[2],1f);
    }

    public void RunSound()
    {
        _source.PlayOneShot(_clips[3],1f);
    }

    public void ExplodeSound()
    {
        _source.PlayOneShot(_clips[4],1f);
    }
}
