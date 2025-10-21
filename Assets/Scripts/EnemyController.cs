using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    
    [SerializeField] float chaseRange = 10f;
    [SerializeField] float chaseTimeout = 2f;

    [SerializeField] float turnSpeed = 5f;
    float outOfRangeTimer = 0f;
    bool isProvoked = false;

    Transform target;
    NavMeshAgent navMeshAgent;
    EnemyHealth enemyHealth;

    float distanceToTarget = Mathf.Infinity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        target = FindFirstObjectByType<PlayerHealth>().transform;
    }
    // Update is called once per frame
    void Update() {
        if (enemyHealth.IsDead()) {
            enabled = false;
            navMeshAgent.enabled = false; 
        }
        distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (isProvoked) {
            EngageTarget();
        } else if (distanceToTarget <= chaseRange) {
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
        navMeshAgent.isStopped = false;
        GetComponent<Animator>().SetBool("attack", false);
        GetComponent<Animator>().SetTrigger("move");
        navMeshAgent.SetDestination(target.position);
    }
    void StopChase() {       
            isProvoked = false;
            navMeshAgent.ResetPath();
            navMeshAgent.isStopped = true;
            GetComponent<Animator>().SetTrigger("idle");

            outOfRangeTimer = 0;      
    }

    void FaceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0 , direction.z));
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
