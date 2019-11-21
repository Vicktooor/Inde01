using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEngine.Networking;

public class AudioImporter : MonoSingleton<AudioImporter>
{
    public Action<AudioClip, string> Loaded;

    public void GetAudioClipFromResources(string folder, string clipName, Action<AudioClip> cb)
    {
        string path = folder + "/" + clipName;
        ResourceRequest req = Resources.LoadAsync<AudioClip>(path);
        StartCoroutine(LoadFromResourcesCoroutine(req, cb));
    }

    public void GetAudioClip(string folder, string clipName, Action<AudioClip> cb)
    {
        string wavPath = Application.persistentDataPath + "/" + folder + "/" + clipName + ".wav";
        string mp3Path = Application.persistentDataPath + "/" + folder + "/" + clipName + ".mp3";

        if (File.Exists(wavPath))
        {
            Debug.Log("Load -> " + wavPath);
            UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(wavPath, AudioType.WAV);
            StartCoroutine(LoadCoroutine(req, cb));
        }
        else if (File.Exists(mp3Path))
        {
            Debug.Log("Load -> " + mp3Path);
            UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(wavPath, AudioType.MPEG);
            StartCoroutine(LoadCoroutine(req, cb));
        }
    }

    private IEnumerator LoadCoroutine(UnityWebRequest req, Action<AudioClip> cb)
    {
        req.SendWebRequest();
        while (!req.isDone) { yield return null; }
        cb(DownloadHandlerAudioClip.GetContent(req));
    }

    private IEnumerator LoadFromResourcesCoroutine(ResourceRequest req, Action<AudioClip> cb)
    {
        while (!req.isDone) { yield return null; }
        cb(req.asset as AudioClip);
    }
}
