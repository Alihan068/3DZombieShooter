using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour {

    //[Header("Weapon Attributes")]
    //[SerializeField] float range = 100f;
    //[SerializeField] float damage = 40f;
    //[SerializeField] float sprayFactor = 0f;
    //[SerializeField] int numberOfProjectiles = 1;
    //[SerializeField] int numberOfBursts = 1;
    //[SerializeField] float timeBetweenShots = 0.5f;
    //[SerializeField] bool isAutomatic = false;
    [SerializeField] WeaponPatternSO weaponPattern;
    [SerializeField] AmmoType ammoType;
    [Header("Misc")]
    [SerializeField] Ammo ammoSlot;
    [SerializeField] Camera FPCamera;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    [SerializeField] TextMeshProUGUI ammoText;

    [SerializeField] AudioClip weaponShootSound;
    [SerializeField] AudioClip emptyWeaponSound;
    [SerializeField] AudioClip[] weaponEquipSound;

    AudioSource audioSource;

    [HideInInspector] public bool canShoot = true;

    Transform projectileBarrel;

    void Update() {
        DisplayAmmo();

        //if ((weaponPattern.weaponType == WeaponPatternSO.WeaponFiringType.RayBullet) && weaponPattern.isAutomatic && Input.GetMouseButtonDown(0) && canShoot == true) {
        //    StartCoroutine(RaycastShoot());
        //}
        //else if ((weaponPattern.weaponType == WeaponPatternSO.WeaponFiringType.ProjectileBullet) && !weaponPattern.isAutomatic && Input.GetMouseButtonDown(0) && canShoot == true) {
        //    StartCoroutine(ProjectileShoot());
        //}

        switch (weaponPattern.weaponType) {
            case WeaponPatternSO.WeaponFiringType.RayBullet:
                if (!canShoot) {
                    return;
                }
                else if (weaponPattern.isAutomatic && Input.GetMouseButton(0)) {
                    StartCoroutine(RaycastShoot());
                }
                else if (!weaponPattern.isAutomatic && Input.GetMouseButtonDown(0)) {
                    StartCoroutine(RaycastShoot());
                }
                break;

            case WeaponPatternSO.WeaponFiringType.ProjectileBullet:
                if (!canShoot) {
                    return;
                }
                else if (weaponPattern.isAutomatic && Input.GetMouseButton(0)) {
                    StartCoroutine(ProjectileShoot());
                }
                else if (!weaponPattern.isAutomatic && Input.GetMouseButtonDown(0)) {
                    StartCoroutine(ProjectileShoot());
                }
                break;

            case WeaponPatternSO.WeaponFiringType.ParticleBullet:
                if (!canShoot) {
                    return;
                }
                else if (weaponPattern.isAutomatic && Input.GetMouseButton(0)) {

                }
                else if (!weaponPattern.isAutomatic && Input.GetMouseButtonDown(0)) {

                }
                break;
        }
    }

    private void OnEnable() {
        canShoot = true;
        audioSource = GetComponentInParent<AudioSource>();
        if (weaponPattern.weaponType == WeaponPatternSO.WeaponFiringType.ProjectileBullet) {
            foreach (Transform child in GetComponentsInChildren<Transform>()) {
                if (child.CompareTag("ProjectileBarrel")) {
                    projectileBarrel = child;
                    break;
                }
            }
        }

        if (weaponEquipSound != null) {
            audioSource.PlayOneShot(weaponEquipSound[Random.Range(0, weaponEquipSound.Length)]);
        }
    }

    IEnumerator RaycastShoot() {
        canShoot = false;

        for (int i = 0; i < weaponPattern.numberOfBursts; i++) {
            if (ammoSlot.GetCurrentAmmo(ammoType) > 0) {
                ProcessRaycast();
                PlayWeaponEffects();
                ammoSlot.ReduceAmmoAmount(ammoType);
                yield return new WaitForSeconds(0.1f);
            }
            else if (emptyWeaponSound != null) {
                audioSource.PlayOneShot(emptyWeaponSound);
            }
        }

        yield return new WaitForSeconds(weaponPattern.timeBetweenShots);
        canShoot = true;
    }

    void ProcessRaycast() {

        Vector3 rayOrigin = FPCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        for (int i = 0; i < weaponPattern.numberOfProjectiles; i++) {

            Vector3 direction = FPCamera.transform.forward;
            direction.x += Random.Range(-weaponPattern.sprayFactor, weaponPattern.sprayFactor);
            direction.y += Random.Range(-weaponPattern.sprayFactor, weaponPattern.sprayFactor);
            Debug.DrawRay(FPCamera.transform.position, direction * weaponPattern.range, Color.red, 5f);

            RaycastHit hit;
            if (Physics.Raycast(FPCamera.transform.position, direction.normalized, out hit, weaponPattern.range)) {
                //Debug.Log("Hit!: " + hit.transform.name);

                CreateHitImpact(hit);

                EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
                if (target != null) {
                    target.TakeDamage(weaponPattern.damage);
                }

            }
            else {
                return;
            }
        }
    }
    IEnumerator ProjectileShoot() {
        canShoot = false;
        if (ammoSlot.GetCurrentAmmo(ammoType) > 0) {

            ProjectileLaunch();
            for (int i = 0; i < weaponPattern.numberOfBursts; i++) {
                PlayWeaponEffects();
                ammoSlot.ReduceAmmoAmount(ammoType);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (emptyWeaponSound != null) {
            audioSource.PlayOneShot(emptyWeaponSound);
        }

        yield return new WaitForSeconds(weaponPattern.timeBetweenShots);
        canShoot = true;
    }

    void ProjectileLaunch() {
        if (weaponPattern.projectilePrefab != null) {

            Vector3 direction = FPCamera.transform.forward;
            direction.x += Random.Range(-weaponPattern.sprayFactor, weaponPattern.sprayFactor);
            direction.y += Random.Range(-weaponPattern.sprayFactor, weaponPattern.sprayFactor);

            for (int i = 0; i < weaponPattern.numberOfProjectiles; i++) {

                GameObject projectile = Instantiate(weaponPattern.projectilePrefab, projectileBarrel.position, FPCamera.transform.rotation);
                Rigidbody bulletRb = projectile.GetComponent<Rigidbody>();
                bulletRb.AddForce(direction.normalized * (weaponPattern.range / 5), ForceMode.Impulse);
            }
        }
    }

    void PlayWeaponEffects() {
        muzzleFlash.Play();
        audioSource.PlayOneShot(weaponShootSound);
    }

    void CreateHitImpact(RaycastHit hit) {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        var main = hitEffect.GetComponentInChildren<ParticleSystem>().main;

        float impactDurationTime = main.duration;

        Destroy(impact, impactDurationTime);
    }

    void DisplayAmmo() {
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
        ammoText.text = currentAmmo.ToString();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        for (int i = 0; i < weaponPattern.numberOfProjectiles; i++) {
            Vector3 direction = FPCamera.transform.forward;
            direction.x += Random.Range(-weaponPattern.sprayFactor, weaponPattern.sprayFactor);
            direction.y += Random.Range(-weaponPattern.sprayFactor, weaponPattern.sprayFactor);

            Vector3 start = FPCamera.transform.position;
            Vector3 end = start + direction * weaponPattern.range;
        }
    }
}
