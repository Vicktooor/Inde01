using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class SceneLoader : MonoSingleton<SceneLoader>
{
    public void LoadScene(string sceneName, bool allowSceneActivation, Action<float> progress)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = allowSceneActivation;
        StartCoroutine(AsyncLoadScene(op, progress));
    }

    public void LoadAdditiveScene(string sceneName, bool allowSceneActivation, Action<float> progress)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = allowSceneActivation;
        StartCoroutine(AsyncLoadScene(op, progress));
    }

    private IEnumerator AsyncLoadScene(AsyncOperation op, Action<float> progress)
    {
        while (!op.isDone)
        {
            progress?.Invoke(op.progress);
            yield return null;
        }
    }
}
