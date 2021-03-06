using System.Collections;
using System.Collections.Generic;
using Generation;
using Search;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Maze maze; // Maze containing cells
    public GameObject wall; // Prefab of wall
    public int n; // Number of rows
    public int m; // Number of columns
    private float cellWidth;
    private float cellHeight;
    public GameObject guide;
    public float wallDestroyRate;
    public int wallsDestroyed = 0;
    public float pathVisualizationRate;

    void Start()
    {
        maze = new Maze(n, m); // Initializing maze object which store all the cells
        cellWidth = 100f / n;
        cellHeight = 100f / m;
        Vector3 location;
        Quaternion rotation;

        // Creating walls between cells
        for (int k = 0; k < 4; k++)
        {
            if (k % 2 == 0)
            {
                wall.transform.localScale =
                    new Vector3(cellWidth, wall.transform.localScale.y, 1);
                if (k == 2)
                    location = new Vector3(0, 0, -cellHeight / 2);
                else
                    location = new Vector3(0, 0, +cellHeight / 2);
                rotation = new Quaternion(90, 0, 0, 0);
            }
            else
            {
                wall.transform.localScale =
                    new Vector3(1, wall.transform.localScale.y, cellHeight);
                if (k == 1)
                    location = new Vector3(-cellWidth / 2, 0, 0);
                else
                    location = new Vector3(+cellWidth / 2, 0, 0);

                rotation = new Quaternion(0, 0, 90, 0);
            }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    maze.cells[i, j].walls[k] =
                        Instantiate(wall,
                        location +
                        new Vector3((i - n / 2) * cellWidth,
                            0,
                            (j - m / 2) * cellHeight),
                        rotation);
        }
    }

    void Update()
    {
        if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.R))
        {
            removeRandomWall();
        }
    }

    public void createGuide(int i, int j)
    {
        guide.transform.position = new Vector3((i * 100) / n - 50 + guide.transform.localScale.z * 5, 1, (j * 100) / n - 50 + guide.transform.localScale.z * 5);

        Instantiate(guide);
    }

    public void solveUsingRandomizedDFS()
    {
        RDFS rdfs = new RDFS(this);
        rdfs.generateMaze();
    }

    public void solveUsingRandomizedKruskals()
    {
        Kruskal kruskal = new Kruskal(this);
        kruskal.generateMaze();
    }

    public void solveUsingRandomizedPrims()
    {
        Prim prim = new Prim(this);
        prim.generateMaze();
    }

    public void solveUsingBFS()
    {
        Cell start = maze.cells[Player.startIndex, 0];
        Cell goal = maze.cells[Player.goalIndex, n - 1];

        BFS bfs = new BFS(start, goal);
        Cell path = bfs.solve();

        StartCoroutine(generatePathVisualization(path));
    }

    public void solveUsingDFS()
    {
        Cell start = maze.cells[Player.startIndex, 0];
        Cell goal = maze.cells[Player.goalIndex, n - 1];

        DFS dfs = new DFS(start, goal);
        Cell path = dfs.solve();

        StartCoroutine(generatePathVisualization(path));
    }

    public void solveUsingGreedy()
    {
        Cell start = maze.cells[Player.startIndex, 0];
        Cell goal = maze.cells[Player.goalIndex, n - 1];

        Greedy greedy = new Greedy(start, goal);
        Cell path = greedy.solve();

        StartCoroutine(generatePathVisualization(path));
    }

    public void solveUsingAStar()
    {
        Cell start = maze.cells[Player.startIndex, 0];
        Cell goal = maze.cells[Player.goalIndex, n - 1];

        AStar aStar = new AStar(start, goal);
        Cell path = aStar.solve();

        StartCoroutine(generatePathVisualization(path));
    }

    public IEnumerator generatePathVisualization(Cell path)
    {
        Stack<Cell> stack = new Stack<Cell>();

        while (path != null)
        {
            stack.Push(path);
            path = path.parent;
        }

        Debug.Log(stack.Count);

        while (stack.Count != 0)
        {
            path = stack.Pop();
            createGuide(path.i, path.j);
            yield return new WaitForSeconds(pathVisualizationRate);
        }
    }

    public void removeRandomWall()
    {
        System.Random random = new System.Random();
        setNeighbor(random.Next(4), maze.cells[random.Next(n), random.Next(m)]);
    }

    // Sets reference to neighbor and destroy walls in between
    // top - 0, left - 1, down - 2, right - 3
    public void setNeighbor(int direction, Cell c1)
    {
        Cell[] neighbors = getNeighbors(c1);
        Cell c2 = neighbors[direction];
        if (c2 == null) return;

        c1.neighbors[direction] = c2;
        GameObject w1 = c1.walls[direction];
        c1.walls[direction] = null;

        if (direction > 1)
            direction -= 2;
        else
            direction += 2;

        c2.neighbors[direction] = c1;
        GameObject w2 = c2.walls[direction];
        c2.walls[direction] = null;

        wallsDestroyed++;
        StartCoroutine(destroyWalls(w1, w2));
    }

    public IEnumerator destroyWalls(GameObject w1, GameObject w2)
    {
        yield return new WaitForSeconds(wallDestroyRate * wallsDestroyed);

        Destroy(w1);
        Destroy(w2);
    }

    // Get possible neighbors of given cell
    public Cell[] getNeighbors(Cell c)
    {
        Cell[] neighbors = new Cell[4];
        if (c.j + 1 < n) neighbors[0] = maze.cells[c.i, c.j + 1];
        if (c.i - 1 >= 0) neighbors[1] = maze.cells[c.i - 1, c.j];
        if (c.j - 1 >= 0) neighbors[2] = maze.cells[c.i, c.j - 1];
        if (c.i + 1 < m) neighbors[3] = maze.cells[c.i + 1, c.j];
        return neighbors;
    }
}
