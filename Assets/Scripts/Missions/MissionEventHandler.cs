using System;
using System.Collections;
using UnityEngine;

public class MissionEventHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject helicopter;
    // 🔥 Call this to execute any method after delay
    public void ExecuteAfterDelay(float delay, Action action)
    {
        StartCoroutine(ExecuteRoutine(delay, action));
    }

    private IEnumerator ExecuteRoutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);

        action?.Invoke();
    }

    private void OnEnable()
    {
        ExecuteAfterDelay(61f, () =>
        {
            helicopter.SetActive(true);
        });
    }
}
