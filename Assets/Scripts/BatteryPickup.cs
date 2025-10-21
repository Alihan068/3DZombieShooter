using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [SerializeField] float restoreAngle = 70f;
    [SerializeField] float restoreIntensity = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            other.GetComponentInChildren<FlashLight>().RestoreLightAngle(restoreAngle);
            other.GetComponentInChildren<FlashLight>().RestoreLightInensity(restoreIntensity);
            Destroy(gameObject);
        }
    }
}
