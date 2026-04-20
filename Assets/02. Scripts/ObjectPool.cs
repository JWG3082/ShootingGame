using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool _instance;

    // 먼저 필요한 오브젝트들 먼저 설정
    [SerializeField] private List<GameObject> _BulletList = new();

    [SerializeField] private GameObject _BulletPrefab;

    [SerializeField] private int _FullCount;

    #region 유니티 콜백 함수

    private void Awake()
    {
        if (_instance == null) _instance = this;
        for (var i = 0; i < _FullCount; i++)
        {
            var bullet = Instantiate(_BulletPrefab, transform.position, transform.rotation);
            bullet.transform.SetParent(transform);
            bullet.name = name+i.ToString();
            bullet.SetActive(false);
            _BulletList.Add(bullet);
        }
    }

    #endregion

    public GameObject SetActiveOBJ()
    {
        foreach (GameObject _bullet in _BulletList)
        {
            if (!_bullet.activeSelf)
            {
                _bullet.transform.SetParent(null);
                _bullet.SetActive(true);
                return _bullet;
            }
        }
        GameObject newObj = Instantiate(_BulletPrefab);
        _BulletList.Add(newObj);
        return newObj;
    }

    public void DisAbleBullet(GameObject bullet)
    {
        bullet.transform.SetParent(transform);
        bullet.SetActive(false);
    }
}