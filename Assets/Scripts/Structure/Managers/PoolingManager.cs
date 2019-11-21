using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pooling
{
    [SerializeField]
    private Transform _poolContainer;
    public Transform PoolContainer { get { return _poolContainer; } }

    public PoolingObject model;
    public int nbMax;
}

public class PoolingManager : MonoSingleton<PoolingManager>
{
    public List<Pooling> poolingTypes;
    private Dictionary<string, List<PoolingObject>> _poolingObjects = new Dictionary<string, List<PoolingObject>>();

    override protected void Awake()
    {
        base.Awake();
        foreach (Pooling p in poolingTypes) CreatePooling(p.model, p.nbMax, p.PoolContainer);
    }

    public void CreatePooling<T>(T model, int nb, Transform poolContainer) where T : PoolingObject
    {
        string type = model.GetType().ToString();
        if (!_poolingObjects.ContainsKey(type)) _poolingObjects.Add(type, new List<PoolingObject>());
        CreatePoolObjects(model, nb, poolContainer);
    }

    public T GetObject<T>() where T : PoolingObject
    {
        string type = typeof(T).ToString();
        if (_poolingObjects.ContainsKey(type))
        {
            List<PoolingObject> disabledObjects = _poolingObjects[type].FindAll(o => o.Usable);
            if (disabledObjects.Count > 0)
            {
                disabledObjects[0].Usable = false;
                return disabledObjects[0] as T;
            }
            else
            {
                Pooling pool = poolingTypes.Find(t => t.model.GetType().ToString() == type);
                T newObj = CreatePoolObject(pool.model as T, pool.PoolContainer);
                return newObj;
            }
        }
        else
        {
            Debug.LogError("Type not in pooling objects");
            return null;
        }
    }

    public void Reset<T>(T obj) where T : PoolingObject
    {
        string type = obj.GetType().ToString();
        Pooling pool = poolingTypes.Find(t => t.model.GetType().ToString() == type);
        obj.transform.SetParent(pool.PoolContainer);
        obj.gameObject.SetActive(false);
        obj.Usable = true;
    }

    private T CreatePoolObject<T>(T model, Transform poolContainer) where T : PoolingObject
    {
        string type = model.GetType().ToString();
        T newObj = Instantiate(model, (poolContainer != null) ? poolContainer : transform);
        newObj.gameObject.SetActive(false);

        if (_poolingObjects.ContainsKey(type)) _poolingObjects[type].Add(newObj);
        else
        {
            _poolingObjects.Add(type, new List<PoolingObject>());
            _poolingObjects[type].Add(newObj);
        }

        return newObj;
    }

    private void CreatePoolObjects<T>(T model, int nb, Transform poolContainer) where T : PoolingObject
    {
        for (int i = 0; i < nb; i++) CreatePoolObject(model, poolContainer);
    }
}
