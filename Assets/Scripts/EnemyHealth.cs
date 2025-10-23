using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] float healthPoints = 100f;

    AudioSource audioSource;
    [SerializeField] GameObject[] ItemDropList;
    [SerializeField] float minDropChance = 10;
    [SerializeField] float luckMultiplier = 10;
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
        DropItems();
        audioSource.PlayOneShot(DeathClips[Random.Range(0, DeathClips.Length)]);
        GetComponent<Animator>().SetTrigger("death");
        
    }

    void DropItems() {
        bool isDropped = false;

        for (int i = 0; i < ItemDropList.Length; i++) {
            if (isDropped) return;
            int dropRoll = Mathf.RoundToInt(Random.Range(luckMultiplier, 100));
            if (dropRoll >= minDropChance) {
                var drop = ItemDropList[Random.Range(0, ItemDropList.Length)];
                Instantiate(drop, transform.position, Quaternion.identity);
                Debug.Log("JACKPOT!: " + drop.name);
                isDropped = true;
            }
        }

    }
}