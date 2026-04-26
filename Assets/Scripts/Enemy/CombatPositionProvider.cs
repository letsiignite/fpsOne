using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public enum CombatPositionType
    {
        Cover,
        Shooting
    }

    public class CombatPositionProvider : MonoBehaviour
    {
        [SerializeField] private Transform[] coverPoints;
        [SerializeField] private Transform[] shootingPoints;
        [SerializeField] private Transform player;
        [Tooltip("anything behind which enemy can hide")]
        [SerializeField] private LayerMask obstructionMask; // Walls, obstacles or anything behind which enemy can hide.
        private float randomness = 2f;
        private float maxCheckDistance = 100f;
        [SerializeField] private float coverCheckHeight = 1.5f;
        [SerializeField] private AudioSource EnemyDialogs;
        [SerializeField] private AudioClip onHit;
        [SerializeField] private AudioClip charge;
        //private Dictionary<Transform, EnemyAI> reservedCovers = new Dictionary<Transform, EnemyAI>();
        private Dictionary<Transform, EnemyAI> reservedPoints = new Dictionary<Transform, EnemyAI>();


        public Transform GetBestPosition(
            CombatPositionType type,
            Vector3 playerPos,
            Vector3 enemyPos,
            EnemyAI requester
            )
        {
            Transform[] points = (type == CombatPositionType.Cover)
                ? coverPoints
                : shootingPoints;

            Transform best = null;
            float bestScore = Mathf.Infinity;

            float distToPlayer = Vector3.Distance(enemyPos, playerPos);
            if (type == CombatPositionType.Cover && distToPlayer < 5f)
            {
                EnemyDialogs.clip = charge;
                EnemyDialogs.Play();
                return null; // force charging
            }

            foreach (Transform point in points)
            {
                if (point == null) continue;

                // ❌ Skip reserved
                if (IsReserved(point)) continue;

                // ---------------- STATE RULES ----------------

                float playerDist = Vector3.Distance(point.position, playerPos);

                if (type == CombatPositionType.Cover)
                {
                    // ❌ Skip covers that are farther from player than current enemy position
                    if (playerDist > distToPlayer)
                        continue;

                    // Must block line of sight
                    if (!IsCoverSafe(point, playerPos))
                        continue;
                }
                else // Shooting
                {
                    if (!HasLineOfSight(point, playerPos))
                        continue;
                }

                // ---------------- SCORING ----------------

                float score = playerDist + Random.Range(0f, randomness);

                if (score < bestScore)
                {
                    bestScore = score;
                    best = point;
                }
            }

            // ✅ Reserve before returning
            if (best != null)
            {
                EnemyDialogs.clip = onHit;
                EnemyDialogs.Play();
                Reserve(best, requester);
            }
            else
            {
                EnemyDialogs.clip = charge;
                EnemyDialogs.Play();
            }

            return best;
        }

        #region Reservation
        bool IsReserved(Transform point)
        {
            return reservedPoints.ContainsKey(point);
        }

        void Reserve(Transform point, EnemyAI ai)
        {
            if (!reservedPoints.ContainsKey(point))
            {
                reservedPoints.Add(point, ai);
            }
        }

        public void Release(Transform point, EnemyAI ai)
        {
            if (point == null) return;

            if (reservedPoints.TryGetValue(point, out var owner))
            {
                if (owner == ai)
                {
                    reservedPoints.Remove(point);
                }
            }
        }
        #endregion

        #region Validation
        bool IsCoverSafe(Transform point, Vector3 playerPos)
        {
            Vector3 origin = point.position + Vector3.up * 1.5f;
            Vector3 dir = (playerPos - origin).normalized;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, maxCheckDistance, obstructionMask))
            {
                return !hit.transform.CompareTag("Player");
            }

            return false;
        }

        bool HasLineOfSight(Transform point, Vector3 playerPos)
        {
            Vector3 origin = point.position + Vector3.up * 1.5f;
            Vector3 dir = (playerPos - origin).normalized;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, maxCheckDistance))
            {
                return hit.transform.CompareTag("Player");
            }

            return false;
        }
        #endregion

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
                Gizmos.color = reservedPoints.ContainsKey(cover) ? Color.red : Color.greenYellow;
                Gizmos.DrawSphere(cover.position, 0.3f);
            }

            foreach (var cover in shootingPoints)
            {
                Gizmos.color = Color.blueViolet;
                Gizmos.DrawSphere(cover.position, 0.3f);
            }
        }
    }
}