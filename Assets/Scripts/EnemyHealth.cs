using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] float healthPoints = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TakeDamage(float damage) {
        healthPoints -= damage;

        if (healthPoints <= 0) {
        Destroy(gameObject);
        }
    }
}