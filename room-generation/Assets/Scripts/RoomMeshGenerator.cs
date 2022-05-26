using UnityEngine;
using System.Collections.Generic;

public class RoomMeshGenerator
{ 
    private int width, height, length;
    List<Vector3> verts;
    List<int> indices;
    List<Vector2> uvs;
    private float textureScaleFactor = 0.25f;
    
    public RoomMeshGenerator(int width, int height, int length)
    {
        this.width = width;
        this.height = height;
        this.length = length;

        verts = new List<Vector3>();
        indices = new List<int>();
        uvs = new List<Vector2>();

        CreateFloorAndCeiling();
        CreateWall(new Vector3(0, 0, 0), new Vector3(0, 0, length));
        CreateWall(new Vector3(0, 0, length), new Vector3(width, 0, length));
        CreateWall(new Vector3(width, 0, length), new Vector3(width, 0, 0));
        CreateWall(new Vector3(width, 0, 0), new Vector3(0, 0, 0));
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
    private void CreateWall(Vector3 start, Vector3 end) { //doors also
        AddQuad(
            start + height*Vector3.up,
            end + height*Vector3.up,
            start,
            end
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