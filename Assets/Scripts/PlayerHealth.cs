using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float healthPoints = 100f;
    [SerializeField] Slider healthSlider;
    private void Start() {
        healthSlider.maxValue = healthPoints;
        healthSlider.value = healthPoints;
    }
    public void TakeDamage(float damage) {
        healthPoints -= damage;
        healthSlider.value = healthPoints;
        Debug.Log(this.name + "took damage, remaining hp: " + healthPoints);
        if(healthPoints <= 0 ) {
            GetComponent<DeathHandler>().HandleDeath();
        }
    }
}
