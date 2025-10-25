using StarterAssets;
using System;
using UnityEngine;

public class Provoker : MonoBehaviour {

    public void ProvokeEnemies(Transform victimTransform, float radiusValue, LayerMask layerMask) {
        Collider[] hits = Physics.OverlapSphere(victimTransform.position, radiusValue, layerMask, QueryTriggerInteraction.Ignore);
        foreach (Collider mob in hits) {
            EnemyController enemyController = mob.GetComponent<EnemyController>();
            enemyController.GotProvoked(victimTransform);
        }
    }

    internal void ProvokeEnemies(Vector3 position, float moveSpeed, LayerMask layerMask) {
        throw new NotImplementedException();
    }
}
