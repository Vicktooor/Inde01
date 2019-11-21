using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DontDestroyByLoad : MonoBehaviour
{
    private static List<GameObject> _DONT_DESTROY_GO = new List<GameObject>();

    public static void EnableDontDestroyOnLoad(GameObject obj)
    {
        DontDestroyOnLoad(obj);
        if (!_DONT_DESTROY_GO.Contains(obj)) _DONT_DESTROY_GO.Add(obj);
    }

    public static void DisableDontDestroyOnLoad(GameObject obj)
    {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
        if (_DONT_DESTROY_GO.Contains(obj)) _DONT_DESTROY_GO.Remove(obj);
    }

    public static void OnDestroyObj(GameObject obj)
    {
        if (_DONT_DESTROY_GO.Contains(obj)) _DONT_DESTROY_GO.Remove(obj);
    }

    private void Awake()
    {
        EnableDontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_DONT_DESTROY_GO.Contains(gameObject)) _DONT_DESTROY_GO.Remove(gameObject);
    }
}
