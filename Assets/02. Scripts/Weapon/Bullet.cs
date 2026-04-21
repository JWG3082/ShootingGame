using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 변수명 작성
    private Rigidbody _rb;
    private TrailRenderer _trail;
    [SerializeField] 
    private float _force = 1200f;

    #region 유니티 콜백 함수

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _trail = GetComponent<TrailRenderer>();
    }
    #endregion

    #region MyRegion

    public void Fire(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
        
        // 물리 속성 초기화
        _rb.linearVelocity = _rb.angularVelocity = Vector3.zero;
        _rb.rotation = rot;
        
        // 트레일 렌더러 초기화
        _trail.Clear();
        
        // 뉴튼(N)
        _rb.AddRelativeForce(Vector3.forward * _force);
    }

    #endregion
}
