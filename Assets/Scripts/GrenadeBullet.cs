using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
    [Header("Timing")]
    [SerializeField] float fuseTime = 2f;

    [Header("Explosion")]
    [SerializeField] float radius = 5f;
    [SerializeField] float explosionForce = 700f;
    [SerializeField] float upwardsModifier = 1f;
    [SerializeField] float maxDamage = 100f;
    [SerializeField] LayerMask affectedLayers = ~0;

    [Header("Effects")]
   
    [SerializeField] AudioClip explosionSfx;
    //[SerializeField] float destroyAfter = 5f;
    [SerializeField] GameObject explosionEffectPrefab;
    AudioSource audioSource;

    void OnEnable() {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(DetonateAfterDelay());
        //Debug.Log("Summoned Grenade");
    }
    public void DetonateNow() {
        StartCoroutine(Explode());
    }

    IEnumerator DetonateAfterDelay() {
        //Debug.Log("FuseTime Activated : " + fuseTime);
        yield return new WaitForSeconds(fuseTime);
        yield return StartCoroutine(Explode());
    }

    IEnumerator Explode() {
        Instantiate(explosionEffectPrefab,transform.position, Quaternion.identity);
        //Debug.Log("Fusetime Finished, Explode!");
        Vector3 currentPos = transform.position;

        // Physics and damage
        Collider[] hits = Physics.OverlapSphere(currentPos, radius, affectedLayers, QueryTriggerInteraction.Ignore);
        foreach (var col in hits) {
            //Debug.Log("Hit! : "+ col);
            // Apply explosion force to rigidbodies
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null) {
                rb.AddExplosionForce(explosionForce, currentPos, radius, upwardsModifier, ForceMode.Impulse);
            }

            // Damage falloff based on distance
            float distance = Vector3.Distance(currentPos, col.ClosestPoint(currentPos));
            float distanceFactor = Mathf.Clamp01(1f - (distance / radius));
            float damage = maxDamage * distanceFactor;

            // Deliver damage to IDamageable on collider or its parents
            EnemyHealth enemyHealth = col.GetComponent<EnemyHealth>();
            if (enemyHealth != null) {
                enemyHealth.TakeDamage(damage);

            PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();
                if (playerHealth != null) { 
                playerHealth.TakeDamage(damage/2);
                }
            }
        }

        // Optional: hide grenade model before destroy
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);

        // destroy grenade object after short time to allow sfx to play
        Destroy(gameObject, 0.1f);
        yield return null;
    }

    // debug visualization
    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
