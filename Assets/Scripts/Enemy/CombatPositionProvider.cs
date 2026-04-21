using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class CombatPositionProvider : MonoBehaviour
    {
        [SerializeField] private Transform[] coverPoints;
        [SerializeField] private Transform[] shootingPoints;
        [SerializeField] private Transform player;
        [Tooltip("anything behind which enemy can hide")]
        [SerializeField] private LayerMask obstructionMask; // Walls, obstacles or anything behind which enemy can hide.
        [SerializeField] private float coverCheckHeight = 1.5f;
        [SerializeField] private AudioSource EnemyDialogs;
        [SerializeField] private AudioClip onHit;
        [SerializeField] private AudioClip charge;
        private Dictionary<Transform, EnemyAI> reservedCovers = new Dictionary<Transform, EnemyAI>();

        public Transform GetNearestCover(Vector3 fromPosition, EnemyAI requester)
        {
            Transform best = null;
            float shortestDist = Mathf.Infinity;
            EnemyDialogs.Stop();
            EnemyDialogs.clip = onHit;
            EnemyDialogs.Play();
            foreach (var cover in coverPoints)
            {
                // ❌ Skip if reserved by someone else
                if (reservedCovers.ContainsKey(cover) && reservedCovers[cover] != requester)
                    continue;

                // ❌ Skip unsafe cover
                if (!IsCoverSafe(cover))
                    continue;

                float dist = Vector3.Distance(fromPosition, cover.position);

                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    best = cover;
                }
            }

            // ✅ Reserve selected cover
            if (best != null)
            {
                reservedCovers[best] = requester;
            }

            return best;
        }

        public Transform GetNearestShootingPoint(Vector3 fromPosition)
        {
            Transform best = null;
            float shortestDist = Mathf.Infinity;
            EnemyDialogs.Stop();
            EnemyDialogs.clip = charge;
            EnemyDialogs.Play();
            foreach (var point in shootingPoints)
            {
                float dist = Vector3.Distance(fromPosition, point.position);

                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    best = point;
                }
            }

            return best;
        }

        public void ReleaseCover(Transform cover, EnemyAI requester)
        {
            if (cover == null) return;

            if (reservedCovers.ContainsKey(cover) && reservedCovers[cover] == requester)
            {
                reservedCovers.Remove(cover);
            }
        }

        bool IsCoverSafe(Transform cover)
        {
            if (player == null) return false;

            Vector3 coverPos = cover.position + Vector3.up * coverCheckHeight;
            Vector3 playerPos = player.position + Vector3.up * coverCheckHeight;

            Vector3 dir = coverPos - playerPos;
            float distance = dir.magnitude;

            RaycastHit hit;

            if (Physics.Raycast(playerPos, dir.normalized, out hit, distance, obstructionMask))
            {
                if (hit.transform != cover)
                {
                    return true; // something blocks view → good cover
                }
            }

            return false;
        }

        void OnDrawGizmos()
        {
            if (coverPoints == null) return;

            foreach (var cover in coverPoints)
            {
                Gizmos.color = reservedCovers.ContainsKey(cover) ? Color.red : Color.green;
                Gizmos.DrawSphere(cover.position, 0.3f);
            }
        }
    }
}