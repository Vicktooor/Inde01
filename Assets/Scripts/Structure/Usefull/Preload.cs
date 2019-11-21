using UnityEngine;
using UnityEngine.SceneManagement;

public static class Preload
{
    private static bool _preLoaded = false;

    public static void Preloading()
    {
        if (!_preLoaded && !GameObject.Find("__app"))
        {
            SceneManager.LoadScene("_preload", LoadSceneMode.Additive);
            _preLoaded = true;
        }
    }
}