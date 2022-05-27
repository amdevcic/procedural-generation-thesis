using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
public class Room : MonoBehaviour
{
    Mesh mesh;
    public int width, length, doorWidth = 2;
    [SerializeField]
    private int minWidth = 3, maxWidth = 10, minLength = 3, maxLength = 10;
    private int height = 3;
    RoomMeshGenerator meshGenerator;
    public List<RoomItem> items;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshGenerator = new RoomMeshGenerator(width, height, length, doorWidth);

        // for testing only
        meshGenerator.AddDoor(1, RoomMeshGenerator.Wall.Left);
        meshGenerator.AddDoor(length - doorWidth - 1, RoomMeshGenerator.Wall.Right);
        
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
        Gizmos.DrawWireCube(GetCenter(), new Vector3(width, height, length));
    }

    public void SetRandomDimensions() {
            width = Random.Range(minWidth, maxWidth);
            length = Random.Range(minLength, maxLength);
    }

    public Vector3 GetCenter() {
        return new Vector3(
            transform.position.x + (float)width*0.5f,
            transform.position.y + (float)height*0.5f,
            transform.position.z + (float)length*0.5f
            );
    }

    private IEnumerable<Vector3> GetCorners(float offset) {
        float xOffset = width * offset * 0.5f, zOffset = length * offset * 0.5f;
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
