using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {
    Idle,
    PursuePlayer,
    Attacking,
    Investigating
}

public class EnemyController : MonoBehaviour {

    [Header("Navigation Settings")]

    [SerializeField] float viewRange = 10f;
    [SerializeField] float pursueTimeOut = 10f;
    [SerializeField] float investigateTimeout = 5f;

    [SerializeField] float jumpAttackDistance = 3f;
    [SerializeField] float heightDifferenceTreshHold = 1.5f;

    [SerializeField] float destinationUpdateInterval = 0.5f;

    [SerializeField] float hearingDistance = 10f;
    [SerializeField] float InvestigationTime = 5f;
    [SerializeField] float fovLimit = 90f;

    [SerializeField] LayerMask playerLayer;

    [SerializeField] float turnSpeed = 5f;
    float outOfInvestigationTimer = 0f;
    float outOfRangeTimer = 0f;
    bool isProvoked = false;

    [Header("Animation Settings")]
    [SerializeField] Transform zombieHead;
    [SerializeField] float lookAtWeight = 1f;
    [SerializeField] float headLookWeight = 1f;
    [SerializeField] float bodyLookWeight = 0.3f;


    float destinationUpdateTimer = 0f;

    [Header("Audio Settings")]
    AudioSource audioSource;
    [SerializeField] AudioClip[] chaseSounds;
    [SerializeField] AudioClip[] idleSounds;

    Transform playerTarget;
    public Transform target;

    NavMeshAgent navMeshAgent;
    EnemyHealth enemyHealth;

    EnemyState enemyState;

    bool soundPlayedOnce = false;
    bool isAlerted = false;

    float distanceToTarget = Mathf.Infinity;

    void OnEnable() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        playerTarget = FindFirstObjectByType<PlayerHealth>().transform;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlaySoundAfterDelay(10f, idleSounds));
        enemyState = EnemyState.Idle;
    }

    void Update() {
        if (enemyHealth.IsDead()) {
            enabled = false;
            navMeshAgent.enabled = false;
        }
        CanSeePlayer();
        if (target != null) {
            distanceToTarget = Vector3.Distance(transform.position, target.position);
        }
        else { target = null; }

        switch (enemyState) {
            case EnemyState.Idle:
                IdleBehaviour();
                if (isAlerted) {
                    enemyState = EnemyState.PursuePlayer;
                }
                if (isProvoked || distanceToTarget <= hearingDistance) {
                    Debug.Log("From Idle to Ivnest");
                    enemyState = EnemyState.Investigating;

                }
                break;

            case EnemyState.Investigating:
                Debug.Log("Investigate");
                ChaseTarget(target);

                destinationUpdateTimer += Time.deltaTime;
                if (destinationUpdateTimer >= destinationUpdateInterval) {
                    destinationUpdateTimer = 0f;
                    Vector3 closestPoint = GetClosestReachablePoint(target.position);
                    navMeshAgent.SetDestination(closestPoint);
                }
                if (IsTargetUnreachableAbove()) {
                    navMeshAgent.isStopped = true;
                    FaceTarget();
                }

                //else if (distanceToTarget <= navMeshAgent.stoppingDistance && isAlerted) {
                //    enemyState = EnemyState.Attacking;
                //}
                //else if (outOfRangeTimer >= chaseTimeout) {
                //    StopChase();
                //    enemyState = EnemyState.Investigating;
                //}
                //else if (distanceToTarget >= viewRange) {
                //    outOfRangeTimer += Time.deltaTime;
                //}

                break;

            case EnemyState.PursuePlayer:
                Debug.Log("Pursue!");
                ChaseTarget(playerTarget);

                destinationUpdateTimer += Time.deltaTime;
                if (destinationUpdateTimer >= destinationUpdateInterval) {
                    destinationUpdateTimer = 0f;
                    Vector3 closestPoint = GetClosestReachablePoint(target.position);
                    navMeshAgent.SetDestination(closestPoint);
                }
                if (IsTargetUnreachableAbove()) {
                    navMeshAgent.isStopped = true;
                    FaceTarget();
                }

                else if (distanceToTarget <= navMeshAgent.stoppingDistance && isAlerted) {
                    enemyState = EnemyState.Attacking;
                }
                else if (outOfRangeTimer >= pursueTimeOut) {
                    StopPursue();
                    enemyState = EnemyState.Investigating;
                }
                else if (distanceToTarget >= viewRange) {
                    outOfRangeTimer += Time.deltaTime;
                }

                break;

            case EnemyState.Attacking:

                FaceTarget();
                AttackTarget();
                if (distanceToTarget >= navMeshAgent.stoppingDistance) {
                    enemyState = EnemyState.PursuePlayer;
                }
                break;
        }

    }

    public void GotProvoked(Transform provoker) {
        Vector3 provokerLastPos = provoker.position;
        target.position = new Vector3(provokerLastPos.x, provokerLastPos.y, provokerLastPos.z);
        Debug.Log("Provoked by:" + provoker.name);
        isProvoked = true;
        if (isAlerted) {
            navMeshAgent.ResetPath();
        }
        else {
            enemyState = EnemyState.Investigating;
        }
    }

    void CanSeePlayer() {

        if (distanceToTarget <= viewRange) {

            Vector3 directionToTarget = (playerTarget.position - zombieHead.position).normalized;
            float angleBetweenTarget = Vector3.Angle(zombieHead.forward, directionToTarget);

            if (angleBetweenTarget < fovLimit / 2) {
                if (!Physics.Linecast(zombieHead.position, playerTarget.position + Vector3.up * 3f, playerLayer)) {
                    isAlerted = true;
                    target = playerTarget;
                }
            }
        }
    }

    void IdleBehaviour() {
        Debug.Log("Idle");
        StartCoroutine(PlaySoundAfterDelay(10f, idleSounds));
        outOfInvestigationTimer = 0;
        outOfRangeTimer = 0;

    }
    void ChaseTarget(Transform target) {
        Debug.Log("Chase!");
        outOfInvestigationTimer = 0;
        StartCoroutine(PlaySoundAfterDelay(10f, chaseSounds));
        navMeshAgent.isStopped = false;
        GetComponent<Animator>().SetBool("attack", false);
        GetComponent<Animator>().SetBool("isMoving", true);

        Vector3 targetPosition = GetClosestReachablePoint(target.position);
        navMeshAgent.SetDestination(targetPosition);
    }

    Vector3 GetClosestReachablePoint(Vector3 targetPos) {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(targetPos, out hit, 10f, NavMesh.AllAreas)) {
            return hit.position;
        }

        return targetPos;
    }
    void StopPursue() {
        Debug.Log("StopPursue");
        GetComponent<Animator>().SetBool("isMoving", false);
        isProvoked = false;
        isAlerted = false;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        outOfRangeTimer = 0;
    }
    IEnumerator PlaySoundAfterDelay(float delay, AudioClip[] clip) {
        if (!soundPlayedOnce) {
            soundPlayedOnce = true;
            yield return new WaitForSeconds(0.1f);
            audioSource.PlayOneShot(clip[UnityEngine.Random.Range(0, clip.Length)]);
            yield return new WaitForSeconds(UnityEngine.Random.Range(5f, delay));
            soundPlayedOnce = false;
        }
    }
    bool IsTargetUnreachableAbove() {

        //Player horizontal distance check
        float horizontalDistance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(target.position.x, 0, target.position.z));

        //Player vertical height diff check
        float heightDifference = target.position.y - transform.position.y;

        //CHeck if reacher destination
        bool reachedDestination = !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;

        return horizontalDistance <= jumpAttackDistance &&
            heightDifference >= heightDifferenceTreshHold &&
            reachedDestination;
    }
    void FaceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion bodyRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, bodyRotation, Time.deltaTime * turnSpeed);
    }

    private void OnAnimatorIK(int layerIndex) {
        if (target != null) {
            Animator animator = GetComponent<Animator>();

            animator.SetLookAtWeight(lookAtWeight, bodyLookWeight, headLookWeight);
            animator.SetLookAtPosition(target.position);
        }
    }

    public void OnDamageTaken() {
        isProvoked = true;
        target = playerTarget;
        enemyState = EnemyState.PursuePlayer;
    }
    void AttackTarget() {
        GetComponent<Animator>().SetBool("attack", true);
        //Debug.Log(name + (" found and attacking " + target.name));
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRange);
        Vector3 sightAngleLeft = DirectionFromAngle(-fovLimit / 2);
        Vector3 sightAngleRight = DirectionFromAngle(fovLimit / 2);
        Gizmos.DrawLine(transform.position, transform.position + sightAngleLeft * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + sightAngleRight * viewRange);
    }

    Vector3 DirectionFromAngle(float angleInDegrees) {
        return Quaternion.Euler(0, transform.eulerAngles.y + angleInDegrees, 0) * Vector3.forward;
    }

    //private void OldUpdateVersion() {
    //    if (distanceToTarget > chaseRange){

    //        outOfRangeTimer += Time.deltaTime;

    //        if (outOfRangeTimer >= chaseTimeout) {
    //            StopChase();

    //        }
    //    }
    //    else if (isProvoked) {
    //        EngageTarget();
    //    }
    //    else if (distanceToTarget <= chaseRange) {
    //        isProvoked = true;
    //    }
    //}
    //}
}
