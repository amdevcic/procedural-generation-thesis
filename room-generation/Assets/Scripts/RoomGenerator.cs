using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    List<Room> rooms;
    public int totalWidth;
    [Range(0, 1)]
    public float wallThickness;
    public float wallHeight;
    public List<GameObject> roomPrefabs;

    private void Awake() {
        rooms = new List<Room>();
    }

    public void Generate() {
        StartCoroutine("GenerateCoroutine");
    }

    IEnumerator GenerateCoroutine() {
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
        rooms.Add(room);
        room.AddDoor(Random.Range(1, room.size.y-room.doorWidth), RoomMeshGenerator.Wall.Right);
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
            if (room.GetCollisions() == 0) {
                rooms.Add(room);
                if (room.transform.position.x < totalWidth) {
                    room.AddDoor(Random.Range(1, room.size.y-room.doorWidth), RoomMeshGenerator.Wall.Right);
                    foreach(Room.Door door in room.GetAllDoors()) {
                        queue.Add(door);
                    }
                }
            } else {
                Destroy(room.gameObject);
            }
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

    public void SetTotalWidth(float width) {
        totalWidth = (int)width;
    }
    public void SetWallThickness(float thickness) {
        wallThickness = thickness;
    }
    public void SetWallHeight(float height) {
        wallHeight = height;
    }
}
