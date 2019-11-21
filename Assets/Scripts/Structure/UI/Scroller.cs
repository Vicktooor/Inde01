using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EScrollDirection { Vertical, Horizontal }
public enum EScrollMove { OneByOne, All }

[System.Serializable]
public struct ScrollerLine
{
    public List<ScrollerComponent> components;
}

public static class ScrollerManager
{
    private static Dictionary<int, List<ScrollerData>> _instancesDatas = new Dictionary<int, List<ScrollerData>>();
    public static void AddComponent(int scrollerID, ScrollerData data)
    {
        if (!_instancesDatas.ContainsKey(scrollerID)) _instancesDatas.Add(scrollerID, new List<ScrollerData>() { data } );
        else _instancesDatas[scrollerID].Add(data);
    }
    public static void RemComponent(int scrollerID, ScrollerData data)
    {
        if (_instancesDatas.ContainsKey(scrollerID)) _instancesDatas[scrollerID].Remove(data);
    }
    public static List<ScrollerData> GetDatas(int scrollerID) { return _instancesDatas.ContainsKey(scrollerID) ? _instancesDatas[scrollerID] : null; }
}

public class Scroller<T> : MonoBehaviour where T : ScrollerComponent
{
    [SerializeField]
    public Vector2 _margin;
    public IntVector2 countRestriction;
    public EScrollMove moveType;
    public EScrollDirection eDirection;
    public bool IsVertical { get { return eDirection == EScrollDirection.Vertical; } }
    public Vector2 ComponentSize { get { return _componentSize + _margin; } }
    public bool sizeDisplay = false;
    public bool alphaDisplay = false;
    public int moveSpeed;
    public Transform container;

    private int _instanceID;
    private bool _initialized = false;
    private Vector2 _size;
    private Vector2 _fullSize;
    private Vector2 _componentSize;
    private float _RefSize { get { return IsVertical ? ComponentSize.y : ComponentSize.x; } }
    private IntVector2 _maxCount = IntVector2.One;
    public int _MaxCount { get { return IsVertical ? _maxCount.y : _maxCount.x; } }
    private int _nbLineData = 0;
    private float _relativePosition;
    private float _prevRelativePosition;
    private bool _moving = false;
    private int _targetLine = 0;

    private float _fullRelativePosition;
    private float _endPosition;

    private RectTransform _rectTransform;
    public RectTransform RectTransform { get { return _rectTransform; } }

    protected List<T> _listComponents = new List<T>();
    public T[] Components { get { return _listComponents.ToArray(); } }

    private List<ScrollerLine> _lines = new List<ScrollerLine>();

    public bool IsFull { get { return (_listComponents.Count / ((_maxCount.x + (IsVertical ? 0 : 1)) * (_maxCount.y + (IsVertical ? 1 : 0)))) >= 1f; } }

    public void Init()
    {
        _instanceID = GetInstanceID();
        _rectTransform = GetComponent<RectTransform>();
        _listComponents = GetComponentsInChildren<T>(true).ToList();
        _size = _rectTransform.rect.size;

        if (_listComponents.Count <= 0) return;
        _initialized = true;
        _componentSize = _listComponents[0].GetComponent<RectTransform>().rect.size;
        _maxCount.x = Mathf.FloorToInt(_size.x / ComponentSize.x);
        _maxCount.x = _maxCount.x <= countRestriction.x ? _maxCount.x : countRestriction.x;
        _maxCount.y = Mathf.FloorToInt(_size.y / ComponentSize.y);
        _maxCount.y = _maxCount.y <= countRestriction.y ? _maxCount.y : countRestriction.y;
        _fullSize = new Vector2(ComponentSize.x * _maxCount.x, ComponentSize.y * _maxCount.y);
        Refresh();
    }

    public void AddComponent(T comp, ScrollerData data)
    {
        ScrollerManager.AddComponent(_instanceID, data);
        _nbLineData = 1 + (Mathf.CeilToInt(ScrollerManager.GetDatas(_instanceID).Count - 1) / (IsVertical ? _maxCount.x : _maxCount.y));
        _endPosition = (_nbLineData * _RefSize) - (_MaxCount * _RefSize);

        if (!IsFull)
        {
            if (container) comp = Instantiate(comp, container);
            else comp = Instantiate(comp, RectTransform);
            comp.Init(data);

            _listComponents.Add(comp);
            if (!_initialized) Init();
            if (_lines.Count < _nbLineData)
            {
                List<ScrollerComponent> nList = new List<ScrollerComponent>() { comp };
                _lines.Add(new ScrollerLine() { components = nList });
            }
            else _lines[_nbLineData - 1].components.Add(comp);
        }
        Refresh();
    }

    public void Refresh()
    {
        if (_rectTransform.rect.size != _size) Init();

        int l = _lines.Count;
        Vector2 nPos;
        ScrollerLine line;
        for (int i = 0; i < l; i++)
        {
            line = _lines[i];
            int counter = 0;
            foreach (ScrollerComponent sc in line.components)
            {
                nPos = Vector2.zero;
                if (IsVertical)
                {
                    nPos += (Vector2.right * ComponentSize.x * counter) + (Vector2.right * (ComponentSize.x / 2f)) + (Vector2.left * (_size.x / 2f));
                    nPos += (Vector2.down * ComponentSize.y * i) + (Vector2.down * (ComponentSize.y / 2f)) + (Vector2.up * (_size.y / 2f));
                    nPos += Vector2.up * _relativePosition;
                    counter = (counter + 1) % _maxCount.x;
                }
                else
                {
                    nPos += (Vector2.right * ComponentSize.x * i) + (Vector2.right * (ComponentSize.x / 2f)) + (Vector2.left * (_size.x / 2f));
                    nPos += (Vector2.down * ComponentSize.y * counter) + (Vector2.down * (ComponentSize.y / 2f)) + (Vector2.up * (_size.y / 2f));
                    nPos += Vector2.left * _relativePosition;
                    counter = (counter + 1) % _maxCount.y;
                }
                nPos += new Vector2(1, -1) * ((_size - (_maxCount * ComponentSize)) / 2f);
                sc.transform.localPosition = nPos;
            }
        }      
        UpdateDisplay();
    }

    private void UpdateComponents(ScrollerLine line, float direction)
    {
        int nbPerLine = IsVertical ? _maxCount.x : _maxCount.y;
        int startIndex;
        if (direction > 0) startIndex = (_targetLine * nbPerLine) + (_MaxCount * nbPerLine);
        else startIndex = _targetLine * nbPerLine;
        int endIndex = startIndex + nbPerLine;
        int count = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            if (count < line.components.Count)
            {
                if (i < ScrollerManager.GetDatas(_instanceID).Count)
                {
                    line.components[count].Init(ScrollerManager.GetDatas(_instanceID)[i]);
                    line.components[count].gameObject.SetActive(true);
                }
                else line.components[count].gameObject.SetActive(false);
            }
            count++;
        }
    }

    public void UpdateDisplay()
    {
        if (!alphaDisplay && !sizeDisplay) return;

        int l = _lines.Count;
        List<ScrollerComponent> comps;
        float n;
        for (int i = 0; i < l; i++)
        {
            float rValue = _relativePosition;
            if (i > 0 && i < l - 1) n = 1f;
            else
            {
                if (i == 0)
                {
                    rValue -= _RefSize / 2f;
                    if (rValue < 0f) rValue = 0f;
                }
                n = Mathf.Clamp01(rValue / (_RefSize / 2f));
                if (i == 0) n = 1f - n;
                if (l <= _MaxCount) n = 1f;
            }

            comps = _lines[i].components;
            int lc = comps.Count;
            for (int j = 0; j < lc; j++)
            {
                if (alphaDisplay) comps[j].SetAlpha(Easing.SmoothStart(n, 2));
                if (sizeDisplay) comps[j].SetSize(Easing.SmoothStart(n, 2));
            }
        }
    }

    public bool Move(float direction)
    {
        if (_endPosition < 0f) return false;
        if (_fullRelativePosition == _endPosition && direction > 0) return false;
        if (_fullRelativePosition < 0f && direction < 0) return false; 

        _fullRelativePosition += direction;
        _prevRelativePosition = _relativePosition;
        _relativePosition += direction;

        if (_relativePosition > _RefSize) _relativePosition = _relativePosition - _RefSize;
        if (_relativePosition < 0f) _relativePosition = _RefSize + _relativePosition;

        bool swap = (_prevRelativePosition > _relativePosition && direction > 0) || (_prevRelativePosition < _relativePosition && direction < 0);
        SwitchLine(swap, direction);

        if (_fullRelativePosition > _endPosition && direction > 0) _fullRelativePosition = _endPosition;
        if (_fullRelativePosition < 0f && direction < 0)  _fullRelativePosition = 0f;
        _relativePosition = swap && direction > 0 ? 0f : swap && direction < 0 ? _RefSize : _relativePosition;

        Refresh();
        return true;
    }

    private void SwitchLine(bool swap, float direction)
    {
        if (swap && direction > 0)
        {
            _targetLine++;
            ScrollerLine line = _lines[0];
            _lines.RemoveAt(0);
            _lines.Add(line);
            UpdateComponents(line, direction);
        }
        else if (swap && direction < 0)
        {
            _targetLine--;
            ScrollerLine line = _lines[_lines.Count - 1];
            _lines.RemoveAt(_lines.Count - 1);
            _lines.Insert(0, line);
            UpdateComponents(line, direction);
        }
    }

    public void Next()
    {
        if (_moving || _fullRelativePosition >= _endPosition) return;
        _moving = true;
        StartCoroutine(MoveCoroutine(1, moveType == EScrollMove.OneByOne ? 1 : _MaxCount));
    }

    public void Previous()
    {
        if (_moving || _fullRelativePosition <= 0f) return;
        _moving = true;
        StartCoroutine(MoveCoroutine(-1, moveType == EScrollMove.OneByOne ? 1 : _MaxCount));
    }

    private IEnumerator MoveCoroutine(int direction, int lineAmount)
    {
        float amount = _RefSize / moveSpeed;
        float moveValue = 0f;
        bool stop = false;
        while (!stop)
        {
            if (moveValue == _RefSize)
            {
                if (direction > 0) _relativePosition = _RefSize;
                if (direction < 0) _relativePosition = 0f;
                lineAmount--;
                if (lineAmount <= 0 || (_fullRelativePosition <= 0f && direction < 0) || (_fullRelativePosition >= _endPosition && direction > 0))
                    stop = true;
                else moveValue = 0f;
            }
            else
            {
                moveValue += amount;
                if (moveValue <= _RefSize) Move(amount * direction);
            }
            yield return null;
        }
        Refresh();
        _moving = false;
    }
}

