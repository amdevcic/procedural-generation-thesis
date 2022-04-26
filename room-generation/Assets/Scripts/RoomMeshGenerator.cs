using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMeshGenerator
{ 
    private int width, height, length;
    private Vector3[] vertices;
    private int[] faces = new int[] {
        0, 1, 2, 3, // bottom
        6, 7, 4, 5, // top
        0, 4, 1, 5, // left
        3, 7, 2, 6, // right
        2, 6, 0, 4, // front
        1, 5, 3, 7, // back
    };
    public RoomMeshGenerator(int width, int height, int length)
    {
        this.width = width;
        this.height = height;
        this.length = length;

        vertices = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, length),
            new Vector3(width, 0, 0),
            new Vector3(width, 0, length),
            
            new Vector3(0, height, 0),
            new Vector3(0, height, length),
            new Vector3(width, height, 0),
            new Vector3(width, height, length),
        };
    }

    public Vector3[] getMeshVertices() {
        Vector3[] meshVertices = new Vector3[6*4];
        for (int i=0; i<6*4; i++) {
            meshVertices[i] = vertices[faces[i]];
        }
        return meshVertices;
    }

    public int[] getMeshTriangles() {
        int[] meshTriangles = new int[6*2*3];
        for (int i=0; i<6; i++) {
            meshTriangles[i*6] = i*4;
            meshTriangles[i*6+1] = meshTriangles[i*6+3] = i*4+1;
            meshTriangles[i*6+2] = meshTriangles[i*6+5] = i*4+2;
            meshTriangles[i*6+4] = i*4+3;
        }
        
        return meshTriangles;
    }

}