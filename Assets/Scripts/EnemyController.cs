using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {
    Idle,
    Chasing,
    Attacking,
    Investigating
}

public class EnemyController : MonoBehaviour {

    [SerializeField] float viewRange = 10f;
    [SerializeField] float chaseTimeout = 10f;
    [SerializeField] float investigateTimeout = 5f;

    [SerializeField] float turnSpeed = 5f;
    float outOfInvestigationTimer = 0f;
    float outOfRangeTimer = 0f;
    bool isProvoked = false;

    AudioSource audioSource;
    [SerializeField] AudioClip[] chaseSounds;
    [SerializeField] AudioClip[] idleSounds;

    Transform target;
    NavMeshAgent navMeshAgent;
    EnemyHealth enemyHealth;

    EnemyState enemyState;

    bool soundPlayedOnce = false;

    float distanceToTarget = Mathf.Infinity;

    [SerializeField] float hearingDistance = 10f;
    [SerializeField] float InvestigationTime = 5f;
    [SerializeField] float fovLimit = 90f;
    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        target = FindFirstObjectByType<PlayerHealth>().transform;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlaySoundAfterDelay(10f, idleSounds));
        enemyState = EnemyState.Idle;
    }

    void Update() {
        if (enemyHealth.IsDead()) {
            enabled = false;
            navMeshAgent.enabled = false;
        }
        distanceToTarget = Vector3.Distance(transform.position, target.position);

        //if (distanceToTarget >= viewRange) {
        //    outOfRangeTimer += Time.deltaTime;

        //    if (outOfRangeTimer >= chaseTimeout) {
        //        StopChase();
        //    }
        //}
        //else if (isProvoked) {
        //    EngageTarget();
        //}
        //else if (distanceToTarget <= viewRange) {
        //    isProvoked = true;
        //}

        switch (enemyState) {
            case EnemyState.Idle:
                IdleBehaviour();
                if (isProvoked || distanceToTarget <= hearingDistance) {
                    Debug.Log("From Idle to Ivnest");
                    enemyState = EnemyState.Investigating;
                    
                }
                break;

            case EnemyState.Investigating:
                FaceTarget();
                Debug.Log("Investigate");
                outOfInvestigationTimer += Time.deltaTime;

                if (CanSeePlayer()) {
                    Debug.Log("From Invest to Chase");
                    enemyState = EnemyState.Chasing;
                    
                }

                else if (outOfInvestigationTimer >= investigateTimeout) {
                    Debug.Log("From Chase To Invest");
                    enemyState = EnemyState.Idle;
                    
                }

                
                break;

            case EnemyState.Chasing:

                ChaseTarget();

                if (distanceToTarget <= navMeshAgent.stoppingDistance) {
                    enemyState = EnemyState.Attacking;
                }
                else if (outOfRangeTimer >= chaseTimeout) {
                    StopChase();
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
                    enemyState = EnemyState.Chasing;
                }
                break;
        }

    }
    bool CanSeePlayer() {

        if (distanceToTarget <= viewRange) {

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float angleBetweenTarget = Vector3.Angle(Vector3.forward, directionToTarget);

            if (angleBetweenTarget < fovLimit / 2) {
                if (!Physics.Linecast(transform.position + Vector3.up, target.position + Vector3.up)) {
                    return true;
                }
            }
        }
        return false;
    }

    void EngageTarget() {
        FaceTarget();

        if (distanceToTarget >= navMeshAgent.stoppingDistance) {
            ChaseTarget();
        }

        if (distanceToTarget <= navMeshAgent.stoppingDistance) {
            AttackTarget();
        }
    }

    void IdleBehaviour() {
        Debug.Log("Idle");
        StartCoroutine(PlaySoundAfterDelay(10f, idleSounds));
        outOfInvestigationTimer = 0;
        outOfRangeTimer = 0;

    }
    void ChaseTarget() {
        Debug.Log("Chase!");
        outOfInvestigationTimer = 0;
        StartCoroutine(PlaySoundAfterDelay(10f, chaseSounds));
        navMeshAgent.isStopped = false;
        GetComponent<Animator>().SetBool("attack", false);
        GetComponent<Animator>().SetBool("isMoving", true);
        navMeshAgent.SetDestination(target.position);
    }

    void StopChase() {
        Debug.Log("StopChase");
        GetComponent<Animator>().SetBool("isMoving", false);
        isProvoked = false;
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
    void FaceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    public void OnDamageTaken() {
        isProvoked = true;
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
