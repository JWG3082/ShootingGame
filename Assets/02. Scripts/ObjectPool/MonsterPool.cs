using UnityEngine;

public class MonsterPool : MonoBehaviour
{
    public static MonsterPool instance;
    public PoolManager<MonsterController> pool;

    private void Awake()
    {
        instance = this;
        var monster = Resources.Load<MonsterController>("Monster");
        pool = new PoolManager<MonsterController>(monster);
    }
}
