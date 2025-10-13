using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjectPool : MonoBehaviour
{
    public GameObject bulletHoles;
    public List<GameObject> pool = new List<GameObject>();

    //void Start()
    //{
    //    for (int i = 0; i < 20; i++)
    //    {
    //        GameObject newObj = Instantiate(bulletHoles);
    //        newObj.SetActive(false);
    //        pool.Add(newObj);
    //    }
    //}
    public GameObject GetObject(Vector3 targetPoint, Vector3 normal)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = targetPoint + normal * 0.01f;
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
                obj.SetActive(true);
                return obj;
            }   
        }
        {
            GameObject newObj = Instantiate(bulletHoles, targetPoint, Quaternion.FromToRotation(Vector3.up, normal));
            pool.Add(newObj);
            return newObj;
        }

    }
}
