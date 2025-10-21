using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon", menuName = "Create WeaponSO")]
public class WeaponPatternSO : ScriptableObject
{

    public enum WeaponFiringType {
        RayBullet,
        ProjectileBullet,
        ParticleBullet,
    }

    public WeaponFiringType weaponType;
    [Header("Weapon Attributes")]
    public float range = 100f;
    public float damage = 40f;
    public float sprayFactor = 0f;
    public int numberOfProjectiles = 1;
    public int numberOfBursts = 1;
    public float timeBetweenShots = 0.5f;
    public bool isAutomatic = false;

    public GameObject projectilePrefab;

}
