using Game;
using UnityEngine;

public class MachineGunTurret : MonoBehaviour, IDamageHandler
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Target Settings")]
    public Transform player;
    public float detectionRange = 30f;
    public LayerMask obstructionMask;
    public string playerTag = "Player";

    [Header("Gun Settings")]
    public Transform gunHead;
    public Transform firePoint;
    public float rotationSpeed = 8f;
    public float fireRate = 0.1f;
    public float damage = 10f;
    public float rayDamage = 10f;
    public float rayDistance = 100f;
    [SerializeField]
    private AudioClip shootAudioClip;
    [SerializeField]
    private AudioSource shootAudioSource;
    [SerializeField]
    private GameObject OnDestroyVfx;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 40f;

    private float fireTimer = 0f;
    private bool isDestroyed = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p) player = p.transform;
        }
    }

    void Update()
    {
        if (isDestroyed) return;      // Turret dead → skip logic
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (HasLineOfSight())
            {
                RotateTowardsPlayer();
                TryShoot();
            }
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 dir = (player.position - firePoint.position).normalized;
        float dist = Vector3.Distance(firePoint.position, player.position);

        if (Physics.Raycast(firePoint.position, dir, out RaycastHit hit, dist))
        {
            if (hit.collider.CompareTag(playerTag))
                return true;

            return false;
        }

        return false;
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - gunHead.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        //Debug.Log("Rotating to player");
        gunHead.rotation = Quaternion.Slerp(gunHead.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    private void TryShoot()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            ShootFireRay();
            fireTimer = 0f;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }

        Destroy(bullet, 4f);
    }

    private void ShootFireRay()
    {
        if (isDestroyed || GameManager.Instance.GetGameState() != GameState.Running)
            return;


        Debug.Log("Shooting");
        Vector3 direction = firePoint.forward;

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, rayDistance))
        {
            Debug.Log(" Hitting "+ hit.collider.tag);
            // If the ray hits the player, apply damage
            if (hit.collider.CompareTag(playerTag))
            {

                if (hit.collider.gameObject.GetComponent<DamageReceiver>())
                {
                    Debug.Log(" Set damage " +damage);
                    hit.collider.gameObject.GetComponent<DamageReceiver>().ReceiveRayHitDamage(damage, transform.position);

                }
            }

            // Optional: Debug draw the shot
            Debug.DrawLine(firePoint.position, hit.point, Color.red, 0.1f);
        }
        else
        {
            // Draw ray to full distance when missing
            Debug.DrawLine(firePoint.position, firePoint.position + direction * rayDistance, Color.black, 0.1f);
        }
    }

    // --------------------------
    // DAMAGE SYSTEM
    // --------------------------
    public void ProcessDamage(float damageMultiplyer, float damage)
    {
        if (isDestroyed) return;

        currentHealth -= (damageMultiplyer * damage);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDestroyed = true;

        // Optional: disable gun visuals, smoke, explosion, etc.
        OnDestroyVfx.SetActive(true);
        Debug.Log("Turret destroyed!");

        // Stop rotating and shooting by disabling this script or just skipping logic
        // You can animate destruction here if needed
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * detectionRange);
    }
}
