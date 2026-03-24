using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private GameObject enemyPrefab;
    private int poolSize;
    private bool canExpand;
    private List<GameObject> pool;
    private Transform poolParent;
    private bool isInitialized = false;

    public void Initialize(GameObject prefab, int size, bool expand)
    {
        if (isInitialized)
        {
            Debug.LogWarning("EnemyPool déjà initialisé !");
            return;
        }

        enemyPrefab = prefab;
        poolSize = size;
        canExpand = expand;
        InitializePool();
        isInitialized = true;
    }

    void InitializePool()
    {
        // Créer un parent pour organiser les ennemis dans la hiérarchie
        poolParent = new GameObject($"{enemyPrefab.name} Pool").transform;
        poolParent.SetParent(transform);

        // Initialiser la liste
        pool = new List<GameObject>();

        // Pré-instancier les ennemis
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewEnemy();
        }
    }

    GameObject CreateNewEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, poolParent);
        enemy.SetActive(false);
        pool.Add(enemy);
        return enemy;
    }

    public GameObject GetEnemy()
    {
        // Chercher un ennemi inactif dans le pool
        foreach (GameObject enemy in pool)
        {
            if (enemy != null && !enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        // Si aucun ennemi n'est disponible et que le pool peut s'étendre
        if (canExpand)
        {
            GameObject newEnemy = CreateNewEnemy();
            newEnemy.SetActive(true);
            return newEnemy;
        }

        return null;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            enemy.SetActive(false);
            enemy.transform.SetParent(poolParent);
        }
    }

    public int GetActiveCount()
    {
        int count = 0;
        foreach (GameObject enemy in pool)
        {
            if (enemy != null && enemy.activeInHierarchy)
                count++;
        }
        return count;
    }
}
