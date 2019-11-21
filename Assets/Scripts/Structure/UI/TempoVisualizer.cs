using UnityEngine;
using UnityEngine.UI;

public class TempoVisualizer : MonoBehaviour
{
    public RawImage beatMarker;
    public RawImage noteMarker;

    private void Awake()
    {
        Events.Instance.AddListener<OnBeatTimeEvent>(UpdateBeat);
        Events.Instance.AddListener<OnNoteTimeEvent>(UpdateNote);
        Events.Instance.AddListener<OnResetTimeMusic>(OnStop);
    }

    private void UpdateBeat(OnBeatTimeEvent e)
    {
        float s = Easing.SmoothStop(e.time, 3) / 2f;
        beatMarker.transform.localScale = new Vector3(1f + s, 1f + s, 1f);
        Color col = beatMarker.color;
        col.a = Easing.SmoothArchStop(e.time, 2);
        beatMarker.color = col;
    }

    private void UpdateNote(OnNoteTimeEvent e)
    {
        float s = Easing.Arch(e.time) / 2f;
        noteMarker.transform.localScale = new Vector3(1f + s, 1f + s, 1f);
        Color col = noteMarker.color;
        col.a = Easing.SmoothStop(e.time, 2);
        noteMarker.color = col;
    }

    private void OnStop(OnResetTimeMusic e)
    {
        UpdateBeat(new OnBeatTimeEvent(0f, AudioSettings.dspTime));
        UpdateNote(new OnNoteTimeEvent(0f, AudioSettings.dspTime));
    }

    private void OnDestroy()
    {
        Events.Instance.RemoveListener<OnBeatTimeEvent>(UpdateBeat);
        Events.Instance.RemoveListener<OnNoteTimeEvent>(UpdateNote);
        Events.Instance.RemoveListener<OnResetTimeMusic>(OnStop);
    }
}
