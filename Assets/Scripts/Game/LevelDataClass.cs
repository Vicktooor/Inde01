using System;
using System.Collections.Generic;
using UnityEngine;

public enum ERoomType { None, Classic, Start, End }
public enum ELayoutType { None, L1, L2, L3 }
public enum ERoomDirection { Left, Right, Top, Bottom }
public enum ELayoutCell { Void, Portal, Platform, InFieldPlatform, }

public class Level
{
    private int ID_ROOM_ITERATOR = 0;
    private int ID_PORTAL_ITERATOR = 0;

    public static IntVector2 LEVEL_SIZE = new IntVector2(4, 6);

    private Room[,] _rooms = new Room[LEVEL_SIZE.x, LEVEL_SIZE.y];
    public int RoomNumber { get { return ID_ROOM_ITERATOR + 1; } }

    public List<PortalCouple> portalCouples = new List<PortalCouple>();
    public List<Room> roomsList = new List<Room>();

    public void Generate()
    {
        int randStart = UnityEngine.Random.Range(0, LEVEL_SIZE.x);
        GeneratePath(new IntVector2(randStart, 0), ERoomType.Start, ERoomDirection.Top);
        GeneratePortals();
        GenerateRoomsConnections();
    }

    private void GeneratePath(IntVector2 refPos, ERoomType roomType, ERoomDirection fromDir)
    {
        Room newRoom = new Room()
        {
            ID = ID_ROOM_ITERATOR,
            type = roomType,
            gridPos = refPos,
            enDirection = fromDir
        };
        _rooms[newRoom.gridPos.x, newRoom.gridPos.y] = newRoom;

        ERoomDirection nextDir = roomType == ERoomType.End ? ERoomDirection.Top : FindDirection(refPos, fromDir);
        refPos.x += nextDir == ERoomDirection.Left ? -1 : nextDir == ERoomDirection.Right ? 1 : 0;
        refPos.y += nextDir == ERoomDirection.Top ? 1 : 0;

        newRoom.exDirection = nextDir;     
        roomsList.Add(newRoom);
        ID_ROOM_ITERATOR++;

        bool reachTop = roomsList.Find(r => r.gridPos.y == LEVEL_SIZE.y - 1) != null;
        if (roomType != ERoomType.End) GeneratePath(refPos, RoomNumber >= 12 && reachTop ? ERoomType.End : ERoomType.Classic, nextDir);
    }

    private ERoomDirection FindDirection(IntVector2 gridPos, ERoomDirection previousDirection)
    {
        List<ERoomDirection> directions = new List<ERoomDirection>() { ERoomDirection.Left, ERoomDirection.Right, ERoomDirection.Top };
        if (gridPos.x <= 0) directions.Remove(ERoomDirection.Left);
        if (gridPos.x >= LEVEL_SIZE.x - 1) directions.Remove(ERoomDirection.Right);
        if (gridPos.y >= LEVEL_SIZE.y - 1) directions.Remove(ERoomDirection.Top);

        directions.Remove(previousDirection == ERoomDirection.Left ? ERoomDirection.Right : previousDirection == ERoomDirection.Right ? ERoomDirection.Left : previousDirection);
        if (gridPos.y == 1 && previousDirection == ERoomDirection.Top) directions.Add(previousDirection);

        return directions[UnityEngine.Random.Range(0, directions.Count)];
    }

    private void GeneratePortals()
    {
        List<int> entranceRooms = new List<int>();

        Room prevRoom;
        foreach (Room r in roomsList)
        {
            if (roomsList.IndexOf(r) > 0)
            {
                prevRoom = roomsList[roomsList.IndexOf(r) - 1];
                if (prevRoom.gridPos.y < r.gridPos.y)
                {
                    portalCouples.Add(new PortalCouple()
                    {
                        ID = ID_PORTAL_ITERATOR,
                        enRoomID = prevRoom.ID,
                        exRoomID = r.ID
                    });
                    ID_PORTAL_ITERATOR++;
                    entranceRooms.Add(prevRoom.ID);
                }
            }
        }

        List<Room> emptyPortalRooms = roomsList.FindAll(r => r.type != ERoomType.End && !entranceRooms.Contains(r.ID));
        AddLastPortals(emptyPortalRooms);
    }

    private void AddLastPortals(List<Room> pRooms)
    {
        Room room;
        int randPortal = UnityEngine.Random.Range(0, pRooms.Count - 1);
        room = pRooms[randPortal];
        portalCouples.Add(new PortalCouple()
        {
            ID = ID_PORTAL_ITERATOR,
            enRoomID = room.ID,
            exRoomID = room.ID + 1
        });
        ID_PORTAL_ITERATOR++;
        pRooms.Remove(room);
        if (ID_PORTAL_ITERATOR < 7) AddLastPortals(pRooms);
    }

    private void GenerateRoomsConnections()
    {
        // R3 rooms
        List<Room> rooms = roomsList.FindAll(r => portalCouples.Find(pc => pc.exRoomID == r.ID) != null);
        foreach (Room r in rooms) r.layoutType = ELayoutType.L3;

        // R1 rooms
        rooms = roomsList.FindAll(r => portalCouples.Find(pc => pc.exRoomID == r.ID) == null);
        foreach (Room r in rooms) r.layoutType = ELayoutType.L1;

        // R2 rooms
        rooms = roomsList.FindAll(r => roomsList.Find(ir => ir.gridPos.x == r.gridPos.x && ir.gridPos.y == r.gridPos.y + 1) != null);
        foreach (Room r in rooms)
        {
            r.layoutType = MathCustom.RandomBool() ? ELayoutType.L2 : r.layoutType;
            if (r.layoutType == ELayoutType.L2)
            {
                _rooms[r.gridPos.x, r.gridPos.y + 1].openSides.Add(ERoomDirection.Bottom);
                r.openSides.Add(ERoomDirection.Top);
            }
        }

        foreach (Room r in roomsList)
        {
            if (portalCouples.Find(pc => pc.enRoomID == r.ID) == null)
            {
                if (r.exDirection == ERoomDirection.Right)
                {
                    r.openSides.Add(ERoomDirection.Right);
                    _rooms[r.gridPos.x + 1, r.gridPos.y].openSides.Add(ERoomDirection.Left);
                }
                else if (r.exDirection == ERoomDirection.Left)
                {
                    r.openSides.Add(ERoomDirection.Left);
                    _rooms[r.gridPos.x - 1, r.gridPos.y].openSides.Add(ERoomDirection.Right);
                }
            }
        }
    }
}

public class Room
{
    public static IntVector2 ROOM_SIZE = new IntVector2(8, 8);

    public int ID;
    public ERoomType type;
    public ELayoutType layoutType;
    public IntVector2 gridPos;
    public ERoomDirection enDirection;
    public ERoomDirection exDirection;
    public List<ERoomDirection> openSides = new List<ERoomDirection>();
    public LayoutCell[,] layout = new LayoutCell[ROOM_SIZE.x - 2, ROOM_SIZE.y - 2];
}

public struct LayoutCell
{
    
}

public class PortalCouple
{
    public int ID;
    public int enRoomID;
    public int exRoomID;
}
