using UnityEngine;

public class AmmoPickup : MonoBehaviour
{

    [SerializeField] AmmoType ammoType;
    [SerializeField] int ammoAmount = 10;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            FindFirstObjectByType<Ammo>().IncreaseCurrentAmmo(ammoType, ammoAmount);
            Debug.Log("PlayerPicked up Ammo of type: " + name);
            Destroy(gameObject);
        }
    }
}
