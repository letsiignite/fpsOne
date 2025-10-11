using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjectPool : MonoBehaviour
{
    public GameObject bulletHoles;
    private Gun_Controller gunController;
    public Queue<GameObject> pool = new Queue<GameObject>();

    public GameObject GetObject(Vector3 targetPoint, Vector3 normal)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return Instantiate(bulletHoles, targetPoint, Quaternion.FromToRotation(Vector3.up, normal));

        }
    }
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
