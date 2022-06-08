using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RoomData", menuName = "room-generation/RoomData", order = 0)]
public class RoomData : ScriptableObject {
    public List<RoomItem> items;
    public int maxBranches = 3;
    public Vector2Int minSize, maxSize;
    public Material wallMaterial, floorMaterial;
}