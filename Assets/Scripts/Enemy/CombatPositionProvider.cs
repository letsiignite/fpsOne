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
            Transform playerPos,
            Vector3 enemyPos,
            EnemyAI requester
            )
        {
            Transform[] points = (type == CombatPositionType.Cover)
                ? coverPoints
                : shootingPoints;

            Transform bestPoint = null;
            float bestScore = Mathf.Infinity;

            float PlayerDistToenemy = Vector3.Distance(enemyPos, playerPos.position);
            if (type == CombatPositionType.Cover && PlayerDistToenemy < 5f)
            {
                /*EnemyDialogs.clip = charge;
                EnemyDialogs.Play();*/
                return null; // force charging
            }
            Debug.Log("++ Checking points");
            foreach (Transform point in points)
            {
                if (point == null) continue;

                // ❌ Skip reserved
                if (IsReserved(point)) continue;

                // ---------------- STATE RULES ----------------

                float pointDistanceToPlayer = Vector3.Distance(point.position , playerPos.position);
                float pointDistanceToEnemy = Vector3.Distance(point.position , enemyPos);
                Debug.Log(point.name+ " - pointDistanceToEnemy = " + pointDistanceToEnemy + " | IsCoverSafe(point, playerPos) = "+ IsCoverSafe(point, playerPos.position));
                if (type == CombatPositionType.Cover)
                {
                    // ❌ Skip covers that are farther from player than current enemy position
                    if (pointDistanceToEnemy > PlayerDistToenemy)
                        continue;

                    // Must block line of sight
                    if (!IsCoverSafe(point, playerPos.position))
                        continue;
                }
                else // Shooting
                {
                    if (!HasLineOfSight(point, playerPos.position))
                        continue;
                }

                // ---------------- SCORING ----------------

                float score = pointDistanceToEnemy + Random.Range(0f, randomness);
                Debug.Log("++ score = "+ score+ " | bestScore = "+ bestScore);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestPoint = point;
                }
            }

            // ✅ Reserve before returning
            
            if (bestPoint != null)
            {
                bool isPlayerCloserThanCover = PlayerDistToenemy < (Vector3.Distance(enemyPos, bestPoint.position));
                if (!isPlayerCloserThanCover)
                {
                   /* EnemyDialogs.clip = onHit;
                    EnemyDialogs.Play();*/
                    Reserve(bestPoint, requester);
                }
                else
                {
                    bestPoint = null; // Player is closer to the AI, so AI connot get to cover.
                }
                
            }
            else
            {
                /*EnemyDialogs.clip = charge;
                EnemyDialogs.Play();*/
            }

            return bestPoint;
        }

        public Transform GetClosestShootingPoint(Vector3 requesterPos, Enemy.EnemyAI requester)
        {
            Transform bestPoint = null;
            float bestDist = float.MaxValue;

            foreach (Transform point in shootingPoints)
            {
                if (point == null) continue;

                // 🔒 Skip already reserved points
                if (reservedPoints.ContainsKey(point))
                    continue;

                float dist = Vector3.Distance(point.position , requesterPos); 

                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestPoint = point;
                }
            }

            // 🔒 Reserve it
            if (bestPoint != null)
            {
                reservedPoints[bestPoint] = requester;
            }

            return bestPoint;
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
            foreach (var pt in reservedPoints)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(pt.Key.position, 0.3f);
            }
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