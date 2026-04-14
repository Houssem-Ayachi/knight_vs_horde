using System;
using UnityEngine;

// this component should be added to any gameObject that will be part of a pool (like enemies and xpOrbs).
public class Poolable : MonoBehaviour
{
    // a callback method that is called when the component becomes active
    public Action onActivated = null;

    // a callback method that is called when the component becomes inactive
    public Action onDeactivated = null;

    private Pool pool;

    void Start()
    {
        
    }

    public Pool Pool
    {
        set { pool = value; }
    }

    void OnEnable()
    {
        onActivated?.Invoke();
    }

    void OnDisable()
    {
        onDeactivated?.Invoke();
    }

    public void ReturnToPool()
    {
        pool.ReturnPoolItem(gameObject);
    }
}
