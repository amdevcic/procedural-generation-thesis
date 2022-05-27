using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
public class Room : MonoBehaviour
{
    Mesh mesh;
    public int doorWidth = 2;
    private float offset;
    public Vector2Int size, minSize, maxSize;
    private int height = 3;
    RoomMeshGenerator meshGenerator;
    public List<RoomItem> items;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshGenerator = new RoomMeshGenerator(size.x, height, size.y, doorWidth, offset);

        // for testing only
        meshGenerator.AddDoor(1, RoomMeshGenerator.Wall.Left);
        meshGenerator.AddDoor(size.y - doorWidth - 1, RoomMeshGenerator.Wall.Right);

        Build();
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        Populate();
    }

    void Build()
    {
        meshGenerator.Build();
        mesh.Clear();
        mesh.SetVertices(meshGenerator.GetVerts());
        mesh.SetTriangles(meshGenerator.GetIndices(), 0);
        mesh.SetUVs(0, meshGenerator.GetUvs());
        mesh.RecalculateNormals();
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(GetCenter(), new Vector3(size.x, height, size.y));
    }

    public void SetDimensions(int width, int length, int height, float offset) {
        this.height = height;
        this.size.x = width;
        this.size.y = length;
        this.offset = offset;
    }

    public Vector3 GetCenter() {
        return new Vector3(
            transform.position.x + (float)size.x*0.5f,
            transform.position.y + (float)height*0.5f,
            transform.position.z + (float)size.y*0.5f
            );
    }

    private IEnumerable<Vector3> GetCorners(float offset) {
        float xOffset = size.x * offset * 0.5f, zOffset = size.y * offset * 0.5f;
        yield return new Vector3(-xOffset, 0,  zOffset);
        yield return new Vector3( xOffset, 0,  zOffset);
        yield return new Vector3(-xOffset, 0, -zOffset);
        yield return new Vector3( xOffset, 0, -zOffset);
        yield break;
    }

    private void Populate() {
        Vector3 center = GetCenter();
        foreach (RoomItem item in items)
        {
            switch (item.pos) {
                case RoomItem.Position.Corners:
                    foreach (Vector3 corner in GetCorners(item.offset))
                        Instantiate(item.prefab, center + corner, Quaternion.identity, gameObject.transform);
                    break;
                case RoomItem.Position.Center:
                    Instantiate(item.prefab, center, Quaternion.identity, gameObject.transform);
                    break;
            }
        }
    }
}
