using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DamageDirection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private RectTransform indicatorContainer;
    [SerializeField] private DamageIndicator indicatorPrefab;

    [Header("Settings")]
    [SerializeField] private int poolSize = 8;
    [SerializeField] private float indicatorDuration = 0.75f;

    private readonly Queue<DamageIndicator> indicatorPool = new();

    void Awake()
    {
        if (!playerCamera)
            playerCamera = Camera.main;

        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var indicator = Instantiate(indicatorPrefab, indicatorContainer);
            indicator.gameObject.SetActive(false);
            indicatorPool.Enqueue(indicator);
        }
    }

    /// <summary>
    /// Call this when player receives damage
    /// </summary>
    public void ShowDamage(Vector3 damageSourcePosition)
    {
        if (indicatorPool.Count == 0)
            return;

        DamageIndicator indicator = indicatorPool.Dequeue();
        indicator.gameObject.SetActive(true);

        Vector3 direction = damageSourcePosition - playerCamera.transform.position;
        direction.y = 0f;

        float angle = Vector3.SignedAngle(
            playerCamera.transform.forward,
            direction,
            Vector3.up
        );

        indicator.Initialize(angle, indicatorDuration, ReturnToPool);
    }

    void ReturnToPool(DamageIndicator indicator)
    {
        indicator.gameObject.SetActive(false);
        indicatorPool.Enqueue(indicator);
    }
}
