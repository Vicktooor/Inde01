using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class RoomObject : MonoBehaviour
{
    private List<PortalObject> _portals = new List<PortalObject>();
    public List<PortalObject> Portals { get { return _portals; } }

    private Room _data;
    public Room Data { get { return _data; } }

    public void Init(Room data)
    {
        _data = data;
        transform.position = new Vector2(_data.gridPos.x * Room.ROOM_SIZE.x, _data.gridPos.y * Room.ROOM_SIZE.y);
        Construct();
    }

    private void Construct()
    {
        Block newBlock;
        for (int i = 0; i < Room.ROOM_SIZE.x; i++)
        {
            for (int j = 0; j < Room.ROOM_SIZE.y; j++)
            {
                if (i > 0 && i < Room.ROOM_SIZE.x - 1 && j > 0 && j < Room.ROOM_SIZE.y - 1) continue;

                if (_data.openSides.Contains(ERoomDirection.Top) && j == Room.ROOM_SIZE.y - 1 && i > 1 && i < Room.ROOM_SIZE.x - 2) continue;
                if (_data.openSides.Contains(ERoomDirection.Bottom) && j == 0 && i > 1 && i < Room.ROOM_SIZE.x - 2) continue;
                if (_data.openSides.Contains(ERoomDirection.Left) && i == 0 && j > 0 && j < Room.ROOM_SIZE.y - 1) continue;
                if (_data.openSides.Contains(ERoomDirection.Right) && i == Room.ROOM_SIZE.x - 1 && j > 0 && j < Room.ROOM_SIZE.y - 1) continue;
                
                newBlock = new GameObject("Block").AddComponent<Block>();
                newBlock.transform.SetParent(transform);
                newBlock.Init(transform.position + new Vector3(i, j, 0f));
            }
        }
    }

    public void PlaceEntrancePortal(int portalID)
    {
        PortalObject nPortal = CreatePortal();
        _portals.Add(nPortal);

        Vector2 worldPos;
        if (_data.exDirection == ERoomDirection.Top)
        {
            if (_data.enDirection == ERoomDirection.Left) worldPos = transform.position + Vector3.right;
            else worldPos = transform.position + (Vector3.right * 6);
        }
        else
        {
            if (_data.exDirection == ERoomDirection.Left) worldPos = transform.position + Vector3.right;
            else worldPos = transform.position + (Vector3.right * 6);
        }

        nPortal.Init(worldPos, portalID);
    }

    public void PlaceExitPortal(int portalID)
    {
        PortalObject nPortal = CreatePortal();
        _portals.Add(nPortal);

        Vector2 worldPos;
        if (_data.enDirection == ERoomDirection.Top)
        {
            if (_data.exDirection == ERoomDirection.Right || _data.exDirection == ERoomDirection.Top) worldPos = transform.position + Vector3.right;
            else worldPos = transform.position + (Vector3.right * 6);
        }
        else
        {
            if (_data.enDirection == ERoomDirection.Right) worldPos = transform.position + Vector3.right;
            else worldPos = transform.position + (Vector3.right * 6);
        }

        nPortal.Init(worldPos, portalID);
    }

    private PortalObject CreatePortal()
    {
        PortalObject nPortal = new GameObject("Portal").AddComponent<PortalObject>();
        nPortal.transform.SetParent(transform);
        return nPortal;
    }
}
