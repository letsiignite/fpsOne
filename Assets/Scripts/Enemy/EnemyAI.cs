using Game;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
namespace Enemy
{
    public class EnemyAI : MonoBehaviour, IDamageHandler
    {
        public Transform startPoint;
        public Transform endPoint;
        public Transform[] coverPoints;
        public Transform player;
        public float detectionRange = 15f;
        public float fieldOfView = 90f;
        public float fireRate = 1f;
        public int maxHealth = 100;
        public GameObject enemyEyesPos;
        public LayerMask layerMask;
        public AudioClip gunFireAudioClip;
        public AudioSource audioSource;

        [SerializeField]
        private EnemyGun enemyGun;
        private float currentHealth;
        private NavMeshAgent agent;
        private float nextFireTime = 0f;
        private Animator animator;
        private bool detectedPlayer = false;
        private bool inCover = false;

        private enum State { MovingToPoint, Idle, Combat, TakingCover, Dead }
        private State currentState;
        private const float CLOSE_COMBAT_THRESHOLD = 5f;
        private Vector3 dir;

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
            if (other.tag == "Player")
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
            return;
            Vector3 direction = player.position - transform.position;
            direction.y = 0f; // Ignore vertical axis

            if (direction.magnitude > 0.1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }

        void Update()
        {
            if(detectedPlayer && currentState == State.Idle)
            {
                FacePlayer();
            }
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
                default:

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
            //Debug.Log("dirToPlayer.magnitude = " + dirToPlayer.magnitude + " | angle = " + angle/2);

            if (dirToPlayer.magnitude <= detectionRange && angle <= fieldOfView / 2f)
            {
                //Debug.Log(" Check Has Line OfSight");
                if (HasLineOfSight())
                {
                    detectedPlayer = true;
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

            // Make sure the enemy does not tilt.
            Vector3 targetPositionAdjusted = new Vector3(player.position.x, transform.position.y, player.position.z);

            agent.isStopped = true;
            transform.LookAt(targetPositionAdjusted);

            if (Time.time >= nextFireTime)
            {
                ShootAtPlayer();
                nextFireTime = Time.time + 1f / fireRate;
            }

            if ((currentHealth < maxHealth / 2) && !inCover)
            {
                currentState = State.TakingCover;
            }
        }

        void TakingCoverBehavior()
        {
            Transform bestCover = FindClosestCover();
            float distanceToCover = Vector3.Distance(transform.position, bestCover.position);
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if(distanceToPlayer < CLOSE_COMBAT_THRESHOLD)
            {
                if(distanceToPlayer < CLOSE_COMBAT_THRESHOLD)
                {
                    inCover = true;
                    currentState = State.Combat;
                }
            }
            if (bestCover != null)
            {
                agent.isStopped = false;
                agent.SetDestination(bestCover.position);
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    inCover = true;
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
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.whiteSmoke;
            dir = (player.position - enemyEyesPos.transform.position).normalized;
            Gizmos.DrawRay(enemyEyesPos.transform.position, dir * detectionRange);
        }
        bool HasLineOfSight()
        {
            RaycastHit hit;
            dir = (player.position - enemyEyesPos.transform.position).normalized;
            if (Physics.Raycast(enemyEyesPos.transform.position, dir, out hit, detectionRange, enemyGun.layerMask))
            {
                //Debug.Log(gameObject.name+ " >> In sight = " + hit.transform.name);
                if (hit.transform.tag == "Player")
                {
                    //Debug.Log(" return true ");
                    return true;
                }
            }
           
            return false;
        }

        void ShootAtPlayer()
        {
            //  TODO: Here we must add projectile instantiation & raycast damage logic
            audioSource.Stop();
            audioSource.clip = gunFireAudioClip;
            audioSource.Play();
            enemyGun.Shoot(dir);
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if ((currentHealth < maxHealth / 2) && !inCover)
            {
                currentState = State.TakingCover;
            }
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
            currentState = State.Dead;
            //Destroy(gameObject);
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

        public void ProcessDamage(float damageMultiplyer, float damage)
        {
            Debug.Log($" In Enemy Ai damageMultiplyer = {damageMultiplyer} | damage = {damage}");
            float totalDamage = damageMultiplyer * damage;
            TakeDamage(totalDamage);
        }
    }
}
