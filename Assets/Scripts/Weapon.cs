using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 40f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;

    [SerializeField] Ammo ammoSlot;
    [SerializeField] float timeBetweenShots = 0.5f;

    [SerializeField] AmmoType ammoType;
    [SerializeField] TextMeshProUGUI ammoText;

    [HideInInspector] public bool canShoot = true;
    // Update is called once per frame
    void Update() {
        DisplayAmmo();

        if (Input.GetMouseButtonDown(0)&& canShoot == true) {
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
            PlayMuzzleFlash();
            ammoSlot.ReduceAmmoAmount(ammoType);

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

    void DisplayAmmo() {
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
        ammoText.text = currentAmmo.ToString();
    }
}
