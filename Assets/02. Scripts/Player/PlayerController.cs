using System;
using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    // 이재현 강사님 코드
    private float _v;
    private float _h;
    private float _r;
    // 이동 속도 변수
    [SerializeField] 
    float _moveSpeed = 8.0f;
    [SerializeField]
    float _fastSpeed = 8.0f;
    float _currentSpeed;
    // 캐릭터 회전 속도
    [SerializeField]
    private float _rotateSpeed = 200f;
    // 애니메이션 변수
    private Animator _animator;
    // 애니메이션 파라미터 해시값 미리 추출(최적화를 위해) 안그러면 맨날맨날 계산해야함 그래서 readonly 쓰는것으로 암
    private readonly int _hashSpeed = Animator.StringToHash("Speed");
    private readonly int _hashStrafe = Animator.StringToHash("Strafe");
    // 내가 만드는 춤 관련 로직
    private readonly int _hashDance = Animator.StringToHash("IsDance");

    private float _initHP = 100.0f;
    private float _currHP = 100.0f;
    
    // 옵저버 패턴
    // 델리게이트 (Delegate): 대리자, 함수를 저장하기 위한 데이터를 정의
    // int hp
    // public void Sum(int a, int b) {return a+ b}
    // 델리게이트 변수명 = Sum;
    // 델리게이트 SumDelegate = Sum;
    
    // 델리게이트 선언
    // 1. public delegate 함수원형;
    // public delegate void PlayerDieHandler();
    // 델리게이트 정의
    // public static event PlayerDieHandler OnPlayerDead;
    
    // 2. Action : NET 미리 정의된 델리게이트
    // public static event Action<T1, T2, ... T16> ActionMethod;
    // Action은 애초에 델리게이트에서 만들어진 명령어
    public static event Action OnPlayerDead;
    // 아 더 공부하고싶다.
    
    // 춤 관련해서 넣을 음원 소스
    // private AudioSource _source;
    // [SerializeField]
    // private AudioClip _danceClip;
    
    // 예습코드 보통은 오브젝트 풀링 쓰지만 그냥 예습차원에서 걍 만들어 볼거임
    // public GameObject _bulletPrefab;
    // public Transform _target_pos;
    // bool isGameOver = false;
    // public ParticleSystem _muzzleFlash;
    
    // 내가 따로 추가한 애니메이터 이제 이거는 비활성화.
    // public Animator _phoebeAnim;
    // private readonly int _hashHit = Animator.StringToHash("IsHit");
    // private readonly int _hashFire = Animator.StringToHash("IsFire");
    // private readonly int _hashRun = Animator.StringToHash("IsRun");
    
    // 내가 다시 설정하는 델리게이트 애니메이션 테스트
    //public static event Action OnPlayerDance;
    
    [SerializeField] Volume _volume;
    Vignette _vignette;
    // 시네머신 부딪칠때 활용하기
    [SerializeField]
    private CinemachineImpulseSource  _impulseSource;
    
    // shift 하고 키보드값은 항상 같이 붙어야 움직이게 작동
    private bool isFaster = false;
    
    #region 유니티 콜백 메소드
    
    // 원래 나는 Awake에서 넣었는데 상관은 없지만 강사님 따라 할려고 주석화 함
    // private void Awake()
    // {
    //     //animator = GetComponent<Animator>();
    // }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        //_source = GetComponent<AudioSource>();
        //StartCoroutine(FireGun());
        //_impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    
    void Update()
    {
        // 새로 이재현 강사님이 말아주는 코드
        InputHandler();
        Movement();
        Animate();
        // // 예습용 코드 이거는 내가 방법 까먹어서 AI에게 도움 요청함 부끄러움...
        // float horizontal = Input.GetAxis("Horizontal");
        // float vertical = Input.GetAxis("Vertical");
        //
        // Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        // if (Input.GetKey(KeyCode.LeftShift))
        // {
        //     moveDirection *= moveSpeed;
        // }
        // transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
        //
        // animator.SetFloat("Strafe", horizontal);
        // animator.SetFloat("Speed", vertical);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            StartCoroutine(StartPhoebeAnim());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_currHP>0 && other.CompareTag("PUNCH"))
        {
            //Debug.Log(other.gameObject.name);
            PlayerDamaged(10);
        }
    }
    
    #endregion

    #region  입력처리
    // 새로 이재현 강사님이 말아주는 코드
    private void InputHandler()
    {
        // 이동 처리 입력값
        _v = Input.GetAxis("Vertical"); // -1.0f ~ 1.0f
        _h = Input.GetAxis("Horizontal"); // -1.0f ~ 1.0f
        // 회전 처리 입력값
        _r = Input.GetAxis("Mouse X");  // - / +
        //Debug.Log($"전후 = {v} / 좌우 {h}");
        if (_v != 0 || _h != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                isFaster = true;
                ActiveShift(_fastSpeed, 1.5f, isFaster);
            }
            else
            {
                isFaster = false;
                ActiveShift(_moveSpeed, 1f, isFaster);
            }
        }
        
        // 테스트로 만들어본 케이스 이거는 추후 연구 필요
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     this.gameObject.GetComponent<WeaponController>().enabled = false;
        //     PheobeAnimator._instance.TriggerAnim(PheobeAnimator._instance._hashDance);
        //     _animator.SetTrigger(_hashDance);
        //     Invoke("ActiveWeapon", 12f);
        //     _source.PlayOneShot(_danceClip);
        //     OnPlayerDance?.Invoke();
        // }
    }
    #endregion

    void ActiveWeapon()
    {
        this.gameObject.GetComponent<WeaponController>().enabled = true;
    }
    
    #region 이동 처리
    // 새로 이재현 강사님이 말아주는 코드
    private void Movement()
    {
        // 좌표 += 방향 * 속도 * 변위 * 시간보정표
        // transform.position += Vector3.forward * (v * 5.0f * Time.deltaTime);
        // transform.position += Vector3.back * (h * 5.0f * Time.deltaTime);
        
        // 방향 벡터
        // Vector3 moveDir = (전진후진 백터) + (좌우 벡터);
        Vector3 _moveDir = (Vector3.forward * _v) + (Vector3.right * _h);
        
        //Debug.Log($"정규화 이전 백터:{moveDir.magnitude}");
        //Debug.Log($"정규화 이후 백터:{moveDir.normalized.magnitude}");
        // 이동 처리
        transform.Translate(_moveDir.normalized * (_currentSpeed * Time.deltaTime));
        // 회전 처리
        // transform.Rotate(회전축 * 속도);
        transform.Rotate(Vector3.up *( _r * _rotateSpeed * Time.deltaTime));
    }   
    /* 정규화 백터, 단위 백터(unit vector)
     * vector3.forward = vector3(0,0,1)
     * vector3.right = vector3(1,0,0)
     * vector3.up = vector3(0,1,0)
     * 
     * vector3.one = vector3(1,1,1)
     * vector3.zero = vector3(0,0,0)
     */

    void ActiveShift(float f, float speed, bool isActive)
    {
        _currentSpeed = f;
        _animator.speed = speed;
        // 페비 애니메이터는 따로 제작
        PheobeAnimator._instance.BoolAnim(PheobeAnimator._instance._hashRun, isActive);
        //_phoebeAnim.SetBool(_hashRun, isActive);
    }
    
    #endregion

    #region 애니메이션 처리

    private void Animate()
    {
        _animator.SetFloat(_hashSpeed, _v);
        _animator.SetFloat(_hashStrafe, _h);
    }

    #endregion
    
    // 예습차원에서 만든 코드
    // private IEnumerator FireGun()
    // {
    //     while (!isGameOver)
    //     {
    //         if (Input.GetMouseButton(0))
    //         {
    //             Instantiate(_bulletPrefab, _target_pos.position, _target_pos.rotation);
    //             _phoebeAnim.SetBool(_hashFire, true);
    //             _muzzleFlash.Play();
    //             yield return new WaitForSeconds(0.1f);
    //         }
    //         else
    //         {
    //             _muzzleFlash.Stop();
    //             _phoebeAnim.SetBool(_hashFire, false);
    //             yield return null;
    //         }
    //     }
    // }
    
    IEnumerator StartPhoebeAnim()
    {
        // 새로 만든 페비 애니메이션 재 설정
        PheobeAnimator._instance.BoolAnim(PheobeAnimator._instance._hashHit, true);
        //_phoebeAnim.SetBool(_hashHit, true);
        if (_volume.profile.TryGet(out _vignette))
        {
            _vignette.intensity.value = 1.0f;
        }
        // 시네머신 효과 바로 쓰기
        _impulseSource.GenerateImpulse();
        yield return new WaitForSeconds(0.5f);
        if (_volume.profile.TryGet(out _vignette))
        {
            _vignette.intensity.value = 0.2f;
        }
        // 새로 만든 페비 애니메이션 재 설정
        PheobeAnimator._instance.BoolAnim(PheobeAnimator._instance._hashHit, false);
    }

    #region MyRegion
    private void PlayerDamaged(float i)
    {
        _currHP -= i;
        StartCoroutine(StartPhoebeAnim());
        if (_currHP <= 0)
        {
            // 주인공 사망 처리
            //PlayerDead();
            
            // 이벤트 발행(Event Raise)
            OnPlayerDead?.Invoke();
        }
    }
    #endregion

    // private void PlayerDead()
    // {
    //     GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
    //     
    //     foreach (var monster in monsters)
    //     {
    //         //monster.SendMessage("OnPlayerDead", SendMessageOptions.DontRequireReceiver);    
    //         monster.GetComponent<MonsterController>().OnPlayerDead();
    //     }
    //     this.gameObject.GetComponent<WeaponController>().enabled = false;
    // }
}
