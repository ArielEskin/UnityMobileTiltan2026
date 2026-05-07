using UnityEngine;
using System.Collections.Generic;

public class BasicObjectPooler : MonoBehaviour
{
    public GameObject prefab;
    public int initialPoolSize = 10; 
    
    private Queue<GameObject> pooledObjects; 

    void Start()
    {
        pooledObjects = new Queue<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pooledObjects.Enqueue(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        if (pooledObjects.Count > 0)
        {
            GameObject obj = pooledObjects.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        GameObject newObj = Instantiate(prefab);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false); 
            pooledObjects.Enqueue(obj);
        }
    }
}