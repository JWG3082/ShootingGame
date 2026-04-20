using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Barrel : MonoBehaviour
{
   // 예습용 코드
   // private Rigidbody _rb;
   // [SerializeField] 
   // private GameObject _ExplosionParticle;
   
   // 새로 배우는 폭발 시스템
   // [SerializeField]  // 폭팝 효과 이펙트 
   // private GameObject _expEffect;
   [SerializeField] private BarrelDataSO _barrelDataSo;
   private const string TAG_BULLET = "BULLET";
   public int _hitCount;
   // 리지드바디와 오디오소스 이거는 프라이빗 처리해야함 = 컴포넌트 캐싱
   private Rigidbody _rb;
   private AudioSource _audio;
   private CinemachineImpulseSource _impulseSource;

   // 내가 자체적 추가 변수
   private MeshRenderer _renderer;
   private float addColorSet = 25f;

   # region 유니티 콜백 함수
   
   private void Start()
   {
      _rb = GetComponent<Rigidbody>();
      _audio = GetComponent<AudioSource>();
      _impulseSource = GetComponent<CinemachineImpulseSource>();
      // 내가 따로 추가하는 코드
      _renderer = GetComponent<MeshRenderer>();
   }
   
   private void OnCollisionEnter(Collision coll)
   {
      if (coll.collider.CompareTag(TAG_BULLET))
      {
         // 데미지 증가
         //_hitCount++;
         // 내가 직접 설정하는 머테리얼 효과
         if (++_hitCount >= 3)
         {
            ExpBarrel();
         }
         // 폭발 부과효과 메소드
         SetEmmision();
         //Invoke("Explode", 0.1f);
      }
   }
   
   //
   // void Explode()
   // {
   //    _audio.PlayOneShot(_clip, 1f);
   //    // 이렇게 해야 에러 코드 안뜹니다. 바로 만들시 어느거 파괴할지 모름.
   //    GameObject explosion = Instantiate(_ExplosionParticle, transform.position, transform.rotation);
   //    _rb.AddExplosionForce(1000, this.transform.position, 10,10);
   //    
   //    _renderer.enabled = false;
   //    Destroy(explosion, 1.5F);
   //    Destroy(this.gameObject, 1.6f);
   // }
   //

   # endregion

   #region 폭발 메소드

   private void ExpBarrel()
   {
      // 폭발 원점
      Vector3 expPos = transform.position + (Random.insideUnitSphere * 2.0f);
      
      // 주변 배럴에 폭발력 전달
      Collider[] barrels = Physics.OverlapSphere(expPos, _barrelDataSo.radius, _barrelDataSo.layerMask);   // 2^8 = 256

      foreach (Collider coll in barrels)
      {
         // 강사님 코드
         var rb = coll.GetComponent<Rigidbody>();
         var barrel = coll.GetComponent<Barrel>();
         // 폭발효과
         // RigidBody.AddExplosionForce(폭발력 폭발원점 반경 위로 올라가는힘)
         barrel._hitCount++;
         barrel.SetEmmision();
         rb.mass = 1.0f;
         rb.AddExplosionForce(_barrelDataSo.force,  expPos, _barrelDataSo.radius, _barrelDataSo.upwardForce);
         //barrel.Explode(expPos);
      }
      Explode(expPos);
   }
   
   #endregion

   #region 폭발 부가효과 메소드

   public void Explode(Vector3 expPos)
   {
      // 폭발 효과
      // Rigidbody.AddExplosionForce(폭발력, 폭발원점, 반경, 위로솟구치는힘)
      _rb.mass = 1f;
      _rb.AddExplosionForce(_barrelDataSo.force, expPos, _barrelDataSo.radius, _barrelDataSo.upwardForce);
      
      // 페비 애니메이션
      PheobeAnimator._instance.TriggerAnim(PheobeAnimator._instance._hashExplosion);
      
      // 폭발 파티클 효과
      GameObject effect = Instantiate(_barrelDataSo.expEffect, transform.position, transform.rotation);
      Destroy(effect, 5f);
      
      // 폭발 사운드 재생
      _audio.PlayOneShot(_barrelDataSo.expSfx, 1f);
      // 충격시 시네머신 효과
      _impulseSource.GenerateImpulse();
      
      // 드럼통 소멸처리
      Destroy(gameObject, 3f);
   }
   
   public void SetEmmision()
   {
      addColorSet += 20f;
      Material barrelMaterial = _renderer.material;
      barrelMaterial.EnableKeyword("_EMISSION");
      barrelMaterial.SetColor("_EmissionColor", Color.white * addColorSet);
   }

   #endregion
}