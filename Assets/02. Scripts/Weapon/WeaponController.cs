using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class WeaponController : MonoBehaviour
{
    // 총알 발싸 지점
    [SerializeField] private Transform _firePos;

    // 총알 프리팹
    [SerializeField] private GameObject _bulletPrefab;

    // 총알 소리
    [SerializeField] private AudioClip _fireSfx;

    // 연사 속도
    [SerializeField] private float _fireRate = 0.15f;

    // MuzzleFlash
    [SerializeField] private MeshRenderer _muzzleFlash;

    // 시네머신 사용하기
    [SerializeField] private CinemachineImpulseSource _fireImpulseSource;

    // 총쏠때 Emission 효과주기
    [SerializeField] private MeshRenderer _gunRenderer;

    [SerializeField] private LayerMask _fireMask;
    
    // 오디오 소스
    private AudioSource _audio;
    private float _nextFire;
    
    // 페비쵸비 이제 새로운 스크립트 제작으로 비활성화
    // [SerializeField] private Animator _phoebeAnimator;
    // private readonly int _hashFire = Animator.StringToHash("IsFire");

    #region 유니티 콜백 함수

    private void Start()
    {
        // 이 명령어는 연산 많이 먹음
        _firePos = GameObject.Find("FirePos").transform;
        // 디렉토리 찾는 방법으로 사용하는 방법
        //_firePos = transform.Find("Fire/FirePos").transform;
        // 이 방법은 컴포넌트가 많은 경우는 아주 안좋음
        //_firePos = GetComponentInChildren<Transform>();

        // muzzle Flash
        // 디렉토리 찾는 방법
        _muzzleFlash = transform.Find("FirePos/MuzzleFlash").GetComponent<MeshRenderer>();
        _muzzleFlash.enabled = false;
        // fire_pos 자식 컴포넌트에 있는 이야기.
        //_muzzleFlash = _firePos.GetComponentInChildren<MeshRenderer>();

        // 소리 
        _audio = GetComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.minDistance = 10.0f;
        _audio.maxDistance = 50.0f;
    }

    private void Update()
    {
        FireBullet();
        //Debug.DrawRay(_firePos.position, 10f * _firePos.forward, Color.green);
    }

    #endregion

    /*
     *  동기 방식(sync)
     *  함수 1(5초)
     *  함수 2(1초) 기다렸다 함
     *
     *  비동기 방식9 (async)
     *  함수 1(5초)
     *  함수 2(1초) 바로 시작함
     *
     *  Multi-Thread - 서버 프로그래밍할대 많이 알아야함.
     *  Thread 프로그래밍
     *  async / await / Task
     *  Co-routine (코루틴) - 언밀히 멀티스레드는 아니다. 멀티스레드처럼 사용할 수 있게 도와주는 방법
     */

    #region 총알 발사 로직

    private void FireBullet()
    {
        // Lagacy InputManager 사용한 방법
        // if (Input.GetMouseButtonDown(0))
        // {
        //     // 총 발사 로직
        //     Instantiate(_bulletPrefab, _firePos.position, _firePos.rotation);
        // }

        // New InputSystem 사용한 방법
        if (Mouse.current.leftButton.isPressed)
            if (Time.time > _nextFire)
            {
                _nextFire = Time.time + _fireRate;
                //Instantiate(_bulletPrefab, _firePos.position, _firePos.rotation);
                
                // 풀에서 사용가능한 총알을 꺼내오기
                //var bullet = BulletPool.Instance.Get();
                //bullet.Fire(_firePos.position, _firePos.rotation);
                
                //GameObject bullet = ObjectPool._instance.SetActiveOBJ();
                //bullet.transform.position = _firePos.position;
                //bullet.transform.rotation = _firePos.rotation;
                //Debug.Log(bullet.transform.rotation.eulerAngles);
                // 여기서 Impulse 사용
                _fireImpulseSource.GenerateImpulse();
                // 음원 재생
                // _audio.Play("이름");
                // _audioSource.PlayOneShot(AudioClip, 볼륨);
                _audio.PlayOneShot(_fireSfx, 0.8f);
                // 총구 화염 효과
                StartCoroutine(ShowMuzzleFlash());
                if (Physics.Raycast(_firePos.position, _firePos.forward, out RaycastHit hit, 10.0f,  _fireMask))
                {
                    //Debug.Log($"hit : {hit.collider.name}");
                    hit.collider.GetComponent<IDamagable>()?.Damage(25f);
                }
            }
    }

    #endregion
    
    #region 총알 발사 효과

    private IEnumerator ShowMuzzleFlash()
    {
        // 텍스쳐 오프셋 변경
        // Random.Range(정수, 정수)     Random.Range(1,10)  1~9
        // Random.Range(실수, 실수)     Random.Range(1.0f,10.0f) 1.0f~10.0f
        // (0, 0) (0.5, 0) (0.5, 0.5) (0.5, 0) 
        var offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        _muzzleFlash.material.mainTextureOffset = offset;
        // 크기 조절
        var scale = Random.Range(1.2f, 2.5f);
        //_muzzleFlash.transform.localScale = new Vector3(scale, scale, scale);
        _muzzleFlash.transform.localScale = Vector3.one * scale;

        // 회전 각도 조절
        float angle = Random.Range(0, 360);
        _muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);
        _muzzleFlash.enabled = true;

        // phoebeChubi 비활성화
        // 새로 만든 페비 애니메이션 재 설정
        PheobeAnimator._instance.TriggerAnim(PheobeAnimator._instance._hashFire);
        //_phoebeAnimator.SetTrigger(_hashFire);

        // 총 Emission 추가
        SetGunEmission(20f);

        // Waiting
        yield return new WaitForSeconds(0.2f);
        _muzzleFlash.enabled = false;
        SetGunEmission(1f);
    }


    private void SetGunEmission(float value)
    {
        var mat = _gunRenderer.material;
        mat.EnableKeyword("EMISSION");
        mat.SetColor("_EmissionColor", Color.white * value);
    }

    #endregion
}