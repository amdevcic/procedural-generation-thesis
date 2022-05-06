using UnityEngine;

//[CreateAssetMenu(fileName = "RoomItem", menuName = "room-generation/RoomItem", order = 0)]
[System.Serializable]
public class RoomItem {

    public enum Position
    {
        Corners, Center
    }

    public GameObject prefab;

    [Range(0f, 1f)]
    public float offset;
    public Position pos;
}