using UnityEngine;
using System.Collections.Generic;

public class RoomMeshGenerator
{ 
    private int width, length, doorWidth;
    private float offset, height;
    List<Vector3> verts;
    List<int> indices;
    List<Vector2> uvs;
    private float textureScaleFactor = 0.6f;
    public enum Wall {
        Left, Top, Right, Bottom 
    }

    public RoomMeshGenerator(int width, float height, int length, int doorWidth, float offset)
    {
        this.width = width;
        this.height = height;
        this.length = length;
        this.doorWidth = doorWidth;
        this.offset = offset;
        
    }

    public void Clear() {
        verts = new List<Vector3>();
        indices = new List<int>();
        uvs = new List<Vector2>();
    }


    public bool CheckIfPositionValid(int position, Wall wall, List<int> doors) {
        // Debug.Log($"{position}, {wall}, {GetWallWidth(wall)}");
        if (doors != null) {
            foreach (int p in doors) {
                if ( p>=position && p<=position+doorWidth
                  || position>=p && position<=p+doorWidth){
                  Debug.Log("collides with door");
                  return false;
                  }
            }
            return position>=0 && position+doorWidth<=GetWallWidth(wall);
            }
        else
            return position>=0 && position<GetWallWidth(wall);
    }

    public int GetWallWidth(Wall wall) {
        switch(wall) {
            case Wall.Left:
            case Wall.Right:
                return length;
            case Wall.Top:
            case Wall.Bottom:
                return width;
            default:
                return 0;
        }
    }

    //returns clockwise direction of wall
    public Vector3 getWallDir(Wall wall) {
        switch(wall) {
            case Wall.Left:
                return Vector3.forward;
            case Wall.Top:
                return Vector3.right;
            case Wall.Right:
                return Vector3.back;
            case Wall.Bottom:
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    } 

    public Wall GetOppositeWall (Wall wall) {
        switch(wall) {
            case Wall.Left:
                return Wall.Right;
            case Wall.Top:
                return Wall.Bottom;
            case Wall.Right:
                return Wall.Left;
            case Wall.Bottom:
                return Wall.Top;
            default:
                return Wall.Right;
        }
    }

    public void CreateFloorAndCeiling() {
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
    public void CreateWall(Vector3 start, Vector3 end, List<int> doorPos) {
        float wallLength = (end-start).magnitude;
        Vector3 wallDir = (end-start).normalized;
        Vector3 wallStart = start, wallEnd = start;
        foreach (int i in doorPos) {
            // wallEnd = wallStart+(i-offset)*wallDir;
            wallEnd = start+(i-offset)*wallDir;
            // Debug.Log($"{wallStart} to {wallEnd}");
            AddQuad(
                wallStart + height*Vector3.up,
                wallEnd + height*Vector3.up,
                wallStart,
                wallEnd
            );
            wallStart = wallEnd + doorWidth*wallDir;
            CreateDoorway(wallEnd, wallStart);
        }
        wallEnd = end;
        AddQuad(
            wallStart + height*Vector3.up,
            wallEnd + height*Vector3.up,
            wallStart,
            wallEnd
        );
    }

    private void CreateDoorway(Vector3 start, Vector3 end) {
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

        float w = (br-bl).magnitude*textureScaleFactor, h = (tl-bl).magnitude*textureScaleFactor;
        
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