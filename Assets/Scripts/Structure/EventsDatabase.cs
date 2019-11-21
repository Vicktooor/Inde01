public class BeatGameEvent : GameEvent
{
    public double dspTime;
    public BeatGameEvent(double dsp)
    {
        dspTime = dsp;
    }
}

public class OnBeat : BeatGameEvent {
    public int currentBeat;
    public OnBeat(int b, double dsp) : base(dsp)
    {
        dspTime = dsp;
        currentBeat = b;
    }
}

public class OnInterBeat : BeatGameEvent {
    public int interBeat;
    public OnInterBeat(int t, double dsp) : base(dsp)
    {
        dspTime = dsp;
        interBeat = t;
    }
}

public class OnBeatTimeEvent : BeatGameEvent
{
    public float time;
    public OnBeatTimeEvent(float t, double dsp) : base(dsp)
    {
        dspTime = dsp;
        time = t;
    }
}

public class OnNoteTimeEvent : BeatGameEvent
{
    public float time;
    public OnNoteTimeEvent(float t, double dsp) : base(dsp)
    {
        dspTime = dsp;
        time = t;
    }
}

public class OnMusicFrameTime : BeatGameEvent
{
    public float frameTime;
    public OnMusicFrameTime(float t, double dsp) : base(dsp)
    {
        dspTime = dsp;
        frameTime = t;
    }
}

public class OnResetTimeMusic : GameEvent { }