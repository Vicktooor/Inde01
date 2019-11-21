using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIOverlay : MonoBehaviour
{
    public Vector2 pivot;
    public Transform targetTransform;

    private RectTransform _rectTransform;

    virtual protected void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    virtual public void Set(Vector2 newPivot, Transform newTransform)
    {
        pivot = newPivot;
        targetTransform = newTransform;
    }

    virtual protected void Update()
    {
        if (targetTransform != null)
        {
            Vector3 newPos = Camera.main.WorldToScreenPoint(targetTransform.position);
            newPos += (Vector3)pivot;
            _rectTransform.position = newPos;
        }
    }
}
