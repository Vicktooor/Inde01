using UnityEngine;
using UnityEngine.UI;

abstract public class ScrollerData { }

public class ScrollerComponent : MonoBehaviour
{
    private bool _raycastTarget = true;
    private ScrollerData _data;

    protected RectTransform[] _rects = new RectTransform[0];
    protected Vector3[] _baseSises = new Vector3[0];
    protected MaskableGraphic[] _graphics = new MaskableGraphic[0];

    protected RectTransform _rectTransform;
    public RectTransform RectTransform { get { return _rectTransform; } }

    public T Data<T>() where T : ScrollerData { return _data as T; }

    virtual public void Init<T>(T data) where T : ScrollerData
    {
        _data = data;
        _rects = GetComponentsInChildren<RectTransform>();
        _graphics = GetComponentsInChildren<MaskableGraphic>();
        _baseSises = new Vector3[_rects.Length];
        int i = 0;
        foreach (RectTransform rt in _rects)
        {
            _baseSises[i] = rt.localScale;
            i++;
        }
        _rectTransform = _rects.Extract(0);
        RaycastTarget(_raycastTarget);
    }

    public void SetSize(float size)
    {
        int count = _rects.Length;
        for (int i = 0; i < count; i++) _rects[i].localScale = _baseSises[i] * size;
    }

    public void SetAlpha(float alpha)
    {
        Color col;
        foreach (MaskableGraphic mg in _graphics)
        {
            col = mg.color;
            col.a = alpha;
            mg.color = col;
        }
    }

    public void RaycastTarget(bool state)
    {
        _raycastTarget = state;
        foreach (MaskableGraphic mg in _graphics) mg.raycastTarget = _raycastTarget;
    }
}