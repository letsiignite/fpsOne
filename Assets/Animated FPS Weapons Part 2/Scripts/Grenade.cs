using Game;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Grenade : MonoBehaviour
{

	[SerializeField] private float delay = 3f;
	[SerializeField] private float radius = 10f;
	[SerializeField] private float force = 500f;
	[SerializeField] private float maxDamage = 70f;

	[SerializeField] private GameObject explosionEffect;

	[SerializeField] private LayerMask damageableLayerMask;
	[SerializeField] private LayerMask obstacleLayerMask;

	private float countdown;
	private bool hasExploded = false;
	float upwardsModifier = 1.0f;


	private void Start()
	{
		countdown = delay;
	}

	private void Update()
	{
		countdown -= Time.deltaTime;
		if (countdown <= 0f && !hasExploded)
		{
			Explode();
			hasExploded = true;
		}
	}

	private void Explode()
	{
		Debug.Log(" ** Explode");

        Instantiate(explosionEffect, transform.position, transform.rotation);
		Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);
		foreach (Collider nearbyObject in collidersToDestroy)
		{
			DestructibleObject dest = nearbyObject.GetComponent<DestructibleObject>();
			if (dest != null)
			{
				dest.Destroy();
			}

		}
		Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);
		foreach (Collider nearbyObject in collidersToMove)
		{
			Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.AddExplosionForce(force, transform.position, radius);
			}

		}
		ApplyExplosionDamage();

		Destroy(gameObject);

	}

	private void ApplyExplosionDamage()
	{
		Vector3 explosionPosition = transform.position;
		Collider[] hits = Physics.OverlapSphere(explosionPosition, radius, damageableLayerMask, QueryTriggerInteraction.Ignore);
		Debug.Log(" ** no of objects hit = "+hits.Length);
		for (int i = 0; i < hits.Length; i++)
		{
			Collider col = hits[i];
			if (col == null) continue;

			// Use the collider's world-space center as target point (better than transform position in many cases)
			Vector3 targetPoint = col.bounds.center;
			Vector3 dir = targetPoint - explosionPosition;
			float dist = dir.magnitude;
			if (dist <= 0f) dist = 0.001f; // avoid divide by zero, Heppens some times.

			// Check for occlusion: if any obstacle between explosionPosition and targetPoint, skip damage.
			// We cast a ray towards the target and test only against obstacleLayerMask.
			// If the ray hits something before reaching the target, it's blocked.
			RaycastHit obstacleHit;
			bool blocked = Physics.Raycast(explosionPosition, dir.normalized, out obstacleHit, dist, obstacleLayerMask, QueryTriggerInteraction.Ignore);
			if (blocked)
			{
				// Target is behind an obstacle -> receives no damage
				continue;
			}

			// Not blocked -> apply damage with linear falloff (can replace with different curve)
			float normalizedDistance = Mathf.Clamp01(dist / radius); // 0..1
			float damage = Mathf.Lerp(maxDamage, 0f, normalizedDistance);
            //Debug.Log(" ** Damage - "+col.gameObject.name+ " |  damage = "+ damage);

            var damageable = col.GetComponentInParent<DamageReceiver>();
			if (damageable != null)
			{
				damageable.ReceiveGrenadeDamage(damage, this.transform.position);
			}

			// Apply physics force if there's a rigidbody attached (could be null)
			Rigidbody rb = col.attachedRigidbody;
			if (rb != null && col.tag != "Player")
			{
				rb.AddExplosionForce(force, explosionPosition, radius, upwardsModifier, ForceMode.Impulse);
			}
		}
	}
}
		 	