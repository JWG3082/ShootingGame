using UnityEngine;
using System;
using Object = UnityEngine.Object;
using UnityEngine.Pool;

public class PoolManager<T> : IDisposable where T : MonoBehaviour
{
    // 풀 변수 선언
    private readonly IObjectPool<T> _pool;
    
    // 생성자 선언 (Constructor)
    public PoolManager(T prefab, int defualtCapacity = 5, int maxSize = 10)
    {
        // 풀 초기화
        _pool = new UnityEngine.Pool.ObjectPool<T>
        (
            createFunc: () => Object.Instantiate(prefab), // 풀에서 사용가능한 오브젝트가 없으면 호출됨
            actionOnGet: obj => obj.gameObject.SetActive(true), // 풀에서 오브젝트를 꺼낼때 호출됨
            actionOnRelease: obj => obj.gameObject.SetActive(false), // 풀에 환원할 때 호출됨
            actionOnDestroy: obj => Object.Destroy(obj.gameObject), // 풀의 최대 크기를 초과한 것을 삭제할 대 호출
            collectionCheck: true, // 같은 오브젝트를 중복 반납여부 확인
            defaultCapacity: defualtCapacity, // 초기 생성 갯수
            maxSize: maxSize
        );
    }
    
    // 외부에서 접근할 메서드 선언
    // 풀에서 가져오기
    public T Get() => _pool.Get();
    // 객체 반납
    public void Release(T obj) => _pool.Release(obj);

    // 메모리 해제 여부
    private bool _isDisposed = false;
    
    public void Dispose()
    {
        // 메모리 해제 처리
        if (_isDisposed)
        {
            return;
        }
        
        _pool.Clear();
        _isDisposed = true;
    }
}
