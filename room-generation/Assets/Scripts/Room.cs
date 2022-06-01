using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Room : MonoBehaviour
{
    public struct Door {
        public int position;
        public RoomMeshGenerator.Wall wall;
        public Room parent;
        public Door(int position, RoomMeshGenerator.Wall wall, Room parent) {
            this.position = position;
            this.wall = wall;
            this.parent = parent;
        }
    }
    
    public List<int>[] doors = new List<int>[4];
    Mesh mesh;
    public int doorWidth = 1;
    private float offset;
    public Vector2Int size, minSize, maxSize;
    private float height = 3f;
    public RoomMeshGenerator meshGenerator;
    public List<RoomItem> items;
    int collisions = 0;
    Vector3[] corners = new Vector3[4];

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 6)
            collisions++;
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == 6)
            collisions--;
    }

    public int GetCollisions() {
        return collisions;
    }

    public void Rebuild()
    {
        meshGenerator.Clear();

        meshGenerator.CreateFloorAndCeiling();
        meshGenerator.CreateWall(new Vector3(offset, 0, offset), new Vector3(offset, 0, size.y-offset), doors[(int) RoomMeshGenerator.Wall.Left]);                  // left
        meshGenerator.CreateWall(new Vector3(offset, 0, size.y-offset), new Vector3(size.x-offset, 0, size.y-offset), doors[(int) RoomMeshGenerator.Wall.Top]);      // top
        meshGenerator.CreateWall(new Vector3(size.x-offset, 0, size.y-offset), new Vector3(size.x-offset, 0, offset), doors[(int) RoomMeshGenerator.Wall.Right]);     // right
        meshGenerator.CreateWall(new Vector3(size.x-offset, 0, offset), new Vector3(offset, 0, offset), doors[(int) RoomMeshGenerator.Wall.Bottom]);                 // bottom

        mesh.Clear();
        mesh.SetVertices(meshGenerator.GetVerts());
        mesh.SetTriangles(meshGenerator.GetIndices(), 0);
        mesh.SetUVs(0, meshGenerator.GetUvs());
        mesh.RecalculateNormals();
        
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        GetComponent<BoxCollider>().center = GetCenter()-transform.position;
        GetComponent<BoxCollider>().size = new Vector3(size.x-offset*2, height, size.y-offset*2);

    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(GetCenter(), new Vector3(size.x, height, size.y));
    }

    public void Initialize(int width, int length, float height, float offset) {
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        this.size.x = width;
        this.size.y = length;
        this.offset = offset;
        this.height = height;
        
        for (int i=0; i<4; i++) {
            doors[i] = new List<int>();
        }
        meshGenerator = new RoomMeshGenerator(width, height, length, doorWidth, offset);
        
        GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;

        gameObject.layer = 6;
        corners = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 0, length), new Vector3(width, 0, length), new Vector3(width, 0, 0)};

        Rebuild();
        Populate();
    }

    public Vector3 GetCenter() {
        return new Vector3(
            transform.position.x + (float)size.x*0.5f,
            transform.position.y + (float)height*0.5f,
            transform.position.z + (float)size.y*0.5f
            );
    }

    public Door AddDoor(int position, RoomMeshGenerator.Wall wall) {
        if (meshGenerator.CheckIfPositionValid(position, wall, true)) {
            doors[(int) wall].Add(position);
            Rebuild();
            return new Door(position, wall, this);
        } else
            return new Door(0, 0, null);
    }

    private IEnumerable<Vector3> GetCorners(float offset) {
        float xOffset = size.x * offset * 0.5f, zOffset = size.y * offset * 0.5f;
        yield return new Vector3(-xOffset, 0,  zOffset);
        yield return new Vector3( xOffset, 0,  zOffset);
        yield return new Vector3(-xOffset, 0, -zOffset);
        yield return new Vector3( xOffset, 0, -zOffset);
        yield break;
    }

    void Populate() {
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
    
    public IEnumerable<Door> GetAllDoors() {
        for (int i=0; i<4; i++) {
            foreach (int doorPos in doors[i]) {
                yield return new Door(doorPos, (RoomMeshGenerator.Wall) i, this);
            }
        }
        yield break;
    }

    public void RemoveDoor(Door door) {
        doors[(int) door.wall].Remove(door.position);
        Rebuild();
    }

    public void LineUpWith(Door door) {
        Room parent = door.parent;
        Door newDoor = AddDoor(Random.Range(1, size.y-doorWidth), meshGenerator.GetOppositeWall(door.wall));
        transform.position = parent.transform.position + parent.GetVectorToDoor(door) - GetVectorToDoor(newDoor) - meshGenerator.getWallDir(newDoor.wall)*doorWidth;
    }

    public Vector3 GetVectorToDoor(Door door) {
        return corners[(int) door.wall] + meshGenerator.getWallDir(door.wall)*door.position;
    }
}
