using System;
using UnityEngine;

public class EsterEgg : MonoBehaviour
{
    private AudioSource _source;
    [SerializeField]
    private AudioClip _clip;
    public delegate void EsterEggEvent(Animator animator, int hash);
    public static EsterEggEvent _eggEvent;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _eggEvent += DanceRevolution;
    }

    void DanceRevolution(Animator animator, int hash)
    {
        animator.SetTrigger(hash);
        _source.PlayOneShot(_clip, 1f);
    }
}