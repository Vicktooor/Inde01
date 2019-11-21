using System;
using System.Collections.Generic;
using UnityEngine;

public class MainLoader<T> : Singleton<MainLoader<T>> where T : UnityEngine.Object
{
    private Dictionary<string, T> _resources = new Dictionary<string, T>();

    public T Load(string folderPath, string fileName)
    {
        if (_resources.ContainsKey(fileName)) return _resources[fileName];
        else
        {
            T lResource = Resources.Load<T>(folderPath + fileName);
            if (lResource != null) _resources.Add(fileName, lResource);
            return lResource;
        }
    }

    public void LoadAsync(string folderPath, string fileName)
    {
        if (AsyncMainLoader.Instance) AsyncMainLoader.Instance.LoadAsync<T>(folderPath, fileName);
    }

    public void LoadAsync(string folderPath, string fileName, Action cb)
    {
        if (AsyncMainLoader.Instance) AsyncMainLoader.Instance.LoadAsync<T>(folderPath, fileName, cb);
    }

    public void AddAsyncLoadedResource(T loadedAsset, string fileName)
    {
        if (_resources.ContainsKey(fileName)) return;
        else _resources.Add(fileName, loadedAsset);
    }

    public T GetResource(string resourceName)
    {
        if (!_resources.ContainsKey(resourceName)) return null;
        return _resources[resourceName];
    }
}