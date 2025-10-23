using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    [SerializeField] float damage = 25f;
    PlayerHealth target;
    [SerializeField] AudioClip[] attackClips;
    AudioSource audioManager;
    

    private void Start() {
        target = FindFirstObjectByType<PlayerHealth>();
        audioManager = GetComponent<AudioSource>();
    }
    public void AttackHitEvent(){
        if (target == null) return;
        target.TakeDamage(damage);
        audioManager.PlayOneShot(attackClips[Random.Range(0, attackClips.Length)]);
        target.GetComponent<DisplayDamage>().ShowDamageImpact();
        Debug.Log(this.name + "is attacking" + target.name);
    }
}
