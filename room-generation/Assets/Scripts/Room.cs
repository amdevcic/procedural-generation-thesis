using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
public class Room : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    public int width, length;
    private int minWidth = 3, maxWidth = 10, minLength = 3, maxLength = 10;
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

    public void SetRandomDimensions() {
            width = Random.Range(minWidth, maxWidth);
            length = Random.Range(minLength, maxLength);
    }

    public Vector3 GetCenter() {
        return new Vector3(
            transform.position.x + (float)width/2,
            transform.position.y + (float)height/2,
            transform.position.z + (float)length/2
            );
    }
}
