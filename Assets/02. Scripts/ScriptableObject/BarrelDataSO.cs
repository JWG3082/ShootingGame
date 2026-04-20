using UnityEngine;

[CreateAssetMenu(fileName = "BarrelDataSO", menuName = "Scriptable Objects/BarrelDataSO")]
public class BarrelDataSO : ScriptableObject
{
    [Header("폭발 옵션")] 
    public float force = 1500.0f;
    public float upwardForce = 100.0f;
    public float radius = 10.0f;
    public GameObject expEffect;
    public LayerMask layerMask;
    
    [Header("효과음")] 
    public AudioClip expSfx;
}