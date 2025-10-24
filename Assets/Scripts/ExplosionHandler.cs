using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    [SerializeField] float lifeTime = 1f;
    [SerializeField] AudioClip explosionSFX;
     AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable() {
        Debug.Log("Boom!");
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(explosionSFX);
        Destroy(gameObject, lifeTime);

        
    }
}
