using UnityEngine;

public class BulletPool : ObjectPool<Bullet>
{
    public static BulletPool Instance;

    public override void Awake()
    {
        base.Awake();

        Instance = this;
    }
}