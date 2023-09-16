//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Graphs;

public class Generator2D : MonoBehaviour
{
    enum CellType
    {
        None,
        Room,
        Hallway
    }

    class Room
    {
        public RectInt bounds;

        public Room(Vector2Int location, Vector2Int size)
        {
            bounds = new RectInt(location, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
        }
    }

    [SerializeField]
    Vector2Int size;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector2Int roomMinSize;
    [SerializeField]
    Vector2Int roomMaxSize;
    [SerializeField]
    GameObject roomfloorPrefab;
    [SerializeField]
    GameObject hallwayfloorPrefab;
    [SerializeField]
    GameObject wallPrefab;
    [SerializeField]
    GameObject PlayerCharacter;
    [SerializeField]
    GameObject Enemy;
    [SerializeField]
    GameObject Boss;
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

    Grid2D<CellType> grid;
    List<Room> rooms;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        grid = new Grid2D<CellType>(size, Vector2Int.zero);
        rooms = new List<Room>();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
        PlaceWalls();
    }

    void PlaceRooms()
    {
        int HighestRoom = 0;
        int LowestRoom = 0;
        for (int i = 0; i < roomCount; i++)
        {
            Vector2Int location = new Vector2Int(
                Random.Range(0, size.x),
                Random.Range(0, size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                Random.Range(roomMinSize.x, roomMaxSize.x + 1),
                Random.Range(roomMinSize.y, roomMaxSize.y + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y)
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
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }

    void CreateHallways()
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges)
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges)
        {
            if (Random.Range(0f,1f) < 0.125)
            {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways()
    {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(size);

        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();

                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (grid[b.Position] == CellType.Room)
                {
                    pathCost.cost += 10;
                }
                else if (grid[b.Position] == CellType.None)
                {
                    pathCost.cost += 5;
                }
                else if (grid[b.Position] == CellType.Hallway)
                {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

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
    
    void PlaceRoom(Vector2Int location, Vector2Int size)
    {
        PlaceFloor(location, size, roomfloorPrefab);
    }

    void PlaceFloor(Vector2Int location, Vector2Int size, GameObject floorPrefab)
    {
        for(int i=0; i < size.x; ++i)
        {
            for(int j=0; j<size.y; ++j)
            {
                GameObject go = Instantiate(floorPrefab, new Vector3(location.x + i , 0 ,  location.y + j ), Quaternion.identity);
                go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
        }
    }

    void PlaceHallway(Vector2Int location)
    {
        PlaceFloor(location, new Vector2Int(1, 1), hallwayfloorPrefab);
    }

    void PlaceWalls()
    {
        for (int i = 0; i < size.x; ++i)
        {
            for (int k = 0; k < size.y; ++k)
            {
                if (grid[new Vector2Int(i, k)] == CellType.None)
                {
                    continue;
                }
                // if it is neighboring empty grid, build wall
                if (i == 0 || grid[new Vector2Int(i - 1, k)] == CellType.None)
                {
                    GameObject go = Instantiate(wallPrefab, new Vector3(i, 0, k), Quaternion.identity);
                    go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.2f, 0.25f);
                    go.GetComponent<Transform>().Rotate(0f, 90f, 0f);
                    go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                }
                if (i == (size.x - 1) || grid[new Vector2Int(i + 1, k)] == CellType.None)
                {
                    GameObject go = Instantiate(wallPrefab, new Vector3(i, 0, k), Quaternion.identity);
                    go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.2f, 0.25f);
                    go.GetComponent<Transform>().Rotate(0f, -90f, 0f);
                    go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                }
                if (k == 0 || grid[new Vector2Int(i, k - 1)] == CellType.None)
                {
                    GameObject go = Instantiate(wallPrefab, new Vector3(i, 0, k), Quaternion.identity);
                    go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.2f, 0.25f);
                    go.GetComponent<Transform>().Rotate(0f, 0f, 0f);
                    go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                }
                if (k == (size.y - 1) || grid[new Vector2Int(i, k + 1)] == CellType.None)
                {
                    GameObject go = Instantiate(wallPrefab, new Vector3(i, 0, k), Quaternion.identity);
                    go.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.2f, 0.25f);
                    go.GetComponent<Transform>().Rotate(0f, 180f, 0f);
                    go.GetComponent<Transform>().SetParent(WallParent.GetComponent<Transform>());
                }
            }
        }
    }

    void PlacePC(Vector2Int location, Vector2Int size, GameObject PC)
    {
        //place at center of the room
        PC.transform.position = new Vector3(location.x + (size.x) / 2, 0, location.y + (size.y) / 2);
    }   

    void PlaceEnemy(Vector2Int location, Vector2Int size, GameObject Enemy)
    {
        int EnemyCount = Random.Range(EnemyPerRoomMin, EnemyPerRoomMax);
        for (int i = 0; i < EnemyCount; ++i)
        {
            if (i >= (size.x * size.y))
            {
                break;
            }
            GameObject enemy = Instantiate(Enemy, new Vector3(location.x + (i % size.x), 0, location.y + (i / size.x)), Quaternion.identity);
            enemy.GetComponent<Transform>().SetParent(EnemyParent.GetComponent<Transform>());
        }
    }

    void PlaceBoss(Vector2Int location, Vector2Int size, GameObject Boss)
    {
        GameObject boss = Instantiate(Boss, new Vector3(location.x + (size.x) / 2, 0, location.y + (size.y) / 2), Quaternion.identity);
        boss.GetComponent<Transform>().SetParent(EnemyParent.GetComponent<Transform>());
    }

}