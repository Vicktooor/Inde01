using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeatAudioSource : MonoSingleton<BeatAudioSource>
{
    private AudioSource[] _audioSources = new AudioSource[2];
    private AudioClip _audioClip;

    private int _toggleIndex = 0;
    private bool _looping = false;
    private float _bpm = -1f;
    private bool _firstBeatSend = false;
    private bool _playing = false;

    private double _duration;
    public double Duration { get { return _duration; } }
    private double _beatDuration;
    public double BeatDuration { get { return _beatDuration; } }
    private double _noteDuration;
    public double NoteDuration { get { return _noteDuration; } }

    private double _elapsedTime = 0d;
    public  double ElapsedTime { get { return _elapsedTime; } }
    private double _elapsedBeatTime = 0d;
    public double ElapsedBeatTime { get { return _elapsedBeatTime; } }
    private double _elapsedNoteTime = 0d;
    public double ElapsedNoteTime { get { return _elapsedNoteTime; } }

    private double _prevElapsedBeatTime = 0d;
    private double _prevElapsedNoteTime = 0d;

    private int _currentBeat = 0;
    public int CurrentBeat { get { return _currentBeat; } }
    private int _currentBeatNote = 0;
    public int CurrentInternBeat { get { return _currentBeatNote; } }
    private double _previousDspTime = AudioSettings.dspTime;

    protected override void Awake()
    {
        base.Awake();
        AudioSource[] sources = GetComponents<AudioSource>();
        _audioSources[0] = sources[0];
        if (sources.Length <= 1) _audioSources[1] = gameObject.AddComponent<AudioSource>();
        else _audioSources[1] = sources[1];
    }

    public void CreateBeatSource(AudioClip audioClip, float bpm, bool loop)
    {
        _audioClip = audioClip;
        _looping = loop;
        _bpm = bpm;
        _duration = (double)_audioClip.samples / _audioClip.frequency;
        _beatDuration = (60d / bpm);
        _noteDuration = _beatDuration / 4f;

        for (int i = 0; i < 2; i++)
        {
            _audioSources[i].clip = audioClip;
            _audioSources[i].spatialize = false;
        }
    }

    public void Play(double delay = 0.5d)
    {
        _previousDspTime = AudioSettings.dspTime;

        double time = AudioSettings.dspTime + (delay < 0.5d ? 0.5d : delay);
        int waitToggle = 1 - _toggleIndex;
        _audioSources[_toggleIndex].PlayScheduled(time);
        _audioSources[waitToggle].PlayScheduled(time + _duration);

        ResetBeatData();
        _playing = true;
    }

    public void JumpAndPlay(float time, double delay = 0.5d)
    {
        if (_audioSources[_toggleIndex].isPlaying)
        {
            Debug.LogError("Can't use JumpAndPlay() playing audioclip - Use Jump instead");
            return;
        }

        _audioSources[_toggleIndex].time = time * (float)_duration;

        UpdateBeatData();
        UpdateElaspedTimes();
        Play(delay);
    }

    public void Jump(float time)
    {
        _previousDspTime = AudioSettings.dspTime;

        _audioSources[_toggleIndex].time = time * (float)_duration;
        int waitToggle = 1 - _toggleIndex;
        _audioSources[waitToggle].SetScheduledStartTime(AudioSettings.dspTime + (_duration - _audioSources[_toggleIndex].time));

        UpdateBeatData();
        UpdateElaspedTimes();
    }

    public void Stop()
    {
        _playing = false;
        _firstBeatSend = false;
        for (int i = 0; i < 2; i++) _audioSources[i].Stop();
        ResetBeatData();
    }

    private void ResetBeatData()
    {
        _elapsedTime = 0d;
        _elapsedBeatTime = 0d;
        _elapsedNoteTime = 0d;
        _prevElapsedBeatTime = 0d;
        _prevElapsedNoteTime = 0d;
        _currentBeat = 0;
        _currentBeatNote = 0;
        Events.Instance.Raise(new OnResetTimeMusic());
    }

    private void Update()
    {
        UpdateElaspedTimes();
    }

    private void UpdateElaspedTimes()
    {
        if (_audioClip == null || !_playing) return;

        _prevElapsedBeatTime = _elapsedBeatTime;
        _prevElapsedNoteTime = _elapsedNoteTime;

        double eTime = ((double)_audioSources[_toggleIndex].timeSamples / _audioClip.frequency);
        _elapsedTime = eTime / _duration;
        _elapsedBeatTime = (eTime % _beatDuration) / _beatDuration;
        _elapsedNoteTime = (eTime % _noteDuration) / _noteDuration;

        double dspTime = AudioSettings.dspTime;

        if (!_firstBeatSend)
        {
            _firstBeatSend = true;
            Events.Instance.Raise(new OnBeat(0, dspTime));
            Events.Instance.Raise(new OnInterBeat(0, dspTime));
        }

        if (_elapsedBeatTime < _prevElapsedBeatTime)
        {
            _currentBeat++;
            Events.Instance.Raise(new OnBeat(_currentBeat, dspTime));
        }

        if (_elapsedNoteTime < _prevElapsedNoteTime)
        {
            _currentBeatNote = (_currentBeatNote + 1) % 4;
            Events.Instance.Raise(new OnInterBeat(_currentBeatNote, dspTime));
        }

        Events.Instance.Raise(new OnBeatTimeEvent((float)_elapsedBeatTime, dspTime));
        Events.Instance.Raise(new OnNoteTimeEvent((float)_elapsedNoteTime, dspTime));
        Events.Instance.Raise(new OnMusicFrameTime((float)AudioSettings.dspTime - (float)_previousDspTime, dspTime));

        _previousDspTime = AudioSettings.dspTime;

        if (_elapsedTime >= 1d)
        {
            if (_looping)
            {
                _audioSources[_toggleIndex].time = 0f;
                _audioSources[_toggleIndex].Stop();
                _audioSources[_toggleIndex].PlayScheduled(AudioSettings.dspTime + _duration);
                _toggleIndex = 1 - _toggleIndex;
                ResetBeatData();
            }
            else Stop();           
        }
    }

    private void UpdateBeatData()
    {
        if (_audioClip == null) return;

        double eTime = ((double)_audioSources[_toggleIndex].timeSamples / _audioClip.frequency);
        _currentBeat = (int)(eTime / _beatDuration);
        _currentBeatNote = (int)(eTime / _noteDuration) % 4;
    }
}
