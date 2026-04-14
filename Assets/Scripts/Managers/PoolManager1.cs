using System.Collections.Generic;
using UnityEngine;

public class PoolManager1 : MonoBehaviour
{
    public static PoolManager1 Instance { get; private set; }

    [SerializeField] private List<PoolEntry> poolEntries;

    private Dictionary<EPoolItemType, Pool> poolsDict = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); // destroy duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // populate the poolsDict with the items in the poolEntries list;
        foreach(PoolEntry entry in poolEntries)
        {
            entry.pool.SetPoolItemContainer(gameObject);

            entry.pool.ResetPool();

            poolsDict.Add(entry.poolItemType, entry.pool);
        }
    }

    public GameObject GetPoolItem(EPoolItemType itemType)
    {
        if(!poolsDict.ContainsKey(itemType))
            return null;

        return poolsDict[itemType].GetItem();
    }

    public void ReturnPoolItem(EPoolItemType itemType, GameObject item)
    {
        if(!poolsDict.ContainsKey(itemType))
            return;

        poolsDict[itemType].ReturnPoolItem(item);
    }

    public int GetActivePoolItems(EPoolItemType itemType)
    {
        if(!poolsDict.ContainsKey(itemType))
            return -1;

        return poolsDict[itemType].ActivePoolItems();
    }
}
