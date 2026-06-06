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
        private float fireRateAlterationForCharge = 0.3f;
        [SerializeField]
        private List<GameObject> objectsToDisableOnDeath;

        private enum State
        {
            MovingToPoint,
            Idle,
            Combat,
            TakingCover,
            InCoverWait,
            PeekState,
            MovingToShootPoint,
            Charging,
            Dead
        }

        private State currentState;
        private const float CLOSE_COMBAT_THRESHOLD = 10f;
        private Vector3 dir;
        private Vector3 startPos;

        [Header("Shoot Randomization")]
        [SerializeField]
        private AudioClip[] shootAudio = new AudioClip[4];
        [SerializeField]
        private int[] shootCount = new int[4];
        private List<int> numbers = new List<int> { 0, 1, 2, 3};

        private CombatPositionProvider provider;
        

        [SerializeField] 
        private float hitReactionProbability = 0.6f;
        [SerializeField] 
        private float lowHealthThreshold = 0.4f;
        [SerializeField] 
        private float coverWaitTime = 2f;

        private float coverTimer;

        private UniqueRandom<int> randomInts;
        private bool initCompleted = false;
        private Transform reservedPoint;
        private bool isTakingCover = false;
        private Transform currentTarget;
        private EnemyAssets enemyAssets;
        private Transform peekPoint;
        private bool isMovingToPeek = false;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            startPos = transform.position;
            currentHealth = maxHealth;
            currentState = State.MovingToPoint;
            if (endPoint != null)
            {
                agent.SetDestination(endPoint.position);
            }
            enemyAssets = GameObject.FindAnyObjectByType<EnemyAssets>();
            randomInts = new UniqueRandom<int>(numbers);
            provider = GameObject.FindAnyObjectByType<CombatPositionProvider>();
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
            enemyGun.gameObject.SetActive(true);
            animator.SetBool("idle", true);
            animator.Update(0f);
            agent.SetDestination(endPoint.position);
            foreach (GameObject g in objectsToDisableOnDeath)
            {
                g.SetActive(true);
            }

            if (reservedPoint != null)
            {
                ReleaseCurrentPoint();
                reservedPoint = null;
            }
            enemyGun.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        void ReleaseCurrentPoint()
        {
            if (reservedPoint != null)
            {
                provider.Release(reservedPoint, this);
                reservedPoint = null;
            }
        }

        public void SetPlayer(Transform player)
        {
            this.player = player;

        }

        public void SetInitialPosition(Transform point)
        {
            reservedPoint = point;
            currentTarget = point;
           
            endPoint = point;
            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = false;
            agent.SetDestination(point.position);

            currentState = State.TakingCover; // or MovingToShootPoint depending on type
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log(" -- OnTriggerEnter | Tag = "+ other.tag);
            if (other.CompareTag("Player") && currentState != State.Dead)
            {
                FacePlayer();
            }
        }

        void OnTriggerStay(Collider other)      // No need to rotate in update method
        {
            if (other.CompareTag("Player") && currentState != State.Dead)
            {
                FacePlayer();
            }
        }

        void FacePlayer()
        {
            //return;
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
                    //DetectPlayer();
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
                case State.PeekState:
                    ResetAnimation();
                    animator.SetBool("run", true);
                    PeekBehavior();
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
            PeekBehavior(); // Peek and shoot
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

        void ChargingBehavior()
        {
            //Debug.Log(" --> ChargingBehavior");
            if (player == null) return;

            // Always chase moving player
            agent.isStopped = false;
            // 🔥 Zig-zag movement
            Vector3 toPlayer = (player.position - transform.position).normalized;

            // Perpendicular direction (left/right)
            Vector3 side = Vector3.Cross(Vector3.up, toPlayer);

            // Oscillation
            float zigZagSpeed = Random.Range(4f, 7f);
            float zigZagAmount = Random.Range(1f, 2f);
            agent.speed = 7f;
            float offset = Mathf.Sin(Time.time * zigZagSpeed) * zigZagAmount;
            //Debug.Log(" --> zigZagSpeed = " + zigZagSpeed + " || zigZagAmount = " + zigZagAmount+ " | offset = "+ offset);
            // Final target
            Vector3 target = player.position + side * offset;

            agent.SetDestination(target);

            float dist = Vector3.Distance(transform.position, player.position);

            // 🔥 1. Close combat → switch to shooting
            if (dist < CLOSE_COMBAT_THRESHOLD)
            {
                agent.isStopped = true;
                currentState = State.Combat;
                return;
            }

            // 🔥 2. Lost line of sight → stop dumb chasing
            if (!HasLineOfSight())
            {
                currentState = State.Idle;
                return;
            }


            if (Time.time >= nextFireTime)
            {
                ShootAtPlayer();
                nextFireTime = (Time.time + 1f / (fireRate * fireRateAlterationForCharge)) + Random.Range(-0.1f, 0.1f);
            }

            // 🔥 3. Optional: if mid-range → take cover instead of charging blindly
            if (dist > CLOSE_COMBAT_THRESHOLD && dist < detectionRange * 0.8f)
            {
                // small probability to break charge → more natural
                if (Random.value < 0.02f)
                {
                    StartTakingCover();
                    return;
                }
            }
        }

        void TakingCoverBehavior()
        {
            agent.speed = 5f;
            if (reservedPoint == null)
            {
                currentState = State.Combat;
                return;
            }

            if (HasLineOfSight() &&
                agent.remainingDistance > Vector3.Distance(transform.position, player.position))
            {
                currentState = State.Combat;
                return;
            }

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentState = State.InCoverWait;
                coverTimer = coverWaitTime;
            }
        }

        void InCoverWaitBehavior()
        {
            agent.isStopped = true;
            coverTimer -= Time.deltaTime;
            RotateTowardsPlayer(1f);
            if (HasLineOfSight())
            {
                currentState = State.PeekState;
            }
            if (coverTimer <= 0f)
            {
                provider.Release(reservedPoint, this);
                reservedPoint = null;
                RotateTowardsPlayer(1f);
                currentState = State.PeekState;
            }
        }

        void PeekBehavior()
        {
            // =========================================================
            // 🔹 Step 1: Get closest shooting point (once)
            // =========================================================
            if (peekPoint == null)
            {
                peekPoint = provider.GetClosestShootingPoint(this.transform.position, this);

                if (peekPoint == null)
                {
                    currentState = State.Combat;
                    return;
                }

                agent.isStopped = false;
                agent.SetDestination(peekPoint.position);
                isMovingToPeek = true;
            }

            // =========================================================
            // 🔹 Step 2: Move to peek point
            // =========================================================
            if (isMovingToPeek)
            {
                //RotateTowardsPlayer(5f);

                if (HasLineOfSight() && 
                    Vector3.Distance(transform.position, peekPoint.position) > Vector3.Distance(transform.position, player.position))
                {
                    currentState = State.Combat;
                    peekPoint = null;
                }

                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    isMovingToPeek = false;
                    agent.isStopped = true;
                }
            }
            else
            {
                // =========================================================
                // 🔹 Step 3: Wait & watch player
                // =========================================================
                RotateTowardsPlayer(8f);

                if (HasLineOfSight())
                {
                    currentState = State.Combat;
                    peekPoint = null;
                }
            }
        }

        void RotateTowardsPlayer(float speed)
        {
            if (player == null) return;

            Vector3 dir = player.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.01f) return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * speed
            );
        }

        void MovingToShootPointBehavior()
        {
            if (reservedPoint == null)
            {
                currentState = State.Combat;
                return;
            }
            if (HasLineOfSight() &&
                   agent.remainingDistance > Vector3.Distance(transform.position, player.position))
            {
                currentState = State.Combat;
            }
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentState = State.Combat; // start shooting
            }
        }

        void StartCharging()
        {
            //enemyAssets.PlayAudioForAttackBehavior(true);
            ReleaseCurrentPoint();
            if (agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
            }
            agent.isStopped = false;
            agent.SetDestination(player.position);

            currentState = State.Charging;
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
            dir = (player.position - enemyGun.shootPoint.transform.position).normalized;
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
                StartTakingCover();
            }
        }

        void StartTakingCover()
        {
            //enemyAssets.PlayAudioForAttackBehavior(false);
            ReleaseCurrentPoint();
            Debug.Log(" ++ Req for cover");
            Transform cover = provider.GetBestPosition(
                CombatPositionType.Cover,
                player,
                transform.position,
                this
            );

            if (cover == null)
            {
                Debug.Log(" ++ NULL for cover");
                StartCharging();
                return;
            }

            reservedPoint = cover;
            currentTarget = cover;

            agent.isStopped = false;
            agent.SetDestination(cover.position);

            currentState = State.TakingCover;
        }

        void Die()
        {
            
            ResetAnimation();
            animator.SetBool("dead", true);
            enemyGun.gameObject.SetActive(false);
            currentState = State.Dead;
            GameObject gunToDrop = GameManager.Instance.GetGunPrefabToDrop(soldierType);
           
            foreach(GameObject g in objectsToDisableOnDeath)
            {
                g.SetActive(false);
            }
            agent.isStopped = true;    
            gunToDrop.transform.position = enemyEyesPos.transform.position + new Vector3(0,2,0);
            enemyGun.gameObject.SetActive(false);
            Debug.Log(" gunToDrop.transform.position = " + gunToDrop.transform.position);
            Debug.Log("enemyEyesPos.transform.position = " + enemyEyesPos.transform.position);
            Debug.Log("Enemy died!");
            if (reservedPoint != null)
            {
                ReleaseCurrentPoint();
                reservedPoint = null;
            }
            
            enemyAssets.PlayRandomAudioOnDeath();
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
