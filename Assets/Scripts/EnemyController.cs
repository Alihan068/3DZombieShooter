using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    [SerializeField] float chaseRange = 10f;
    [SerializeField] float chaseTimeout = 2f;

    [SerializeField] float turnSpeed = 5f;
    float outOfRangeTimer = 0f;
    bool isProvoked = false;

    AudioSource audioSource;
    [SerializeField] AudioClip[] chaseSounds;
    [SerializeField] AudioClip[] idleSounds;

    Transform target;
    NavMeshAgent navMeshAgent;
    EnemyHealth enemyHealth;

    bool soundPlayedOnce = false;

    float distanceToTarget = Mathf.Infinity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        target = FindFirstObjectByType<PlayerHealth>().transform;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlaySoundAfterDelay(10f, idleSounds));
    }
    // Update is called once per frame
    void Update() {
        if (enemyHealth.IsDead()) {
            enabled = false;
            navMeshAgent.enabled = false;
        }
        distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget >= chaseRange){          
            outOfRangeTimer += Time.deltaTime;

            if (outOfRangeTimer >= chaseTimeout) {
                StopChase();
            }
        }
        else if (isProvoked) {
            EngageTarget();
        }
        else if (distanceToTarget <= chaseRange) {
            isProvoked = true;
        }

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

    void ChaseTarget() {
        StartCoroutine(PlaySoundAfterDelay(10f, chaseSounds));
        navMeshAgent.isStopped = false;
        GetComponent<Animator>().SetBool("attack", false);
        GetComponent<Animator>().SetTrigger("move");
        navMeshAgent.SetDestination(target.position);
    }
    void StopChase() {
        GetComponent<Animator>().SetTrigger("idle");
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
            yield return new WaitForSeconds(delay);
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
        Gizmos.DrawWireSphere(transform.position, chaseRange);

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
