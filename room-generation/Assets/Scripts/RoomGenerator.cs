using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    List<Room> rooms;
    public int minWidth, maxWidth, minLength, maxLength;
    public int numberOfRooms;
    public GameObject roomPrefab;

    void Awake()
    {
        rooms = new List<Room>();
        Vector3 offset = Vector3.zero;
        for (int i=0; i<numberOfRooms; i++) {
            GameObject roomObject = Instantiate(
                roomPrefab, 
                transform.position + offset,
                Quaternion.identity) as GameObject;
            Room room = roomObject.GetComponent<Room>();
            room.width = Random.Range(minWidth, maxWidth);
            room.length = Random.Range(minLength, maxLength);
            offset.x += room.width;
            rooms.Add(room);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
