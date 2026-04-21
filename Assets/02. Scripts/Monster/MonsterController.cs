using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
   // 몬스터 상태 정의 (Enum)
   public enum MonsterState
   {
      IDLE,
      TRACE,
      ATTACK,
      DEAD
   }
   // 몬스터의 상태를 저장하는 변수
   [SerializeField] 
   private MonsterState _MonsterState;
   
   // 추적 사정거리
   [SerializeField]
   private float _TraceDist = 10.0f;
   [SerializeField]
   private float _AttackDist = 2.0f;
   // 몬스터 사망 여부
   [SerializeField]
   private bool _isDead = false;
   // 몬스터의 HP
   [SerializeField]
   private float _hp = 100.0f;
   
   // 애니메이션 해시 함수
   private readonly int _hashIsTrace = Animator.StringToHash("IsTrace");
   private readonly int _hashIsAttack = Animator.StringToHash("IsAttack");
   private readonly int _hashDead = Animator.StringToHash("Dead");
   private readonly int _hashHit = Animator.StringToHash("HIT");
   // 내가 추가한 함수
   private readonly int _hashIsDance = Animator.StringToHash("IsDance");
   
   // 컴포넌트 캐싱
   private Transform _monsterTr;
   private Transform _playerTr;
   private NavMeshAgent _agent;
   private Animator _animator;
   // private Collider[] _colliders;
   private SkinnedMeshRenderer[] _renderer;
   private Material _material;
   
   // waitforseconds 캐싱
   private WaitForSeconds _ws;
   
   // 내가 다시 한번 추가한 변수
   // private bool _isEster = false;
   
   // 몬스터의 모든 콜라이더 저장할 컬랙션
   private List<Collider> _colliders = new List<Collider>(); // new();
   
   #region 유니티 콜백 함수

   void Awake()
   {
      _ws = new WaitForSeconds(0.3f);
      _monsterTr = GetComponent<Transform>();
      _playerTr = GameObject.FindGameObjectWithTag("PLAYER").transform;
      _agent = GetComponent<NavMeshAgent>();
      _animator = GetComponent<Animator>();
      //_colliders = GetComponentsInChildren<Collider>();
      _renderer = GetComponentsInChildren<SkinnedMeshRenderer>();
      
      // 콜라이더 값 미리 저장
      _monsterTr.GetComponentsInChildren<Collider>(_colliders);
   }
   
   private void OnEnable()
   {
      // 이벤트 불러오는 부분 (이벤트 구독(Subscribe))
      PlayerController.OnPlayerDead += this.OnPlayerDead;
      //PlayerController.OnPlayerDance += this.OnPlayerDance;
      StartCoroutine(CheckMonsterState());
      StartCoroutine(MonsterAction());
   }

   void OnDisable()
   {
      // 비활성화시 이벤트 종료 부분 (이벤트 구독 해지(UnSubscribe))
      PlayerController.OnPlayerDead -= this.OnPlayerDead;
      //PlayerController.OnPlayerDance -= this.OnPlayerDance;
   }
   
   private void OnCollisionEnter(Collision coll)
   {
      if (coll.collider.CompareTag("BULLET"))
      {
         Destroy(coll.gameObject);
         _animator.SetTrigger(_hashHit);
         _hp -= 10.0f;
         if (_hp <= 0.0f)
         {
            _MonsterState = MonsterState.DEAD;
         }
         else
         {
            StartCoroutine(CheckColorState());
         }
      }
   }
   
   #endregion
   
   private void ToggleColliders(bool active)
   {
      // TODO: 오류 수정
      foreach (var coll in _colliders)
      {
         coll.enabled = active;
      }
   }
   private IEnumerator CheckColorState()
   {
      for (int i = 0; i < _renderer.Length; i++)
      {
         _material = _renderer[i].material;
         _renderer[i].material.SetColor("_SpecColor", Color.red);
         yield return new WaitForSeconds(0.1f);
         _renderer[i].material.SetColor("_SpecColor", Color.gray);
      }
      yield break;
   }

   // 코루틴 1. 몬스터 상태를 경신(0.3f)
   private IEnumerator CheckMonsterState()
   {
      // while(_isDead == false)
      while (!_isDead)
      {
         if (_MonsterState == MonsterState.DEAD)
         {
            yield break;
         }
         // 반복되는 로직
         // player와 monster간의 거리 계산
         // float dist = Vector3.Distance(_monsterTr.position, _playerTr.position);
         // 백터의 뺄셈 연산 (A-B) --> A와 B간의 백터
         float dist = (_monsterTr.position - _playerTr.position).sqrMagnitude;
         // 5 5*5 = 25
         if (dist <= _AttackDist * _AttackDist) // 피타고라스 정리에 따라 제곱값으로 계산
         {
            _MonsterState = MonsterState.ATTACK;   // 공격 사정거리
         }
         else if (dist <= _TraceDist * _TraceDist) // 피타고라스 정리에 따라 제곱값으로 계산
         {
            _MonsterState = MonsterState.TRACE;    // 추적 사정거리
         }
         else
         {
            _MonsterState = MonsterState.IDLE;
         }
         yield return _ws;
      }   
   }
   
   // 코루틴 2 - 몬스터 상태에 따라서 행동
   private IEnumerator MonsterAction()
   {
      while (!_isDead)
      {
         switch (_MonsterState)
         {
            case MonsterState.IDLE:
               // 아이들 로직
               //Debug.Log("아이돌");
               _agent.isStopped = true;
               _animator.SetBool(_hashIsTrace, false);
               break;
            case MonsterState.TRACE:
               // 추적 로직
               //Debug.Log("트레이스");
               _agent.SetDestination(_playerTr.position);
               //_agent.destination = _playerTr.position;  이 명령어도 사용 가능.
               _agent.isStopped = false;
               _animator.SetBool(_hashIsTrace, true);    
               _animator.SetBool(_hashIsAttack, false);  // 이미 공격상태로 전이된 상태일 경우
               break;
            case MonsterState.ATTACK:
               // 공격 로직
               //Debug.Log("어택");
               _agent.isStopped = true;
               _animator.SetBool(_hashIsAttack, true);
               transform.LookAt(_playerTr);
               break;
            case MonsterState.DEAD:
               _isDead = true;
               _agent.isStopped = true;
               _animator.SetTrigger(_hashDead);
               ToggleColliders(false);
               for (int i = 0; i < _renderer.Length; i++)
               {
                  _material = _renderer[i].material;
                  _renderer[i].material.SetColor("_SpecColor", Color.red);
               }
               // 잠시 대기 후 처리
               yield return new WaitForSeconds(2f);
               // 원래 값 설정
               _hp = 100f;
               _MonsterState = MonsterState.IDLE;
               _isDead = false;
               ToggleColliders(true);
               // 오브젝트 풀링으로 변환
               MonsterPool.instance.pool.Release(this);
               break;
         }
         yield return _ws;
      }
   }

   #region 플레이어 죽었을때
   public void OnPlayerDead()
   {
      // 댄스 애니메이션 처리
      _animator.SetTrigger(_hashIsDance);
      // 네비 메시 정지
      _agent.isStopped = true;
      // 코루틴 정지
      StopAllCoroutines();
   }
   #endregion

   // public void OnPlayerDance()
   // {
   //    _animator.SetTrigger(_hashIsDance);
   //    _agent.isStopped = true;
   //    StartCoroutine(StartActDance());
   // }
   //
   // private IEnumerator StartActDance()
   // {
   //    yield return new WaitForSeconds(12f);
   //    _agent.isStopped = false;
   //    yield break;
   // }
}
