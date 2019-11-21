using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private PolygonCollider2D _collider;
    private Vector2 _worldPosition;

    public void Init(Vector2 worldPos)
    {
        gameObject.layer = LayerMask.NameToLayer("Wall");
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = MainLoader<Sprite>.Instance.GetResource("test_block");
        transform.position = worldPos;
        _collider = gameObject.AddComponent<PolygonCollider2D>();
    }
}
