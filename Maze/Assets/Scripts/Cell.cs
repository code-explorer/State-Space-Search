using UnityEngine;

namespace Generation
{
    public class Cell
    {
        public int i, j; // Index in cells array of maze object
        public Cell[] neighbors; // Neighboring cells
        public GameObject[] walls;
        public bool visited;
        public Cell parent;
        public bool inMaze; // Whether the cell is a part of the maze
        public Cell(int _i, int _j)
        {
            neighbors = new Cell[4];
            walls = new GameObject[4];
            i = _i;
            j = _j;
            visited = false;
            parent = null;
            inMaze = false;
        }
    }
}
