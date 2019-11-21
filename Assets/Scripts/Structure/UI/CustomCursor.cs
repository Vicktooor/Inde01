using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CustomCursor : MonoSingleton<CustomCursor>
{
    public bool outsideControl = false;
    public bool active = true;
    public List<Color> colors;

    private Transform _cursorTrf;
    private RawImage _mainImage;
    private RawImage _colorImage;
    private int _colorIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
        _cursorTrf = transform.GetChild(0);
        _colorImage = _cursorTrf.GetComponent<RawImage>();
        _mainImage = _cursorTrf.GetChild(0).GetComponent<RawImage>();
        _colorImage.color = colors[_colorIndex];
    }

    public void Update()
    {
        if (!outsideControl) _cursorTrf.position = Input.mousePosition;
    }

    public void NextColor()
    {
        _colorIndex = (_colorIndex + 1) % colors.Count;
        _colorImage.color = colors[_colorIndex];
    }

    public void SetColor(Color col)
    {
        _colorImage.color = col;
    }

    public void SetVisible(bool visible)
    {
        if (visible && visible != _cursorTrf.gameObject.activeSelf) _cursorTrf.position = Input.mousePosition;
        _cursorTrf.gameObject.SetActive(visible);
    }
}
