using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Graphs;

public class Generator3D : MonoBehaviour
{
    enum CellType
    {
        // attempt #1
        //RoomBot,
        None,
        Room,
        Hallway,
        Stairs
    }

    class Room
    {
        public BoundsInt bounds;

        public Room(Vector3Int location, Vector3Int size)
        {
            bounds = new BoundsInt(location, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
                || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));
        }
    }

    [SerializeField]
    Vector3Int size;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector3Int roomMinSize;
    [SerializeField]
    Vector3Int roomMaxSize;
    [SerializeField]
    GameObject roomfloorPrefab;
    [SerializeField]
    GameObject hallwayfloorPrefab;
    [SerializeField]
    GameObject stairsPrefab;
    [SerializeField]
    GameObject wallPrefab;
    [SerializeField]
    GameObject inviswallPrefab;
    [SerializeField]
    GameObject PlayerCharacter;
    [SerializeField]
    GameObject[] Enemy;
    [SerializeField]
    GameObject[] Boss;
    [SerializeField]
    int EnemyPerRoomMin;
    [SerializeField]
    int EnemyPerRoomMax;
    [SerializeField]
    GameObject EnvironmentParent;
    [SerializeField]
    GameObject WallParent;
    [SerializeField]
    GameObject EnemyParent;

    public bool UseInvisibleWall;

    Grid3D<CellType> grid;
    List<Room> rooms;
    Delaunay3D delaunay;
    HashSet<Prim.Edge> selectedEdges;


    void Start()
    {
        grid = new Grid3D<CellType>(size, Vector3Int.zero);
        rooms = new List<Room>();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
        if (UseInvisibleWall)
        {
            PlaceInvisWalls();
        }
        else
        {
            PlaceWalls();
        }
    }

    void PlaceRooms()
    {
        int HighestRoom = 0;
        int LowestRoom = 0;

        for (int i = 0; i < roomCount; i++)
        {
            Vector3Int location = new Vector3Int(
                //-2 to prevent hallway pathfind out of bound
                Random.Range(0, size.x - 2),
                Random.Range(0, size.y - 2),
                Random.Range(0, size.z - 2)
            );

            Vector3Int roomSize = new Vector3Int(
                Random.Range(roomMinSize.x, roomMaxSize.x + 1),
                Random.Range(roomMinSize.y, roomMaxSize.y + 1),
                Random.Range(roomMinSize.z, roomMaxSize.z + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2));

            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y
                || newRoom.bounds.zMin < 0 || newRoom.bounds.zMax >= size.z)
            {
                add = false;
            }

            if (add)
            {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);
                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }
                /* attempt #1
                for(int j= newRoom.bounds.xMin; j < newRoom.bounds.xMax; ++j)
                {
                    for(int k= newRoom.bounds.zMin; k < newRoom.bounds.zMax; ++k)
                    {
                        grid[new Vector3Int(j, newRoom.bounds.yMin, k)] = CellType.RoomBot;
                    }
                }
                */
                if (newRoom.bounds.yMin > rooms[HighestRoom].bounds.yMin)
                {
                    HighestRoom = rooms.Count - 1;
                }
                else if (newRoom.bounds.yMin < rooms[LowestRoom].bounds.yMin)
                {
                    LowestRoom = rooms.Count - 1;
                }
            }
        }
        // Place Characters in rooms
        for (int i = 0; i < rooms.Count; ++i)
        {
            if (i == HighestRoom)
            {
                PlacePC(rooms[i].bounds.position, rooms[i].bounds.size, PlayerCharacter);
            }

            else if (i == LowestRoom)
            {
                PlaceBoss(rooms[i].bounds.position, rooms[i].bounds.size, Boss);
            }

            else
            {
                PlaceEnemy(rooms[i].bounds.position, rooms[i].bounds.size, Enemy);
            }
        }
    }

    void Triangulate()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms)
        {
            vertices.Add(new Vertex<Room>((Vector3)room.bounds.position + ((Vector3)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay3D.Triangulate(vertices);
    }

    void CreateHallways()
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges)
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges)
        {
            if (Random.Range(0f, 1f) < 0.125)
            {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways()
    {
        DungeonPathfinder3D aStar = new DungeonPathfinder3D(size);

        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            // made sure it paths to room floor
            var startPos = new Vector3Int((int)startPosf.x, (int)startRoom.bounds.yMin, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endRoom.bounds.yMin, (int)endPosf.z);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder3D.Node a, DungeonPathfinder3D.Node b) => {
                var pathCost = new DungeonPathfinder3D.PathCost();

                var delta = b.Position - a.Position;

                if (delta.y == 0)
                {
                    //flat hallway
                    pathCost.cost = Vector3Int.Distance(b.Position, endPos);    //heuristic
                    /* attempt#2
                    if (grid.InBounds(b.Position + Vector3Int.up))
                    {
                        if(grid[b.Position + Vector3Int.up] == CellType.Hallway || grid[b.Position + Vector3Int.up] == CellType.Stairs )
                        {
                            Debug.Log("Path Denied");
                            return pathCost;
                        }
                    }
                    */
                    if (grid[b.Position] == CellType.Stairs)
                    {
                        return pathCost;
                    }
                    else if (grid[b.Position] == CellType.Room)
                    {
                        pathCost.cost += 5;
                    }
                    else if (grid[b.Position] == CellType.None)
                    {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;
                }
                else
                {
                    //staircase
                    if ((grid[a.Position] != CellType.None && grid[a.Position] != CellType.Hallway)
                        || (grid[b.Position] != CellType.None && grid[b.Position] != CellType.Hallway)) return pathCost;

                    pathCost.cost = 100 + Vector3Int.Distance(b.Position, endPos);    //base cost + heuristic

                    int xDir = Mathf.Clamp(delta.x, -1, 1);
                    int zDir = Mathf.Clamp(delta.z, -1, 1);
                    Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                    Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                    if (!grid.InBounds(a.Position + verticalOffset)
                        || !grid.InBounds(a.Position + horizontalOffset)
                        || !grid.InBounds(a.Position + verticalOffset + horizontalOffset))
                    {
                        return pathCost;
                    }

                    if (grid[a.Position + horizontalOffset] != CellType.None
                        || grid[a.Position + horizontalOffset * 2] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None)
                    {
                        return pathCost;
                    }

                    pathCost.traversable = true;
                    pathCost.isStairs = true;
                }

                return pathCost;
            });

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];

                    if (grid[current] == CellType.None)
                    {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0)
                    {
                        var prev = path[i - 1];

                        var delta = current - prev;

                        if (delta.y != 0)
                        {
                            int xDir = Mathf.Clamp(delta.x, -1, 1);
                            int zDir = Mathf.Clamp(delta.z, -1, 1);
                            Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                            Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                            grid[prev + horizontalOffset] = CellType.Stairs;
                            grid[prev + horizontalOffset * 2] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset * 2] = CellType.Stairs;



                            if (delta.y == 1)
                            {
                                PlaceStairs(prev + horizontalOffset, verticalOffset + horizontalOffset);
                                PlaceStairs(prev + new Vector3(0, 0.5f, 0) + horizontalOffset * 2, verticalOffset + horizontalOffset);
                            }

                            else
                            {
                                PlaceStairs(prev + new Vector3(0, -0.5f, 0) + horizontalOffset, verticalOffset + horizontalOffset);
                                PlaceStairs(prev + new Vector3(0, -1f, 0) + horizontalOffset * 2, verticalOffset + horizontalOffset);
                            }

                        }

                        Debug.DrawLine(prev + new Vector3(0.5f, 0.5f, 0.5f), current + new Vector3(0.5f, 0.5f, 0.5f), Color.blue, 100, false);
                    }
                }

                foreach (var pos in path)
                {
                    if (grid[pos] == CellType.Hallway)
                    {
                        PlaceHallway(pos);
                    }
                }
            }
        }
    }

    void PlaceRoom(Vector3Int location, Vector3Int size)
    {
        PlaceFloor(location, size, roomfloorPrefab);
    }

    void PlaceFloor(Vector3Int location, Vector3Int size, GameObject floorPrefab)
    {
        for (int i = 0; i < size.x; ++i)
        {
            for (int j = 0; j < size.z; ++j)
            {
                GameObject go = Instantiate(floorPrefab, new Vector3(location.x + i, location.y, location.z + j), Quaternion.identity);
                go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.25f, 0.25f);
                go.GetComponent<Transform>().SetParent(EnvironmentParent.GetComponent<Transform>());
            }
        }
    }

    void PlaceHallway(Vector3Int location)
    {
        PlaceFloor(location, new Vector3Int(1, 1, 1), hallwayfloorPrefab);
    }

    void PlaceStairs(Vector3 location, Vector3Int direction)
    {
        GameObject go = Instantiate(stairsPrefab, location, Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.2f, 0.25f);
        go.GetComponent<Transform>().SetParent(EnvironmentParent.GetComponent<Transform>());
        if (direction == new Vector3Int(0, 1, 1))
            go.GetComponent<Transform>().Rotate(0f, 180f, 0f);
        else if (direction == new Vector3Int(0, 1, -1))
            go.GetComponent<Transform>().Rotate(0f, 0f, 0f);
        else if (direction == new Vector3Int(1, 1, 0))
            go.GetComponent<Transform>().Rotate(0f, -90f, 0f);
        else if (direction == new Vector3Int(-1, 1, 0))
            go.GetComponent<Transform>().Rotate(0f, 90f, 0f);

        else if (direction == new Vector3Int(0, -1, 1))
            go.GetComponent<Transform>().Rotate(0f, 0f, 0f);
        else if (direction == new Vector3Int(0, -1, -1))
            go.GetComponent<Transform>().Rotate(0f, 180f, 0f);
        else if (direction == new Vector3Int(1, -1, 0))
            go.GetComponent<Transform>().Rotate(0f, 90f, 0f);
        else if (direction == new Vector3Int(-1, -1, 0))
            go.GetComponent<Transform>().Rotate(0f, -90f, 0f);

    }

    void PlaceWalls()
    {
        for (int j = 0; j < size.y; ++j)
        {
            for (int i = 0; i < size.x; ++i)
            {
                for (int k = 0; k < size.z; ++k)
                {
                    if (grid[new Vector3Int(i, j, k)] == CellType.None)
                    {
                        continue;
                    }
                    // if it is neighboring empty grid, build wall
                    if (i == 0 || grid[new Vector3Int(i - 1, j, k)] == CellType.None)
                    {
                        GameObject go = Instantiate(wallPrefab, new Vector3(i, j, k), Quaternion.identity);
                        go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.2f, 0.25f);
                        go.GetComponent<Transform>().Rotate(0f, 90f, 0f);
                        go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                    }
                    if (i == (size.x - 1) || grid[new Vector3Int(i + 1, j, k)] == CellType.None)
                    {
                        GameObject go = Instantiate(wallPrefab, new Vector3(i, j, k), Quaternion.identity);
                        go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.2f, 0.25f);
                        go.GetComponent<Transform>().Rotate(0f, -90f, 0f);
                        go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                    }
                    if (k == 0 || grid[new Vector3Int(i, j, k - 1)] == CellType.None)
                    {
                        GameObject go = Instantiate(wallPrefab, new Vector3(i, j, k), Quaternion.identity);
                        go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.2f, 0.25f);
                        go.GetComponent<Transform>().Rotate(0f, 0f, 0f);
                        go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                    }
                    if (k == (size.z - 1) || grid[new Vector3Int(i, j, k + 1)] == CellType.None)
                    {
                        GameObject go = Instantiate(wallPrefab, new Vector3(i, j, k), Quaternion.identity);
                        go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.2f, 0.25f);
                        go.GetComponent<Transform>().Rotate(0f, 180f, 0f);
                        go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                    }
                }
            }
        }
    }

    void PlaceInvisWalls()
    {
        for (int j = 0; j < size.y; ++j)
        {
            for (int i = 0; i < size.x; ++i)
            {
                for (int k = 0; k < size.z; ++k)
                {
                    if (grid[new Vector3Int(i, j, k)] == CellType.None)
                    {
                        continue;
                    }
                    // if it is neighboring empty grid, build wall
                    if (i == 0 || grid[new Vector3Int(i - 1, j, k)] == CellType.None)
                    {
                        GameObject go = Instantiate(inviswallPrefab, new Vector3(i - 0.5f, j + 0.5f, k), Quaternion.identity);
                        go.GetComponent<Transform>().localScale = new Vector3(0.1f, 1, 1);
                        go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                    }
                    if (i == (size.x - 1) || grid[new Vector3Int(i + 1, j, k)] == CellType.None)
                    {
                        GameObject go = Instantiate(inviswallPrefab, new Vector3(i + 0.5f, j + 0.5f, k), Quaternion.identity);
                        go.GetComponent<Transform>().localScale = new Vector3(0.1f, 1, 1);
                        go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                    }
                    if (k == 0 || grid[new Vector3Int(i, j, k - 1)] == CellType.None)
                    {
                        GameObject go = Instantiate(inviswallPrefab, new Vector3(i, j + 0.5f, k - 0.5f), Quaternion.identity);
                        go.GetComponent<Transform>().localScale = new Vector3(1, 1, 0.1f);
                        go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                    }
                    if (k == (size.z - 1) || grid[new Vector3Int(i, j, k + 1)] == CellType.None)
                    {
                        GameObject go = Instantiate(inviswallPrefab, new Vector3(i, j + 0.5f, k + 0.5f), Quaternion.identity);
                        go.GetComponent<Transform>().localScale = new Vector3(1, 1, 0.1f);
                        go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                    }
                }
            }
        }
    }

    void PlaceEnemy(Vector3Int location, Vector3Int size, GameObject[] Enemy)
    {
        int EnemyCount = Random.Range(EnemyPerRoomMin, EnemyPerRoomMax);
        for (int i = 0; i < EnemyCount; ++i)
        {
            if (i >= (size.x * size.z))
            {
                break;
            }
            Vector3 rand = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
            GameObject enemy = Instantiate(Enemy[Random.Range(0, Enemy.Length)], new Vector3(location.x + (size.x) / 2, location.y, location.z + (size.z) / 2)+rand, Quaternion.identity);
            
            enemy.GetComponent<Transform>().SetParent(EnemyParent.GetComponent<Transform>());
        }
    }

    void PlacePC(Vector3Int location, Vector3Int size, GameObject PC)
    {
        PC.transform.position = new Vector3(location.x + (size.x) / 2, location.y, location.z + (size.z) / 2);
    }

    void PlaceBoss(Vector3Int location, Vector3Int size, GameObject[] Boss)
    {
        int bossType = 0;
        if (GameManager.gameLevel >= 1)
        {
            bossType = Random.Range(0, Boss.Length);
        }
        Debug.Log("currentLevel = " + GameManager.gameLevel);
        GameObject boss = Instantiate(Boss[bossType], new Vector3(location.x + (size.x) / 2, location.y, location.z + (size.z) / 2), Quaternion.identity);
        boss.GetComponent<Transform>().SetParent(EnemyParent.GetComponent<Transform>());
        boss.GetComponent<EnemyController>().isBoss = true;
        GameManager.Instance.bossAlive = true;
    }
}
