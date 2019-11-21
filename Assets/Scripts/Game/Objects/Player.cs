using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    public float maxXSpeed = 0.5f;
    public float maxYSpeed = 20f;
    public float jumpHeight = 1.5f;

    private bool _init = false;
    private int _direction;
    public int Direction { get { return _direction; } }
    private SpriteRenderer _renderer;
    private bool _leavingPortal = false;
    private float _normalizedFallSpeed = 0f;
    private bool _jumping = false;
    private float _jumpTime = 0f;
    private float _jumpStartY = 0f;

    private RaycastHit2D portalHit;
    private RaycastHit2D wallHit;
    private RaycastHit2D[] groundHits = new RaycastHit2D[2];
    private bool _onGround = true;

    public void Init(ERoomDirection iDirection)
    {
        _direction = iDirection == ERoomDirection.Right ? 1 : -1;
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sortingOrder = 1000;
        _renderer.sprite = MainLoader<Sprite>.Instance.GetResource("test_player");
        _init = true;
    }

    private void Update()
    {
        if (!_init) return;

        Vector3 prevPos = transform.position;
        transform.position += Vector3.right * (Time.deltaTime * (1f / maxXSpeed) * _direction);
        if (!_jumping && !_onGround) transform.position += Vector3.down * (Time.deltaTime * _normalizedFallSpeed * maxYSpeed);

        Vector3 castOrigin = transform.position + (Vector3.up * 0.25f);
        Vector3 directionVector = Vector3.right * _direction;
        if (!_leavingPortal)
        {
            portalHit = Physics2D.CircleCast(castOrigin, 0.2f, directionVector, 0f, LayerMask.GetMask(new string[] { "Portal" }));
            if (portalHit.collider != null && Input.GetMouseButtonDown(0))
            {
                _leavingPortal = true;
                LevelManager.Instance.TeleportPlayer(portalHit.collider.GetComponent<PortalObject>());
                _jumpTime = 0f;
                _jumping = false;
                return;
            }
        }
        else
        {
            portalHit = Physics2D.CircleCast(castOrigin, 0.5f, directionVector, 0f, LayerMask.GetMask(new string[] { "Portal" }));
            if (portalHit.collider == null) _leavingPortal = false;
        }

        bool wasOnGround = _onGround;
        groundHits[0] = Physics2D.Raycast(castOrigin + new Vector3(-0.125f, 0f, 0f), -Vector2.up, 0.25f, LayerMask.GetMask(new string[] { "Wall" }));
        groundHits[1] = Physics2D.Raycast(castOrigin + new Vector3(0.125f, 0f, 0f), -Vector2.up, 0.25f, LayerMask.GetMask(new string[] { "Wall" }));
        _onGround = groundHits[0] || groundHits[1];
        if (!_onGround) _normalizedFallSpeed = Mathf.Clamp(_normalizedFallSpeed + Time.deltaTime, 0.01f, 1f);
        else
        {
            _normalizedFallSpeed = 0f;
            if (!wasOnGround)
            {
                _jumping = false;
                _jumpTime = 0f;
                if (groundHits[0].point != Vector2.zero) transform.position = new Vector3(transform.position.x, Physics2D.Raycast(castOrigin + new Vector3(-0.125f, 0.5f, 0f), -Vector2.up, 1f, LayerMask.GetMask(new string[] { "Wall" })).point.y);
                else if (groundHits[1].point != Vector2.zero) transform.position = new Vector3(transform.position.x, Physics2D.Raycast(castOrigin + new Vector3(0.125f, 0.5f, 0f), -Vector2.up, 1f, LayerMask.GetMask(new string[] { "Wall" })).point.y);
                castOrigin = transform.position + (Vector3.up * 0.25f);
                PlayerCamera.Instance.SnapY(transform.position.y);
            }
        }

        wallHit = Physics2D.Raycast(castOrigin, directionVector, 0.25f, LayerMask.GetMask(new string[] { "Wall" }));
        if (wallHit.collider != null)
        {
            _direction *= -1;
            transform.position = new Vector2(wallHit.point.x + (_direction * 0.25f), transform.position.y);
        }

        if (_jumping)
        {            
            _jumpTime = Mathf.Clamp01(_jumpTime + (Time.deltaTime * (1f / maxXSpeed)));
            transform.position = new Vector3(transform.position.x, _jumpStartY + Easing.Arch(_jumpTime) * jumpHeight, 0f);
            if (_jumpTime == 1f)
            {
                _jumpTime = 0f;
                _jumping = false;
                _normalizedFallSpeed = Mathf.Clamp01((Vector3.Distance(prevPos, transform.position) / Time.deltaTime) / maxXSpeed);
            }
        }

        if (!_jumping && _onGround && Input.GetMouseButtonDown(0))
        {
            _jumpStartY = transform.position.y;
            _jumping = true;
        }
    }
}
