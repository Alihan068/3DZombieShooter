using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    [SerializeField] float damage = 25f;
    [SerializeField] PlayerHealth target;

    private void Start() {
        target = FindFirstObjectByType<PlayerHealth>();
    }
    public void AttackHitEvent(){
        if (target == null) return;
        target.TakeDamage(damage);
        Debug.Log(this.name + "is attacking" + target.name);
    }
}
