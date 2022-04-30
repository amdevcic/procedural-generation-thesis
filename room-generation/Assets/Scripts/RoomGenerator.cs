using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    List<Room> rooms;
    public int totalWidth;
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
            room.SetRandomDimensions();
            offset.x += room.width;
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
}
