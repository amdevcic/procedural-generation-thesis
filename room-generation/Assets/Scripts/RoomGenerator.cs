using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    List<Room> rooms;
    public int totalWidth;
    public int totalLength;
    [Range(0, 1)]
    public float wallThickness;
    public float wallHeight;
    public List<GameObject> roomPrefabs;
    private Bounds bounds;
    Camera mapCamera;

    private void Awake() {
        rooms = new List<Room>();
        mapCamera = GetComponentInChildren<Camera>();
    }

    public void Generate() {
        mapCamera.orthographicSize = Mathf.Max(totalLength, totalWidth)/2;
        StartCoroutine("GenerateCoroutine");
    }

    IEnumerator GenerateCoroutine() {
        bounds = new Bounds();
        List<Room.Door> queue = new List<Room.Door>();
        DestroyAll();
        Room room = Instantiate(
            roomPrefabs[Random.Range(0, roomPrefabs.Count)],
            transform.position,
            Quaternion.identity).GetComponent<Room>();
        room.Initialize(
            Random.Range(room.minSize.x, room.maxSize.x),
            Random.Range(room.minSize.y, room.maxSize.y),
            wallHeight,
            wallThickness*0.5f
        );
        room.transform.position += 10*Vector3.forward;
        rooms.Add(room);
        room.AddDoor(Random.Range(1, room.size.y-room.doorWidth), RoomMeshGenerator.Wall.Right);
        bounds = UpdateBounds(room);
        foreach(Room.Door door in room.GetAllDoors()) {
            queue.Add(door);
        }
        Room.Door currentDoor;
        while (queue.Count>0) {
            currentDoor = queue[0];
            queue.RemoveAt(0);
            room = Instantiate(
                roomPrefabs[Random.Range(0, roomPrefabs.Count)],
                transform.position,
                Quaternion.identity).GetComponent<Room>();
            room.Initialize(
                Random.Range(room.minSize.x, room.maxSize.x),
                Random.Range(room.minSize.y, room.maxSize.y),
                wallHeight,
                wallThickness*0.5f
            );
                room.LineUpWith(currentDoor);
            yield return new WaitForFixedUpdate();
            if (room.GetCollisions() == 0 && IsWithinBounds(room)) {
                rooms.Add(room);
                bounds = UpdateBounds(room);
                for (int i=0; i<room.maxBranches; i++) {
                    Room.Door newDoor = AddRandomDoorToRoom(room);
                    if (newDoor.parent != null)
                        queue.Add(newDoor);
                    }
            } else {
                currentDoor.parent.RemoveDoor(currentDoor);
                Destroy(room.gameObject);
            }
            mapCamera.transform.position = bounds.center;
        }
        foreach (Room r in rooms) {
            r.Populate();
        }
        yield return null;
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
}
