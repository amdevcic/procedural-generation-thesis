using UnityEngine;
using System.Collections.Generic;

public class RoomMeshGenerator
{ 
    private int width, height, length, doorWidth;
    List<Vector3> verts;
    List<int> indices;
    List<Vector2> uvs;
    // private float textureScaleFactor = 0.25f;
    private List<int>[] doors = new List<int>[4];
    public enum Wall {
        Left, Top, Right, Bottom 
    }

    public RoomMeshGenerator(int width, int height, int length, int doorWidth)
    {
        this.width = width;
        this.height = height;
        this.length = length;
        this.doorWidth = doorWidth;
        
        for (int i=0; i<4; i++) {
            doors[i] = new List<int>();
        }
        // doors[(int) Wall.Left].Add(1);
        // doors[(int) Wall.Right].Add(length-doorWidth-1);
    }

    public void Build() {
        verts = new List<Vector3>();
        indices = new List<int>();
        uvs = new List<Vector2>();

        CreateFloorAndCeiling();
        CreateWall(new Vector3(0, 0, 0), new Vector3(0, 0, length), doors[(int) Wall.Left]);            // left
        CreateWall(new Vector3(0, 0, length), new Vector3(width, 0, length), doors[(int) Wall.Top]);    // top
        CreateWall(new Vector3(width, 0, length), new Vector3(width, 0, 0), doors[(int) Wall.Right]);   // right
        CreateWall(new Vector3(width, 0, 0), new Vector3(0, 0, 0), doors[(int) Wall.Bottom]);           // bottom
    }

    public void AddDoor(int position, Wall wall) {
        doors[(int) wall].Add(position);
    }

    private void CreateFloorAndCeiling() {
        AddQuad(
            new Vector3(0, 0, length),
            new Vector3(width, 0, length),
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0)
        );
        AddQuad(
            new Vector3(0, height, 0),
            new Vector3(width, height, 0),
            new Vector3(0, height, length),
            new Vector3(width, height, length)
        );
    }
    private void CreateWall(Vector3 start, Vector3 end, List<int> doorPos) {
        float wallLength = (end-start).magnitude;
        Vector3 wallDir = (end-start).normalized;
        Vector3 wallStart = start, wallEnd = start;
        foreach (int i in doorPos) {
            wallEnd = wallStart+i*wallDir;
            AddQuad(
                wallStart + height*Vector3.up,
                wallEnd + height*Vector3.up,
                wallStart,
                wallEnd
            );
            wallStart = wallEnd + doorWidth*wallDir;
        }
        wallEnd = end;
        AddQuad(
            wallStart + height*Vector3.up,
            wallEnd + height*Vector3.up,
            wallStart,
            wallEnd
        );
    }

    private void AddQuad(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br) {
        int n = verts.Count;
        verts.Add(tl);
        verts.Add(tr);
        verts.Add(bl);
        verts.Add(br);

        // TODO: fix uvs
        uvs.Add(tl);
        uvs.Add(tr);
        uvs.Add(bl);
        uvs.Add(br);


        indices.Add(n);
        indices.Add(n+1);
        indices.Add(n+2);
        indices.Add(n+1);
        indices.Add(n+3);
        indices.Add(n+2);
    }

    public List<Vector3> GetVerts() {
        return verts;
    }
    public List<int> GetIndices() {
        return indices;
    }
    public List<Vector2> GetUvs() {
        return uvs;
    }
}