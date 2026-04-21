using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    private const string TAG_BULLET = "BULLET";

    // 스파크 파티클 프리팹 저장할 변수
    [SerializeField] protected GameObject _SparkEffect;

    #region 유니티 콜백 함수

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag(TAG_BULLET)) BulletPart(coll);
        // 이 스크립트 삭제
        //Destroy(this);
        // 이 게임 오브젝트 삭제
        //Destroy(this.gameObject);
    }

    #endregion

    protected void BulletPart(Collision coll)
    {
        // 충돌 정보를 추출
        // coll.GetContent = Gets the contact point at the specified index 즉 특수한 번호의 콜라이더의 충돌지점 알아주는 
        // 명령어.
        // 충돌 정보를 추출
        var cp = coll.GetContact(0);
        // 충돌 좌표
        var point = cp.point;
        // 법선 백터 - 충돌지점 바로 위에 맞춰야함
        var normal = -1.0f * cp.normal;
        // 법선 백터가 바로 보는 각도 산출 (Quaternion)
        var rot = Quaternion.LookRotation(normal);
        // 스파트 생성
        var spark = Instantiate(_SparkEffect, point, rot);
        Destroy(spark, 0.5f);

        // 충돌한 게임오브젝트
        //Destroy(coll.gameObject);
        
        // 풀에 환원하는 코드
        BulletPool.Instance.Return(coll.gameObject.GetComponent<Bullet>());
        //ObjectPool._instance.DisAbleBullet(coll.gameObject);
    }
}