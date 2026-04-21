using Game;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
namespace Enemy
{
    public class EnemyAI : MonoBehaviour, IDamageHandler, IEnemySolder
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
        [Tooltip("What type of gun this solder has")]
        public DropGunType soldierType;

        [SerializeField]
        private EnemyGun enemyGun;
        private float currentHealth;
        private NavMeshAgent agent;
        private float nextFireTime = 0f;
        private Animator animator;
        private bool detectedPlayer = false;
        private bool inCover = false;
        [SerializeField]
        private List<GameObject> objectsToDisableOnDeath;

        private enum State
        {
            MovingToPoint,
            Idle,
            Combat,
            TakingCover,
            InCoverWait,
            MovingToShootPoint,
            Charging,
            Dead
        }

        private State currentState;
        private const float CLOSE_COMBAT_THRESHOLD = 5f;
        private Vector3 dir;
        private Vector3 startPos;

        [Header("Shoot Randomization")]
        [SerializeField]
        private AudioClip[] shootAudio = new AudioClip[4];
        [SerializeField]
        private int[] shootCount = new int[4];
        private List<int> numbers = new List<int> { 0, 1, 2, 3};

        [SerializeField] 
        private CombatPositionProvider combatPositionProviderObj;
        

        [SerializeField] 
        private float hitReactionProbability = 0.6f;
        [SerializeField] 
        private float lowHealthThreshold = 0.4f;
        [SerializeField] 
        private float coverWaitTime = 2f;

        private float coverTimer;
        private Transform currentTargetPoint;

        private UniqueRandom<int> randomInts;
        private bool initCompleted = false;
        private Transform reservedCover;
        private bool isTakingCover = false;
        Transform bestCover;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            startPos = transform.position;
            currentHealth = maxHealth;
            currentState = State.MovingToPoint;
            agent.SetDestination(endPoint.position);
            randomInts = new UniqueRandom<int>(numbers);
            combatPositionProviderObj = GameObject.FindAnyObjectByType<CombatPositionProvider>();
        }

        private void OnEnable()
        {
            if(initCompleted)
            Reset();
        }

        public void Reset()
        {
            Debug.Log(" Reseting "+gameObject.name);
            // If agent is null then this enemy has not been activated yet, so no need to reset
            if (agent == null) return;
            transform.position = startPos;
            agent.ResetPath();
            agent.Warp(startPos);
            agent.isStopped = false;
            if ( !agent.isOnNavMesh )
            {
                Debug.Log(" Not on NAV Mesh ");
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }

            currentHealth = maxHealth;
            currentState = State.MovingToPoint;
            detectedPlayer = false;

            ResetAnimation();

            animator.SetBool("idle", true);
            animator.Update(0f);
            agent.SetDestination(endPoint.position);
            foreach (GameObject g in objectsToDisableOnDeath)
            {
                g.SetActive(true);
            }

            if (reservedCover != null)
            {
                combatPositionProviderObj.ReleaseCover(reservedCover, this);
                reservedCover = null;
            }
            enemyGun.gameObject.SetActive(true);
            gameObject.SetActive(true);
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
            if(currentState == State.Dead)
            { return; }

            if (GameManager.Instance.GetGameState() != GameState.Running)
            {
                //Debug.Log("* * Game not running * *");
                ResetAnimation();
                animator.SetBool("idle", true);
                return;
            }
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
                        initCompleted = true;
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
                case State.InCoverWait:
                    ResetAnimation();
                    animator.SetBool("idle", true);
                    InCoverWaitBehavior();
                    break;

                case State.MovingToShootPoint:
                    ResetAnimation();
                    animator.SetBool("run", true);
                    MovingToShootPointBehavior();
                    break;

                case State.Charging:
                    ResetAnimation();
                    animator.SetBool("run", true);
                    ChargingBehavior();
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
            animator.SetBool("dead", false);
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
            //Debug.Log(gameObject.name + " can see - "+ HasLineOfSight());
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
                nextFireTime = (Time.time + 1f / fireRate) + Random.Range(-0.1f, 0.1f);
            }

            if ((currentHealth < maxHealth / 2) && !inCover)
            {
                currentState = State.TakingCover;
            }
        }

        void TakingCoverBehavior()
        {
            if (bestCover == null)
            {
                bestCover = combatPositionProviderObj.GetNearestCover(transform.position, this);
            }
            float distanceToCover = Vector3.Distance(transform.position, bestCover.position);
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if(distanceToPlayer < CLOSE_COMBAT_THRESHOLD)
            {
                inCover = true;
                currentState = State.Combat;
            }
            if (bestCover != null)
            {
                agent.isStopped = false;
                agent.SetDestination(bestCover.position);
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    inCover = true;
                    currentState = State.InCoverWait;
                    coverTimer = coverWaitTime;
                    bestCover = null;
                }
            }
        }

        void InCoverWaitBehavior()
        {
            agent.isStopped = true;
            coverTimer -= Time.deltaTime;

            if (coverTimer <= 0f)
            {
                combatPositionProviderObj.ReleaseCover(reservedCover, this);
                reservedCover = null;

                currentTargetPoint = combatPositionProviderObj.GetNearestShootingPoint(transform.position);
                isTakingCover = false;
                if (currentTargetPoint != null)
                {
                    currentState = State.MovingToShootPoint;
                    agent.isStopped = false;
                    agent.SetDestination(currentTargetPoint.position);
                }
                else
                {
                    currentState = State.Combat;
                }
            }
        }

        void MovingToShootPointBehavior()
        {

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Debug.Log(gameObject.name + " --> Shooting");
                currentState = State.Combat;
            }
        }

        void ChargingBehavior()
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            Vector3 targetPositionAdjusted = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPositionAdjusted);

            if (Time.time >= nextFireTime)
            {
                ShootAtPlayer();
                nextFireTime = Time.time + 1f / fireRate;
            }
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
            int index = randomInts.GetNext();
            //Debug.Log(" Shoot Index = " + index);
            audioSource.Stop();
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.clip =shootAudio[index];
            audioSource.Play();
            //Debug.Log(gameObject.name + " Shooting ");
            enemyGun.Shoot(dir, shootCount[index]);
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;

            if (currentState == State.Dead) return;

            float healthPercent = currentHealth / maxHealth;

            // LOW HEALTH BEHAVIOR
            if (healthPercent < lowHealthThreshold)
            {
                DecideLowHealthBehavior();
            }
            else
            {
                // PROBABILITY-BASED REACTION
                if (Random.value < hitReactionProbability)
                {
                    Debug.Log(" --> On Hit | Random.value = " + Random.value);
                    StartTakingCover();
                }
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void DecideLowHealthBehavior()
        {
            float choice = Random.value;
            Debug.Log(" --> Low Health | choice = "+ choice);
            if (choice < 0.5f)
            {
                StartTakingCover(); 
            }
            else
            {
                StartCharging();
            }
        }

        void StartTakingCover()
        {
            if (combatPositionProviderObj == null || isTakingCover) return;
            Debug.Log(" --> StartTakingCover");
            isTakingCover = true;
            Transform cover = combatPositionProviderObj.GetNearestCover(transform.position, this);

            if (cover == null)
            {
                Debug.Log(" --> cover == null");
                StartCharging();
                return;
            }

            float distanceToCover = Vector3.Distance(transform.position, cover.position);
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToCover > distanceToPlayer)
            {
                Debug.Log(" --> Dist");
                combatPositionProviderObj.ReleaseCover(cover, this);
                StartCharging();
                return;
            }

            reservedCover = cover;
            currentTargetPoint = cover;

            currentState = State.TakingCover;
            agent.isStopped = false;
            agent.SetDestination(currentTargetPoint.position);
        }

        void StartCharging()
        {
            Debug.Log(" --> StartCharging");
            currentState = State.Charging;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        void Die()
        {
            
            ResetAnimation();
            animator.SetBool("dead", true);
            currentState = State.Dead;
            GameObject gunToDrop = GameManager.Instance.GetGunPrefabToDrop(soldierType);
           
            foreach(GameObject g in objectsToDisableOnDeath)
            {
                g.SetActive(false);
            }
            agent.isStopped = true;    
            gunToDrop.transform.position = enemyEyesPos.transform.position + new Vector3(0,2,0);
            Debug.Log(" gunToDrop.transform.position = " + gunToDrop.transform.position);
            Debug.Log("enemyEyesPos.transform.position = " + enemyEyesPos.transform.position);
            Debug.Log("Enemy died!");
            if (reservedCover != null)
            {
                combatPositionProviderObj.ReleaseCover(reservedCover, this);
                reservedCover = null;
            }
            enemyGun.gameObject.SetActive(false);
            //GameManager.Instance.SetGameState(GameState.PlayerKilled);
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

        public void ProcessDamage(float damageMultiplyer, float damage, out bool isDead)
        {
            Debug.Log($" In Enemy Ai damageMultiplyer = {damageMultiplyer} | damage = {damage}");
            float totalDamage = damageMultiplyer * damage;
            TakeDamage(totalDamage);
            isDead = (currentHealth <= 0) ? true : false;
        }
    }
}
