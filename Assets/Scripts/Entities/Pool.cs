using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public int poolSize;
    public bool expandable;
    public GameObject targetItem;

    private List<GameObject> pool = new List<GameObject>();
    private GameObject container = null;

    void Start()
    {

    }

    public void ResetPool()
    {
        // empty pool just in case
        pool.Clear();

        GameObject item;

        for(int i=0; i<poolSize;i++)
        {
            item = CreatePoolItem();

            pool.Add(item);
        }
    }

    public void SetPoolItemContainer(GameObject target)
    {
        container = target;
    }

    // creates a new poolItem, adds it to the pool and returns it.
    private GameObject CreatePoolItem()
    {
        // intantiate a new pool item and set its parent to the current game object.
        GameObject item;
        if(container != null)
        {
            item = Instantiate(targetItem, container.transform);

            item.GetComponent<Poolable>().Pool = this;
        }
        else
        {
            item = Instantiate(targetItem);
        }

        item.SetActive(false);

        return item;
    }

    public GameObject GetItem()
    {
        foreach(GameObject item in pool)
        {
            if(!item.activeInHierarchy)
            {
                item.SetActive(true);
                return item;
            }
        }

        // if no item was found in the pool and the pool is expandable then create a new one and return it.
        // this pool item won't be added to the pool and should be freed dynamically from memory (not sure if this is a good idea)
        if (expandable)
        {
            GameObject newEnemy = CreatePoolItem();
            newEnemy.SetActive(true);
            return newEnemy;
        }

        // sorry boss pool is empty
        return null;
    }

    public void ReturnPoolItem(GameObject item)
    {
        // TODO: need to figure out a way to make sure this item really exists in the pool
        // checking to see if an exact replica exists in the pool with a foreach loop might work but i'm afraid it'll be costly
        item.SetActive(false);
    }

    public int ActivePoolItems()
    {
        // TODO: i should maybe make an attribute called "activePoolItems" set it to 0 by default and then each time a pool item is fetched add one to it.
        int total = 0;

        foreach(GameObject item in pool)
        {
            if(item.activeInHierarchy)
            {
                total++;
            }
        }

        return total;
    }
}
