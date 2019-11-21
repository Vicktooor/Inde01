using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoSingleton<LevelManager>
{
    public Level level;
    public List<Color> portalColors;

    private Player _player;
    private List<RoomObject> _rooms = new List<RoomObject>();

    private void Start()
    {
        MainLoader<Sprite>.Instance.Load("Sprites/", "test_block");
        MainLoader<Sprite>.Instance.Load("Sprites/", "test_portal");
        MainLoader<Sprite>.Instance.Load("Sprites/", "test_player");

        level = new Level();
        level.Generate();
        RoomObject roomObject;
        foreach (Room r in level.roomsList)
        {
            roomObject = new GameObject("Room").AddComponent<RoomObject>();
            roomObject.transform.SetParent(transform);
            roomObject.Init(r);
            _rooms.Add(roomObject);
        }

        foreach (PortalCouple pc in level.portalCouples)
        {
            _rooms.Find(r => r.Data.ID == pc.enRoomID).PlaceEntrancePortal(pc.ID);
            _rooms.Find(r => r.Data.ID == pc.exRoomID).PlaceExitPortal(pc.ID);
        }

        SpawnPlayer();
        PlayerCamera.Instance.Init(_player);
    }

    public void SpawnPlayer()
    {
        RoomObject startRoom = _rooms.Find(r => r.Data.type == ERoomType.Start);
        _player = new GameObject("Player").AddComponent<Player>();
        if (startRoom.Data.exDirection == ERoomDirection.Right) _player.transform.position = startRoom.transform.position + Vector3.right;
        else _player.transform.position = startRoom.transform.position + (Vector3.right * 6);
        _player.Init(startRoom.Data.exDirection);
    }

    public void TeleportPlayer(PortalObject portal)
    {
        PortalObject targetPortal = _rooms.Find(r => r.Portals.Find(p => p.ID == portal.ID) && !r.Portals.Contains(portal)).Portals.Find(p => p.ID == portal.ID);
        PlayerCamera.Instance.Teleport(targetPortal.transform.position + (Vector3.right * 0.5f), _player.transform.position);
    }
}