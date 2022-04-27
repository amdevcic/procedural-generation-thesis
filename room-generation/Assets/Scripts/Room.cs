using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    public int width, length;
    private int height = 3;
    RoomMeshGenerator meshGenerator;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshGenerator = new RoomMeshGenerator(width, height, length);
        Build();
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void Build()
    {
        mesh.Clear();
        mesh.vertices = meshGenerator.getMeshVertices();
        mesh.triangles = meshGenerator.getMeshTriangles();
        mesh.RecalculateNormals();
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(GetCenter(), new Vector3(width, height, length));
    }

    Vector3 GetCenter() {
        return new Vector3(
            transform.position.x + (float)width/2,
            transform.position.y + (float)height/2,
            transform.position.z + (float)length/2
            );
    }
}
