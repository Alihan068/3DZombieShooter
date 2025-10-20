using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 40f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;

    [SerializeField] Ammo ammoSlot;
    [SerializeField] float timeBetweenShots = 0.5f;

    bool canShoot = true;
    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)&& canShoot == true) {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot() {
        canShoot = false;
        if (ammoSlot.GetCurrentAmmo() > 0) {

            ProcessRaycast();
            PlayMuzzleFlash();
            ammoSlot.ReduceAmmoAmount();

        }

        yield return new WaitForSeconds(timeBetweenShots);
        canShoot=true;
    }

    void ProcessRaycast() {
        RaycastHit hit;
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range)) {
            Debug.Log("Hit!: " + hit.transform.name);

            CreateHitImpact(hit);

            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target == null) {
                return;
            }
            target.TakeDamage(damage);
        }
        else {
            return;
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
}
