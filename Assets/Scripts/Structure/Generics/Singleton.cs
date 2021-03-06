﻿using UnityEngine;

public class Singleton<T> where T : new()
{
    protected static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;
            else
            {
                _instance = new T();
                return _instance;
            }
        }
    }
}

public class MonoSingleton<T> : MonoBehaviour
{
    protected static T _instance;
    public static T Instance { get { return _instance; } }

    public bool dontDestroyOnLoad = false;
    public bool hideAtLaunch = false;

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("MonoSingleton [" + _instance.ToString() + "] allready exist");
            Destroy(this);
        }
        _instance = GetComponent<T>();
        if (dontDestroyOnLoad) DontDestroyByLoad.EnableDontDestroyOnLoad(gameObject);
        gameObject.SetActive(!hideAtLaunch);
    }

    protected virtual void OnDestroy()
    {
        _instance = default;
        DontDestroyByLoad.OnDestroyObj(gameObject);
    }
}
