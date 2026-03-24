using System.Collections.Generic;
using UnityEngine;

public class XPOrbPool : MonoBehaviour
{
    private GameObject orbPrefab;
    private int poolSize;
    private bool canExpand;
    private List<GameObject> pool;
    private Transform poolParent;
    private bool isInitialized = false;

    public void Initialize(GameObject prefab, int size, bool expand)
    {
        if (isInitialized)
        {
            Debug.LogWarning("XPOrbPool déjà initialisé !");
            return;
        }

        orbPrefab = prefab;
        poolSize = size;
        canExpand = expand;
        InitializePool();
        isInitialized = true;
    }

    void InitializePool()
    {
        poolParent = new GameObject($"{orbPrefab.name} Pool").transform;
        poolParent.SetParent(transform);

        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewOrb();
        }
    }

    GameObject CreateNewOrb()
    {
        GameObject orb = Instantiate(orbPrefab, poolParent);
        orb.SetActive(false);
        pool.Add(orb);
        return orb;
    }

    public GameObject GetOrb(Vector3 position)
    {
        foreach (GameObject orb in pool)
        {
            if (orb != null && !orb.activeInHierarchy)
            {
                orb.transform.position = position;
                orb.SetActive(true);
                return orb;
            }
        }

        if (canExpand)
        {
            GameObject newOrb = CreateNewOrb();
            newOrb.transform.position = position;
            newOrb.SetActive(true);
            return newOrb;
        }

        return null;
    }

    public void ReturnOrb(GameObject orb)
    {
        if (orb != null)
        {
            orb.SetActive(false);
            orb.transform.SetParent(poolParent);
        }
    }

    public int GetActiveCount()
    {
        int count = 0;
        foreach (GameObject orb in pool)
        {
            if (orb != null && orb.activeInHierarchy)
                count++;
        }
        return count;
    }
}