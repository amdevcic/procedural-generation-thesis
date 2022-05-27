using UnityEngine;
using System.Collections.Generic;

public class RoomMeshGenerator
{ 
    private int width, height, length, doorWidth;
    private float offset;
    List<Vector3> verts;
    List<int> indices;
    List<Vector2> uvs;
    // private float textureScaleFactor = 0.25f;
    private List<int>[] doors = new List<int>[4];
    public enum Wall {
        Left, Top, Right, Bottom 
    }

    public RoomMeshGenerator(int width, int height, int length, int doorWidth, float offset)
    {
        this.width = width;
        this.height = height;
        this.length = length;
        this.doorWidth = doorWidth;
        this.offset = offset;
        
        for (int i=0; i<4; i++) {
            doors[i] = new List<int>();
        }
    }

    public void Build() {
        verts = new List<Vector3>();
        indices = new List<int>();
        uvs = new List<Vector2>();

        CreateFloorAndCeiling();
        CreateWall(new Vector3(offset, 0, offset), new Vector3(offset, 0, length-offset), doors[(int) Wall.Left]);                  // left
        CreateWall(new Vector3(offset, 0, length-offset), new Vector3(width-offset, 0, length-offset), doors[(int) Wall.Top]);      // top
        CreateWall(new Vector3(width-offset, 0, length-offset), new Vector3(width-offset, 0, offset), doors[(int) Wall.Right]);     // right
        CreateWall(new Vector3(width-offset, 0, offset), new Vector3(offset, 0, offset), doors[(int) Wall.Bottom]);                 // bottom
    }

    public void AddDoor(int position, Wall wall) {
        doors[(int) wall].Add(position);
    }

    private void CreateFloorAndCeiling() {
        AddQuad(
            new Vector3(offset, 0, length-offset),
            new Vector3(width-offset, 0, length-offset),
            new Vector3(offset, 0, offset),
            new Vector3(width-offset, 0, offset)
        );
        AddQuad(
            new Vector3(offset, height, offset),
            new Vector3(width-offset, height, offset),
            new Vector3(offset, height, length-offset),
            new Vector3(width-offset, height, length-offset)
        );
    }
    private void CreateWall(Vector3 start, Vector3 end, List<int> doorPos) {
        float wallLength = (end-start).magnitude;
        Vector3 wallDir = (end-start).normalized;
        Vector3 wallStart = start, wallEnd = start;
        foreach (int i in doorPos) {
            // wallEnd = wallStart+i*wallDir;
            wallEnd = wallStart+(i-offset)*wallDir;
            AddQuad(
                wallStart + height*Vector3.up,
                wallEnd + height*Vector3.up,
                wallStart,
                wallEnd
            );
            wallStart = wallEnd + doorWidth*wallDir;
            AddDoorway(wallEnd, wallStart);
        }
        wallEnd = end;
        AddQuad(
            wallStart + height*Vector3.up,
            wallEnd + height*Vector3.up,
            wallStart,
            wallEnd
        );
    }

    private void AddDoorway(Vector3 start, Vector3 end) {
        Vector3 o = Vector3.Cross((end-start), Vector3.up).normalized*offset;
        Vector3 h = Vector3.up*height;
        AddQuad(start+o, end+o, start, end);
        AddQuad(start+h, end+h, start+o+h, end+o+h);
        AddQuad(start+h, start+o+h, start, start+o);
        AddQuad(end+o+h, end+h, end+o, end);
    }

    private void AddQuad(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br) {
        int n = verts.Count;
        verts.Add(tl);
        verts.Add(tr);
        verts.Add(bl);
        verts.Add(br);

        float w = (br-bl).magnitude*0.6f, h = (tl-bl).magnitude*0.6f;
        
        uvs.Add(new Vector2(0, 0)); // bottom left
        uvs.Add(new Vector2(w, 0)); // top left
        uvs.Add(new Vector2(0, h)); // bottom right
        uvs.Add(new Vector2(w, h)); // top right


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