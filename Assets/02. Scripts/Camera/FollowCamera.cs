using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField] [Range(2f, 10f)] 
    private float _speed = 10f;
    [SerializeField] [Range(2f, 10f)]
    private float _distance = 10f;
    [SerializeField] [Range(-5f, 5f)]
    private float _height = 3.0f;

    // 사람 보는 위치로 바꾸는 
    [SerializeField] [Range(-5f, 5f)]
    private float _yOffset = 2.0f;
    
    #region 유니티 콜백 함수

    private void Start()
    {
        // 가급적이면 public 함수나 뭔가 연결해서 쓰는 방법을 쓰지 말기.
        _target = GameObject.FindGameObjectWithTag("PLAYER")?.transform;
        
        // 만약 타겟을 못찾을때 사용하는 방어로직
        if (_target == null)
        {
            Debug.LogError("No target found");
        }
    }

    private void LateUpdate()
    {
        // 위치를 올려주는 지역변수
        Vector3 offsetTarget = _target.position + Vector3.up * _yOffset;
        // 카메라 위치 알려주는 명령어 거리는 타겟 기준 위치는 글로벌 좌표(플레이어가 넘어질때)
        // 원래 값
        //Vector3 pos = _target.position - (_target.forward * _distance) + (Vector3.up * _height);
        // 조정값
        Vector3 pos = _target.position - (_target.forward * _distance) + (Vector3.up * _height);
        transform.position = pos;
        transform.LookAt(offsetTarget);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && this.enabled)
        {
            Gizmos.color = Color.red;
            // 기즈모 상태. 와이어 그려주는 부분.
            Gizmos.DrawWireSphere(_target.position, 0.3f);
            
            // y offset 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_target.position + Vector3.up * _yOffset, 0.3f);
            // 라인 그려주는 명령어.
            Gizmos.DrawLine(transform.position, _target.position);
        }
    }

    #endregion
}