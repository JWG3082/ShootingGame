using System;
using UnityEngine;

public class PheobeAnimator : MonoBehaviour
{
    // 싱글톤
    public static PheobeAnimator _instance;
    // 애니메이션 세팅
    [HideInInspector]
    public Animator _animator;
    public readonly int _hashHit = Animator.StringToHash("IsHit") ;
    public readonly int _hashRun = Animator.StringToHash("IsRun") ;
    public readonly int _hashFire = Animator.StringToHash("IsFire") ;
    public readonly int _hashExplosion = Animator.StringToHash("IsExplosion");
    public readonly int _hashDance = Animator.StringToHash("IsDance") ;
    
    #region 유니티 콜백 함수

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        _animator = GetComponent<Animator>();
    }

    #endregion

    #region PhoebeChobi 애니메이션 코드
    
    public void BoolAnim(int animName, bool isActive)
    {
        _animator.SetBool(animName, isActive);
    }

    public void TriggerAnim(int animName)
    {
        _animator.SetTrigger(animName);
    }
    #endregion
}
