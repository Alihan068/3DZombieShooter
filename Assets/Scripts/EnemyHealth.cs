using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] float healthPoints = 100f;

    bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TakeDamage(float damage) {

        BroadcastMessage(nameof(EnemyController.OnDamageTaken));

        healthPoints -= damage;

        if (healthPoints <= 0) {
            Die();
        }
    }

    public bool IsDead() {
        return isDead;
    }

    public void Die() {
        if (isDead) { return; }
        isDead = true;
        GetComponent<Animator>().SetTrigger("death");
    }
}