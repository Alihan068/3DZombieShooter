using Unity.VisualScripting;
using UnityEngine;

public class ZombieHead : MonoBehaviour
{
    //EnemyController enemyController;
    EnemyController enemyController;
    Transform target;
    EnemyState enemyState;

    private void Start() {
        target = FindFirstObjectByType<PlayerHealth>().transform;
        enemyController = FindFirstObjectByType<EnemyController>();
        enemyState = enemyController.gameObject.GetComponent<EnemyState>();
        Debug.Log(target.name);
    }

    private void Update() {
        Debug.Log("Remote check enemy State = " + enemyState);
        if (enemyState == EnemyState.Chasing) {
            LookAtTargetVertically();
        }
    }

    public void LookAtTargetVertically() {
            transform.LookAt(target);

    }
}
