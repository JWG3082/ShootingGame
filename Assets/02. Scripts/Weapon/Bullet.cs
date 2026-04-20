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

    private void OnEnable()
    {
        // 혹시 모르니 다시 한번 재 초기화.
        _rb.linearVelocity= Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        // 뉴튼(n) 총이 나가지는 명령어
        _rb.AddRelativeForce(Vector3.forward * _force);
        // 예습 코드
        
        // 불렛 자체적인 시간 제한
        Invoke("DestroyBullet", 30f);
    }
    
    private void OnDisable()
    {
        // 초기화
        _rb.linearVelocity= Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _trail.Clear();
        CancelInvoke();
    }

    // 예습 코드
    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Wall"))
    //     {
    //         Destroy(gameObject, 1f);
    //     }
    // }
    
    #endregion

    #region MyRegion

    void DestroyBullet()
    {
        Destroy(gameObject);
        //ObjectPool._instance.DisAbleBullet(this.gameObject);
    }
    #endregion
}
