using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [Header("Weapon Attributes")]
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 40f;
    [SerializeField] float sprayFactor = 0f;
    [SerializeField] int numberOfProjectiles = 1;
    [SerializeField] int numberOfBursts = 1;
    [SerializeField] float timeBetweenShots = 0.5f;
    [SerializeField] bool isAutomatic = false;
    [SerializeField] AmmoType ammoType;
    [Header("Misc")]
    [SerializeField] Ammo ammoSlot;
    [SerializeField] Camera FPCamera;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    [SerializeField] TextMeshProUGUI ammoText;

    [HideInInspector] public bool canShoot = true;
    // Update is called once per frame
    void Update() {
        DisplayAmmo();

        if (!isAutomatic && Input.GetMouseButtonDown(0) && canShoot == true) {
            StartCoroutine(Shoot());
        }
        else if (isAutomatic && Input.GetMouseButton(0) && canShoot == true) {
            StartCoroutine(Shoot());
        }
    }

    private void OnEnable() {
        canShoot = true;
    }

    IEnumerator Shoot() {
        canShoot = false;
        if (ammoSlot.GetCurrentAmmo(ammoType) > 0) {

            ProcessRaycast();
            for (int i = 0; i < numberOfBursts; i++) {
                PlayMuzzleFlash();
                ammoSlot.ReduceAmmoAmount(ammoType);
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    void ProcessRaycast() {

        Vector3 rayOrigin = FPCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        for (int i = 0; i < numberOfProjectiles; i++) {

            Vector3 direction = FPCamera.transform.forward;
            direction.x += Random.Range(-sprayFactor, sprayFactor);
            direction.y += Random.Range(-sprayFactor, sprayFactor);
            Debug.DrawRay(FPCamera.transform.position, direction * range, Color.red, 5f);

            RaycastHit hit;
            if (Physics.Raycast(FPCamera.transform.position, direction.normalized, out hit, range)) {
                //Debug.Log("Hit!: " + hit.transform.name);

                CreateHitImpact(hit);

                EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
                if (target != null) {
                    target.TakeDamage(damage);
                }

            }
            else {
                return;
            }
        }
    }

    void PlayMuzzleFlash() {
        muzzleFlash.Play();
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
        for (int i = 0; i < numberOfProjectiles; i++) {
            Vector3 direction = FPCamera.transform.forward;
            direction.x += Random.Range(-sprayFactor, sprayFactor);
            direction.y += Random.Range(-sprayFactor, sprayFactor);

            Vector3 start = FPCamera.transform.position;
            Vector3 end = start + direction * range;
        }
    }
}
