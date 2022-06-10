using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomGenerator : MonoBehaviour
{
    List<Room> rooms;
    public int totalWidth;
    public int totalLength;
    [Range(0, 1)]
    public float wallThickness;
    public float wallHeight;
    public int attempts;
    public List<RoomData> roomTypes;
    private Bounds bounds;
    private bool bfs = true;
    Camera mapCamera;
    GameObject baseRoom;
    public Text timeElapsedText;
    float timeElapsed;

    private void Awake() {
        rooms = new List<Room>();
        mapCamera = GetComponentInChildren<Camera>();
        baseRoom = new GameObject("Room");
        baseRoom.AddComponent<Room>();
    }

    public void Generate() {
        mapCamera.orthographicSize = Mathf.Max(totalLength, totalWidth)/2;
        StartCoroutine("GenerateCoroutine");
    }

    IEnumerator GenerateCoroutine() {
        timeElapsed = Time.time;
        bounds = new Bounds();
        List<Room.Door> queue = new List<Room.Door>();
        DestroyAll();
        RoomData data = roomTypes[Random.Range(0, roomTypes.Count)];
        Room room = Instantiate(
            baseRoom,
            transform.position,
            Quaternion.identity).GetComponent<Room>();
        room.Initialize(
            Random.Range(data.minSize.x, data.maxSize.x),
            Random.Range(data.minSize.y, data.maxSize.y),
            wallHeight,
            wallThickness*0.5f,
            data
        );
        rooms.Add(room);
        AddRandomDoorToRoom(room);
        bounds = UpdateBounds(room);
        foreach(Room.Door door in room.GetAllDoors()) {
            queue.Add(door);
        }
        Room.Door currentDoor;
        while (queue.Count>0) {
            currentDoor = PopDoor(queue);
            bool success = false;
            for (int i=0; i<attempts; i++) {
                data = roomTypes[Random.Range(0, roomTypes.Count)];
                room = Instantiate(
                    baseRoom,
                    transform.position,
                    Quaternion.identity).GetComponent<Room>();
                room.Initialize(
                    Random.Range(data.minSize.x, data.maxSize.x),
                    Random.Range(data.minSize.y, data.maxSize.y),
                    wallHeight,
                    wallThickness*0.5f,
                    data
                );
                room.LineUpWith(currentDoor);
                yield return new WaitForFixedUpdate();
                if (room.GetCollisions() == 0 && IsWithinBounds(room)) {
                    rooms.Add(room);
                    bounds = UpdateBounds(room);
                    for (int j=0; j<data.maxBranches; j++) {
                        Room.Door newDoor = AddRandomDoorToRoom(room);
                        if (newDoor.parent != null)
                            queue.Add(newDoor);
                    }
                    success = true;
                    break;
                } else {
                    Destroy(room.gameObject);
                }
            }
            if (!success)
                currentDoor.parent.RemoveDoor(currentDoor);
            mapCamera.transform.position = bounds.center;
        }
        foreach (Room r in rooms) {
            r.Populate();
        }
        timeElapsed = Time.time - timeElapsed;
        timeElapsedText.text = "Time taken: "+timeElapsed.ToString("0.00")+" seconds";
        yield return null;
    }

    private Room.Door PopDoor(List<Room.Door> queue) {
        Room.Door door;
        if (bfs) {
            door = queue[0];
            queue.RemoveAt(0);
        }
        else {
            door = queue[queue.Count-1];
            queue.RemoveAt(queue.Count-1);
        }
        return door;
    }

    private void DestroyAll() {
        foreach (Room room in rooms) {
            Destroy(room.gameObject);
        }
        rooms = new List<Room>();
    }

    public Vector3 GetStartingPosition() {
        if (rooms.Count > 0){
            return rooms[0].GetCenter();
        }
        else
            return Vector3.zero;
    }

    Room.Door AddRandomDoorToRoom(Room room) {
        int wall = Random.Range(0, 4);
        return room.AddDoor(Random.Range(1, room.meshGenerator.GetWallWidth((RoomMeshGenerator.Wall)wall)-room.doorWidth), (RoomMeshGenerator.Wall)wall);
    }

    Bounds UpdateBounds(Room room) {
        Bounds newbounds = this.bounds;
        foreach (Vector3 corner in room.GetCorners(0)) {
            newbounds.Encapsulate(corner+room.GetCenter());
        }
        return newbounds;
    }

    bool IsWithinBounds(Room room) {
        Bounds check = UpdateBounds(room);
        return check.size.x <= totalWidth && check.size.z <= totalLength;
    }

    public void SetTotalWidth(float width) {
        totalWidth = (int)width;
    }
    public void SetTotalLength(float length) {
        totalLength = (int)length;
    }
    public void SetWallThickness(float thickness) {
        wallThickness = thickness;
    }
    public void SetWallHeight(float height) {
        wallHeight = height;
    }
    public void SetAttempts(float attempts) {
        this.attempts = (int)attempts;
    }

    public void SetSearchType(bool bfs) {
        this.bfs = bfs;
    }
}
