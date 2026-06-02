using Enemy;
using Game;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class SniperEnemy : MonoBehaviour, IDamageHandler, IEnemySolder
{
    private enum State
    {
        Idle,
        Alert,
        Shooting,
        MovingToCover,
        Dead
    }

    [Header("References")]
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform shootPoint;
    [SerializeField]
    private NavMeshAgent agent;

    [Header("Vision")]
    [SerializeField]
    private float viewDistance = 40f;
    [SerializeField]
    private float viewAngle = 45f;
    [SerializeField]
    private LayerMask visionMask;

    [Header("Shooting")]
    [SerializeField]  
    private float fireCooldown = 2f;
    [SerializeField]
    private AudioClip shootSound;
    [SerializeField]
    private AudioSource audioSource;
    public int damage = 50;

    [Header("Cover")]
    [SerializeField]
    private List<Transform> coverPoints;

    [SerializeField]
    private Animator animator;
    private State currentState = State.Idle;
    private float lastFireTime;

    private UniqueRandom<Transform> randomCover;
    private Transform currentCover;
    private Vector3 startPos;
    private Quaternion startRotation;
    public int maxHealth = 100;
    private float currentHealth;
    [SerializeField] GameObject enemyGun;

    [SerializeField]
    private List<GameObject> objectsToDisableOnDeath;
    public GameObject enemyEyesPos;
    [Tooltip("What type of gun this solder has")]
    public DropGunType soldierType;

    [Header("Laser")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private float laserDuration = 1f;
    [SerializeField] private Color laserColor = Color.red;

    private bool isPreparingShot = false;

    void Start()
    {
        randomCover = new UniqueRandom<Transform>(coverPoints);
        animator = GetComponent<Animator>();
        laserLine.enabled = false;
        startPos = transform.position;
        startRotation = transform.rotation;
    }

    private void ResetAnimation()
    {
        animator.SetBool("run", false);
        animator.SetBool("idle", false);
        animator.SetBool("shoot", false);
        animator.SetBool("dead", false);
    }

    public void Reset()
    {
        transform.position = startPos;
        transform.rotation = startRotation;
        currentHealth = maxHealth;
        currentState = State.Idle;

        agent.ResetPath();
        agent.Warp(startPos);
        agent.isStopped = false;

        foreach (GameObject g in objectsToDisableOnDeath)
        {
            g.SetActive(true);
        }

        ResetAnimation();
        enemyGun.SetActive(true);
        animator.SetBool("idle", true);
        animator.Update(0f);
        gameObject.SetActive(true);

    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                ResetAnimation();
                animator.SetBool("idle", true);
                HandleIdle();
                break;

            case State.Alert:
                ResetAnimation();
                animator.SetBool("idle", true);
                HandleAlert();
                break;

            case State.Shooting:
                ResetAnimation();
                animator.SetBool("shoot", true);
                HandleShooting();
                break;

            case State.MovingToCover:
                ResetAnimation();
                animator.SetBool("run", true);
                HandleMoveToCover();
                break;
        }
    }

    // ---------------- STATES ----------------

    void HandleIdle()
    {
        if (CanSeePlayer())
        {
            currentState = State.Alert;
        }
    }

    void HandleAlert()
    {
        LookAtPlayer();
        //Debug.Log(" CanSeePlayer() = " + CanSeePlayer()+ 
        //   " | Time.time > lastFireTime + fireCooldown = "+(Time.time > lastFireTime + fireCooldown));
        if (CanSeePlayer() && Time.time > lastFireTime + fireCooldown && !isPreparingShot)
        {
            StartCoroutine(PrepareAndShoot());
        }
    }

    IEnumerator PrepareAndShoot()
    {
        isPreparingShot = true;

        currentState = State.Shooting;
        ResetAnimation();
        animator.SetBool("shoot", true);

        float timer = 0f;
        laserLine.enabled = true;
        //laserLine.startColor = laserColor;
        //laserLine.endColor = laserColor;

        while (timer < laserDuration)
        {
            if (player != null)
            {
                Vector3 start = shootPoint.position;
                Vector3 end = player.position;

                laserLine.SetPosition(0, start);
                laserLine.SetPosition(1, end);

                LookAtPlayer();
            }

            timer += Time.deltaTime;
            yield return null;
        }

        laserLine.enabled = false;

        // 🔥 Shoot after laser warning
        audioSource.Stop();
        audioSource.clip = shootSound;
        audioSource.Play();

        ShootOnce();
        lastFireTime = Time.time;

        isPreparingShot = false;

        Invoke("ShootAndScoot", 1f);
    }

    void HandleShooting()
    {
        if (!isPreparingShot && CanSeePlayer() && Time.time > lastFireTime + fireCooldown)
        {
            StartCoroutine(PrepareAndShoot());
        }
    }

    private void ShootAndScoot()
    {
        currentCover = randomCover.GetNext();
        agent.SetDestination(currentCover.position);

        currentState = State.MovingToCover;
    }

    void HandleMoveToCover()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = State.Alert;
        }
    }

    // ---------------- ACTIONS ----------------

    void ShootOnce()
    {
        if (Physics.Raycast(shootPoint.position,
                             (player.position - shootPoint.position).normalized,
                             out RaycastHit hit,
                             viewDistance,
                             visionMask))
        {
            Debug.Log(" Sniper Shot at = " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.GetComponent<DamageReceiver>() != null)
            {
                hit.collider.gameObject.GetComponent<DamageReceiver>().ReceiveRayHitDamage(damage, transform.position, out bool none);
                //Debug.Log(" Calling - ReceiveRayHitDamage "+damage);
            }
        }
    }

    void LookAtPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 5f
        );
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = player.position - transform.position;

        if (dirToPlayer.magnitude > viewDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle * 0.5f)
            return false;

        if (Physics.Raycast(transform.position + Vector3.up,
                            dirToPlayer.normalized,
                            out RaycastHit hit,
                            viewDistance,
                            visionMask))
        {
            //Debug.Log(" Hit at = "+hit.transform.name);
            return hit.transform.tag == "Player";
        }

        return false;
    }

    void OnDrawGizmos()
    {
        // -------- VIEW DISTANCE --------
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // -------- VIEW ANGLE --------
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);

        // -------- PLAYER LINE --------
        if (player != null)
        {
            Gizmos.color = CanSeePlayer() ? Color.red : Color.gray;
            Gizmos.DrawLine(shootPoint.position, player.position);
        }

        // -------- COVER POINTS --------
        if (coverPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var cover in coverPoints)
            {
                if (cover != null)
                    Gizmos.DrawSphere(cover.position, 0.4f);
            }
        }

        // -------- CURRENT TARGET COVER --------
        if (currentCover != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(currentCover.position, 0.6f);
            Gizmos.DrawLine(transform.position, currentCover.position);
        }
    }

    public void ProcessDamage(float damageMultiplyer, float damage, out bool isDead)
    {
        Debug.Log($" In Sniper damageMultiplyer = {damageMultiplyer} | damage = {damage}");
        float totalDamage = damageMultiplyer * damage;
        TakeDamage(totalDamage);
        isDead = (currentHealth == 0) ? true : false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        ResetAnimation();
        animator.SetBool("dead", true);
        currentState = State.Dead;
        GameObject gunToDrop = GameManager.Instance.GetGunPrefabToDrop(soldierType);
        enemyGun.SetActive(false);
        foreach (GameObject g in objectsToDisableOnDeath)
        {
            g.SetActive(false);
        }
        agent.isStopped = true;
        gunToDrop.transform.position = enemyEyesPos.transform.position + new Vector3(0, 2, 0);
        Debug.Log(" gunToDrop.transform.position = " + gunToDrop.transform.position);
        Debug.Log("enemyEyesPos.transform.position = " + enemyEyesPos.transform.position);
        Debug.Log("Enemy died!");
        //GameManager.Instance.SetGameState(GameState.PlayerKilled);
        //Destroy(gameObject);
    }
}
