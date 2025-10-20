using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float healthPoints = 100f;
   
    public void TakeDamage(float damage) {
        healthPoints -= damage;
        Debug.Log(this.name + "took damage, remaining hp: " + healthPoints);
        if(healthPoints <= 0 ) {
            GetComponent<DeathHandler>().HandleDeath();
        }
    }
}
