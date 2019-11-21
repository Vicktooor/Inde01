using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class PortalObject : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private PolygonCollider2D _collider;
    private int _ID;
    public int ID { get { return _ID; } }

    public int ownID;
    public int linkID;

    public void Init(Vector2 worldPos, int pID)
    {
        gameObject.layer = LayerMask.NameToLayer("Portal");
        _ID = pID;
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = MainLoader<Sprite>.Instance.GetResource("test_portal");
        _renderer.color = LevelManager.Instance.portalColors[pID];
        transform.position = worldPos;
        _collider = gameObject.AddComponent<PolygonCollider2D>();
    }
}
