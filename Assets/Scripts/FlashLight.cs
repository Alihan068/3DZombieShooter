using UnityEngine;

public class FlashLight : MonoBehaviour {
    [SerializeField] float lightDecay = 0.1f;
    [SerializeField] float angleDecay = 1f;
    [SerializeField] float minimumAngle = 40f;

    Light myLight;
    void Start() {
        myLight = GetComponent<Light>();
        myLight.spotAngle = myLight.innerSpotAngle + 5f;
    }

    // Update is called once per frame
    void Update() {
        DecreaseLightAngle();
        DecreaseLightIntensity();
    }

    void DecreaseLightAngle() {
        if (myLight.spotAngle <= minimumAngle) return;
        else {
            myLight.spotAngle -= angleDecay * Time.deltaTime;
            myLight.spotAngle = myLight.innerSpotAngle + 5f;
        }
    }

    void DecreaseLightIntensity() {
        myLight.intensity -= lightDecay * Time.deltaTime;
    }

    public void RestoreLightAngle(float restoreAngle) {
        myLight.spotAngle = restoreAngle;
        myLight.innerSpotAngle = myLight.innerSpotAngle + 5f;
    }

    public void RestoreLightInensity(float intensityAmount) {
        myLight.intensity += intensityAmount;
    }
}
