using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] private T _prefab;
    [SerializeField] private int _initialSize = 10;
    
    // 큐 자료형 (선입선출 FIFO)   <==> 스택 자료형 (후입선출 LIFO)
    private Queue<T> _pool = new Queue<T>();

    public virtual void Awake()
    {
        // 초기 풀 채우기 (초기 생성)
        for (int i = 0; i < _initialSize; i++)
        {
            // 생성
            T obj = Instantiate(_prefab);
            // 큐에 넣기 전에 비활서화
            obj.transform.SetParent(this.transform);
            obj.gameObject.SetActive(false); // Awake, OnEnable, OnDisable
            // 큐에 적재
            _pool.Enqueue(obj);
        }
    }
    
    // 풀에서 사용가능한 객체를 리턴하는 메서드
    public T Get()
    {
        // 풀 갯수가 남아있는지 확인
        if (_pool.Count > 0)
        {
            // 풀에서 가저오기
            T obj = _pool.Dequeue();
            // 넘겨주기 전에 활성화
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(null);
            return obj;
        }
        // _pool 이 비어있으면 미리 만들어둔 객체를 다 사용했음
        // 풀이 비어있으면 새로 생성
        return Instantiate(_prefab);
    }

    public void Return(T obj)
    {
        // 비활성화 시킨 후 재활용 처리
        obj.transform.SetParent(this.transform);
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
}