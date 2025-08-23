using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Transform[] coverPoints;
    public Transform player;
    public float detectionRange = 15f;
    public float fieldOfView = 90f;
    public float fireRate = 1f;
    public int maxHealth = 100;

    private int currentHealth;
    private NavMeshAgent agent;
    private float nextFireTime = 0f;
    private Animator animator;

    private enum State { MovingToPoint, Idle, Combat, TakingCover }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentState = State.MovingToPoint;
        agent.SetDestination(endPoint.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            FacePlayer();
        }
    }

    void OnTriggerStay(Collider other)      // No need to rotate in update method
    {
        if (other.CompareTag("Player"))
        {
            FacePlayer();
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;                  // keep only horizontal rotation
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.MovingToPoint:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    currentState = State.Idle;
                }
                ResetAnimation();
                animator.SetBool("run", true);
                //agent.speed = (animator.deltaPosition / Time.deltaTime).magnitude;
                DetectPlayer();
                break;

            case State.Idle:
                ResetAnimation();
                animator.SetBool("idle", true);
                DetectPlayer();
                break;

            case State.Combat:
                ResetAnimation();
                animator.SetBool("shoot", true);
                CombatBehavior();
                break;

            case State.TakingCover:
                ResetAnimation();
                animator.SetBool("run", true);
                //agent.speed = (animator.deltaPosition / Time.deltaTime).magnitude;
                TakingCoverBehavior();
                break;
        }
    }

    private void ResetAnimation()
    {
        animator.SetBool("run", false);
        animator.SetBool("idle", false);
        animator.SetBool("shoot", false);
    }

    void DetectPlayer()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        Debug.Log("DetectPlayer");
        if (dirToPlayer.magnitude <= detectionRange && angle <= fieldOfView / 2f)
        {
            Debug.Log("Player in range");
            if (HasLineOfSight())
            {
                currentState = State.Combat;
            }
        }
    }

    void CombatBehavior()
    {
        if (player == null) return;

        if (!HasLineOfSight())
        {
            currentState = State.Idle;
            return;
        }

        agent.isStopped = true;
        transform.LookAt(player);

        if (Time.time >= nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + 1f / fireRate;
        }

        if (currentHealth < maxHealth / 2)
        {
            currentState = State.TakingCover;
        }
    }

    void TakingCoverBehavior()
    {
        Transform bestCover = FindClosestCover();
        if (bestCover != null)
        {
            agent.isStopped = false;
            agent.SetDestination(bestCover.position);
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentState = State.Combat;
            }
        }
    }

    Transform FindClosestCover()
    {
        Transform best = null;
        float shortestDist = Mathf.Infinity;
        foreach (Transform cover in coverPoints)
        {
            float dist = Vector3.Distance(transform.position, cover.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                best = cover;
            }
        }
        return best;
    }

    bool HasLineOfSight()
    {
        RaycastHit hit;
        Vector3 dir = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position + Vector3.up, dir, out hit, detectionRange))
        {
            Debug.Log(" Ray hit = "+hit.transform.name);
            if (hit.transform == player)
            {
                return true;
            }
        }
        Debug.Log(" Done ray cast");
        return false;
    }

    void ShootAtPlayer()
    {
        Debug.Log("Enemy shooting at player!");
        //  TODO: Here we must add projectile instantiation & raycast damage logic
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        ResetAnimation();
        animator.SetBool("dead", true);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw field of view
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftBoundary * detectionRange);
        Gizmos.DrawRay(transform.position, rightBoundary * detectionRange);
    }
}
