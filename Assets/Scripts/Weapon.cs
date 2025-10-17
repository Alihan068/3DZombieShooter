using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 40f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            Shoot();
        }
    }

    void Shoot() {
        ProcessRaycast();
        PlayMuzzleFlash();

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
