using StarterAssets;
using System.Collections;
using UnityEngine;

public class PlayerMoveEffects : MonoBehaviour {
    FirstPersonController fpController;
    Provoker provoker;

    [SerializeField] LayerMask layerMask;

    private Coroutine stepCoroutine;


    bool isMoving;
    private void Start() {
        fpController = FindFirstObjectByType<FirstPersonController>();
        provoker = FindFirstObjectByType<Provoker>();

    }
    private void Update() {
        isMoving = fpController.isMoving;

        if (isMoving && stepCoroutine == null)
            stepCoroutine = StartCoroutine(Stepper());
        else if (!isMoving && stepCoroutine != null) {
            StopCoroutine(stepCoroutine);
            stepCoroutine = null;
        }

    }
    IEnumerator Stepper() {
        while (isMoving) {
            MoveEffects();
            yield return new WaitForSeconds(fpController.MoveSpeed * Time.deltaTime);
        }
    }

    void MoveEffects() {
        if (provoker != null) {
            provoker.ProvokeEnemies(transform, fpController.MoveSpeed, layerMask);
        }
    }
}
