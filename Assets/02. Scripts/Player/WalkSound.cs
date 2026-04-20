using System;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    AudioSource _source;
    [SerializeField] 
    AudioClip[] _clips;
    
    private void Awake()
    {
        _source = GetComponent<AudioSource>(); 
    }

    public void Walk()
    {
        _source.PlayOneShot(_clips[0]);
    }
}