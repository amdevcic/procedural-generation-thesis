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
    public int maxBranches = 3;
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
        mesh = meshGenerator.getMesh();
        mesh.RecalculateNormals();
        
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<BoxCollider>().center = GetCenter()-transform.position;
        GetComponent<BoxCollider>().size = new Vector3(size.x-offset*2, height, size.y-offset*2);

    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(GetCenter(), new Vector3(size.x, height, size.y));
    }

    public void Initialize(int width, int length, float height, float offset) {
        
        mesh = new Mesh();

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
        // Populate();
    }

    public Vector3 GetCenter() {
        return new Vector3(
            transform.position.x + (float)size.x*0.5f,
            transform.position.y + (float)height*0.5f,
            transform.position.z + (float)size.y*0.5f
            );
    }

    public Door AddDoor(int position, RoomMeshGenerator.Wall wall) {
        if (meshGenerator.CheckIfPositionValid(position, wall, doors[(int)wall], true)) {
            doors[(int) wall].Add(position);
            Rebuild();
            return new Door(position, wall, this);
        } else
            return new Door(0, 0, null);
    }

    public IEnumerable<Vector3> GetCorners(float offset) {
        float xOffset = size.x * offset * 0.5f, zOffset = size.y * offset * 0.5f;
        yield return new Vector3(-xOffset, 0,  zOffset);
        yield return new Vector3( xOffset, 0,  zOffset);
        yield return new Vector3(-xOffset, 0, -zOffset);
        yield return new Vector3( xOffset, 0, -zOffset);
        yield break;
    }

    public void Populate() {
        Vector3 center = GetCenter() + height*0.5f*Vector3.down;
        foreach (RoomItem item in items)
        {
            if (item.yStretch) {
                item.prefab.transform.localScale = new Vector3(1, height, 1);
            }
            switch (item.pos) {
                case RoomItem.Position.Corners:
                    foreach (Vector3 corner in GetCorners(item.offset))
                        Instantiate(item.prefab, center + corner, Quaternion.identity, gameObject.transform);
                    break;
                case RoomItem.Position.Center:
                    Instantiate(item.prefab, center+height*item.offset*Vector3.up, Quaternion.identity, gameObject.transform);
                    break;
                case RoomItem.Position.Walls:
                    for (int i=0; i<4; i++) {
                        for (int j=1; j<meshGenerator.GetWallWidth(i); j += (int) (1/item.frequency)) {
                            Vector3 walldir = meshGenerator.getWallDir((RoomMeshGenerator.Wall)i);
                            Vector3 wallnormal = Vector3.Cross(walldir, Vector3.down);
                            if (meshGenerator.CheckIfPositionValid(j, (RoomMeshGenerator.Wall)i, doors[i], false)) {
                                Instantiate(item.prefab, 
                                transform.position + corners[i] + j*walldir + height*item.offset*Vector3.up
                                + wallnormal*offset,
                                Quaternion.LookRotation(wallnormal, Vector3.up), gameObject.transform);
                            }
                        }
                    }
                    break;
                case RoomItem.Position.Random:
                    Vector3 pos = new Vector3(
                        Random.Range(1, meshGenerator.GetWallWidth(1)),
                        height*item.offset,
                        Random.Range(1, meshGenerator.GetWallWidth(0))
                    );
                    Instantiate(item.prefab, transform.position + pos, Quaternion.Euler(0, Random.Range(0, 360), 0), gameObject.transform);
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
        RoomMeshGenerator.Wall opposite = meshGenerator.GetOppositeWall(door.wall);
        Door newDoor = AddDoor(Random.Range(1, meshGenerator.GetWallWidth(opposite)-doorWidth), opposite);
        transform.position = parent.transform.position + parent.GetVectorToDoor(door) - GetVectorToDoor(newDoor) - meshGenerator.getWallDir(newDoor.wall)*doorWidth;
    }

    public Vector3 GetVectorToDoor(Door door) {
        return corners[(int) door.wall] + meshGenerator.getWallDir(door.wall)*door.position;
    }
}
