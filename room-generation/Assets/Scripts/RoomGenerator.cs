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
        DestroyAll();
        Vector3 offset = Vector3.zero;
        while (offset.x < totalWidth) {
            GameObject roomObject = Instantiate(
                roomPrefabs[Random.Range(0, roomPrefabs.Count)],
                transform.position + offset,
                Quaternion.identity) as GameObject;

            Room room = roomObject.GetComponent<Room>();
            room.SetDimensions(
                Random.Range(room.minSize.x, room.maxSize.x),
                Random.Range(room.minSize.y, room.maxSize.y),
                wallHeight,
                wallThickness*0.5f
            );

            offset.x += room.size.x;
            rooms.Add(room);
        }
        Destroy(rooms[rooms.Count-1].gameObject);
        rooms.RemoveAt(rooms.Count-1);
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
