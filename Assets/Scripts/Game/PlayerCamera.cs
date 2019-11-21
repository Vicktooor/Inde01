using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoSingleton<PlayerCamera>
{
    public float xFollowDistance;
    public float yFollowDistance;
    public float ySnapTime = 0.125f;
    public ParticleSystem teleportParticle;

    private Player _player;
    private Transform _playerTrf;
    private float _yPosition;
    private bool _init = false;
    private bool _teleport = false;
    private Transform _lensTransform;

    public void Init(Player player)
    {
        _player = player;
        _playerTrf = player.transform;
        _yPosition = _playerTrf.position.y;
        transform.position = new Vector3(_playerTrf.position.x, _yPosition, 0f);
        _init = true;
        _lensTransform = GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        if (!_init || _teleport) return;

        Vector3 previousLensPos = _lensTransform.position;
        transform.position = new Vector3(_playerTrf.position.x, _yPosition, 0f);

        float dist = _lensTransform.position.x - _playerTrf.position.x;
        if (Mathf.Abs(dist) < xFollowDistance || (dist < 0 && _player.Direction == -1) || (dist > 0 && _player.Direction == 1))
            _lensTransform.position = new Vector3(previousLensPos.x, previousLensPos.y, -5f);

        dist = _playerTrf.position.y - transform.position.y;
        if (dist <= -yFollowDistance) transform.position = new Vector3(transform.position.x, _playerTrf.position.y + yFollowDistance, 0f);
    }

    public void SnapY(float yPosition)
    {
        _yPosition = yPosition;
        StartCoroutine(SnapYPosition(yPosition));
    }

    private IEnumerator SnapYPosition(float yPosition)
    {
        float t = 0f;
        float prevY = transform.position.y;
        float yValue = yPosition - prevY;

        while (t < 1f)
        {
            t = Mathf.Clamp01(t + (Time.deltaTime * (1f / ySnapTime)));
            transform.position = new Vector3(transform.position.x, prevY + (yValue * Easing.CrossFade(Easing.SmoothStart, 2, Easing.SmoothStop, 2, t)), 0f);
            yield return null;
        }
    }

    public void Teleport(Vector3 targetPos, Vector3 playerPos)
    {
        _teleport = true;
        _player.gameObject.SetActive(false);
        StartCoroutine(TeleportCoroutine(targetPos, playerPos, teleportParticle.main.duration));
    }

    private IEnumerator TeleportCoroutine(Vector3 targetPos, Vector3 playerPos, float duration)
    {
        float t = 0f;

        teleportParticle.Emit(20);
        Vector3 newLensPos;
        Vector3 startPos = transform.position;
        float eTime;
        while (t < 1f)
        {
            t = Mathf.Clamp01(t + (Time.deltaTime * (1f / duration)));
            eTime = Easing.CrossFade(Easing.SmoothStart, 3, Easing.SmoothStop, 3, t);
            transform.position = Vector3.Lerp(startPos, targetPos, eTime);
            newLensPos = Vector3.Lerp(_lensTransform.position, transform.position, t);
            newLensPos.z = -5f;
            _lensTransform.position = newLensPos;

            _playerTrf.position = Vector3.Lerp(playerPos, targetPos, eTime);
            teleportParticle.transform.position = _playerTrf.position;

            yield return null;
        }

        _yPosition = _playerTrf.position.y;
        _player.gameObject.SetActive(true);
        _teleport = false;
    }
}
