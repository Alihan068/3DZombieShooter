using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] float healthPoints = 100f;

    AudioSource audioSource;
    [SerializeField] AudioClip[] takeDamageClips;
    [SerializeField] AudioClip[] DeathClips;

    bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    public void TakeDamage(float damage) {

        BroadcastMessage(nameof(EnemyController.OnDamageTaken));

        healthPoints -= damage;

        if (healthPoints <= 0) {
            Die();
        }
        audioSource.PlayOneShot(takeDamageClips[Random.Range(0, takeDamageClips.Length)]);
    }

    public bool IsDead() {
        return isDead;
    }

    public void Die() {
        if (isDead) { return; }
        isDead = true;
        audioSource.PlayOneShot(DeathClips[Random.Range(0, DeathClips.Length)]);
        GetComponent<Animator>().SetTrigger("death");
        
    }
}