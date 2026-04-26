using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject soldierPrefab;
    public Transform[] spawnPoints;
    public CombatPositionProvider provider;
    public Transform player;

    [Header("Settings")]
    public int squadSize = 4;
    public float spawnInterval = 10f;

    private List<GameObject> activeSoldiers = new List<GameObject>();
    private Coroutine spawnRoutine;
    private bool isActive = false;

    // =========================================================
    // 🔹 START / STOP
    // =========================================================

    private void OnEnable()
    {
        ActivateSpawner();
    }

    public void ActivateSpawner()
    {
        if (isActive) return;

        isActive = true;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawner()
    {
        if (!isActive) return;

        isActive = false;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }
    }

    // =========================================================
    // 🔹 MAIN LOOP
    // =========================================================

    IEnumerator SpawnLoop()
    {
        while (isActive && GameManager.Instance.GetGameState() == GameState.Running)
        {
            SpawnSquad();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // =========================================================
    // 🔹 SPAWN LOGIC
    // =========================================================

    void SpawnSquad()
    {
        for (int i = 0; i < squadSize; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];

            GameObject soldier = Instantiate(
                soldierPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            activeSoldiers.Add(soldier);

            Enemy.EnemyAI ai = soldier.GetComponent<Enemy.EnemyAI>();
            ai.SetPlayer(player);
            AssignInitialPosition(ai, i);
        }
    }

    void AssignInitialPosition(Enemy.EnemyAI ai, int index)
    {
        CombatPositionType type =
            (index % 2 == 0) ? CombatPositionType.Cover : CombatPositionType.Shooting;

        Transform point = provider.GetBestPosition(
            type,
            player.position,
            ai.transform.position,
            ai
        );

        if (point == null)
        {
            ai.SendMessage("StartCharging", SendMessageOptions.DontRequireReceiver);
            return;
        }
        ai.SetInitialPosition(point);
        
    }

    // =========================================================
    // 🔹 RESET (IMPORTANT)
    // =========================================================

    public void ResetSpawner()
    {
        StopSpawner();

        foreach (GameObject soldier in activeSoldiers)
        {
            if (soldier != null)
            {
                Destroy(soldier);
            }
        }

        activeSoldiers.Clear();
    }
}