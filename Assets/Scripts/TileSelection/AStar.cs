using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// A class to do A* pathfinding.
/// Taken from http://playmedusa.com/blog/verbose-astar-pathfinding-algorithm-in-c-for-unity3d/
/// and modified for our needs
/// </summary>

public class AStar {

    private class Node {
        public int x;
        public int y;
        public float G;
        public float H;
        public float F;
        public Node parent;
        public Tile cell;
        public Node(int x, int y, float G, float F, float H, Node parent, Tile c) {
            this.x = x;
            this.y = y;
            this.G = G;
            this.H = H;
            this.F = F;
            this.parent = parent;
            this.cell = c;
        }
    }

    private List<Node> openList;
    private List<Node> closeList;
    private List<Node> neighbours;
    private List<Node> finalPath;
    private Node start;
    private Node end;

    private Map map;
    private int mapWidth;
    private int mapHeight;

    public AStar() {
        openList = new List<Node>();
        closeList = new List<Node>();
        neighbours = new List<Node>();
        finalPath = new List<Node>();
    }

    public void FindPath(Tile startCell, Tile goalCell, Map map) {
        this.map = map;
        this.mapWidth = map.Width;
        this.mapHeight = map.Depth;

        start = new Node(startCell.XPos, startCell.YPos, 0, 0, 0, null, startCell);
        end = new Node(goalCell.XPos, goalCell.YPos, 0, 0, 0, null, goalCell);
        openList.Add(start);
        bool keepSearching = true;
        bool pathExists = true;

        while ((keepSearching) && (pathExists)) {
            Node currentNode = ExtractBestNodeFromOpenList();
            if (currentNode == null) {
                pathExists = false;
                break;
            }
            closeList.Add(currentNode);
            if (NodeIsGoal(currentNode)) { 
                keepSearching = false;
            } else {
                FindValidFourNeighbours(currentNode);
                foreach (Node neighbour in neighbours) {
                    if (FindInCloseList(neighbour) != null)
                        continue;
                    Node inOpenList = FindInOpenList(neighbour);
                    if (inOpenList == null) {
                        openList.Add(neighbour);
                    } else {
                        if (neighbour.G < inOpenList.G) {
                            inOpenList.G = neighbour.G;
                            inOpenList.F = inOpenList.G + inOpenList.H;
                            inOpenList.parent = currentNode;
                        }
                    }
                }
            }
        }

        if (pathExists) {
            Node n = FindInCloseList(end);
            while (n != null) {
                finalPath.Add(n);
                n = n.parent;
            }
        }
    }

    public List<Tile> CellsFromPath() {
        List<Tile> path = new List<Tile>();
        foreach (Node n in finalPath) {
            path.Add(n.cell);
        }

        if (path.Count != 0) {
            path.Reverse();
            path.RemoveAt(0);
        }
        return path;
    }

    Node ExtractBestNodeFromOpenList() {
        float minF = float.MaxValue;
        Node bestOne = null;
        foreach (Node n in openList) {
            if (n.F < minF) {
                minF = n.F;
                bestOne = n;
            }
        }
        if (bestOne != null)
            openList.Remove(bestOne);
        return bestOne;
    }

    bool NodeIsGoal(Node node) {
        return ((node.x == end.x) && (node.y == end.y));
    }

    void FindValidFourNeighbours(Node n) {
        neighbours.Clear();
        if ((n.y - 1 >= 0) && ((map[n.x, n.y - 1].IsValidForMovement))) {
            Node vn = PrepareNewNodeFrom(n, 0, -1);
            neighbours.Add(vn);
        }
        if ((n.y + 1 <= mapHeight - 1) && ((map[n.x, n.y + 1].IsValidForMovement))) {
            Node vn = PrepareNewNodeFrom(n, 0, +1);
            neighbours.Add(vn);
        }
        if ((n.x - 1 >= 0) && ((map[n.x - 1, n.y].IsValidForMovement))) {
            Node vn = PrepareNewNodeFrom(n, -1, 0);
            neighbours.Add(vn);
        }
        if ((n.x + 1 <= mapWidth - 1) && ((map[n.x + 1, n.y].IsValidForMovement))) {
            Node vn = PrepareNewNodeFrom(n, 1, 0);
            neighbours.Add(vn);
        }
    }

    Node PrepareNewNodeFrom(Node n, int x, int y) {
        Node newNode = new Node(n.x + x, n.y + y, 0, 0, 0, n, map[n.x + x, n.y + y]);
        newNode.G = n.G + MovementCost(n, newNode);
        newNode.H = Heuristic(newNode);
        newNode.F = newNode.G + newNode.H;
        newNode.parent = n;
        return newNode;
    }

    float Heuristic(Node n) {
        return Mathf.Sqrt((n.x - end.x) * (n.x - end.x) + (n.y - end.y) * (n.y - end.y));
    }

    float MovementCost(Node a, Node b) {
        //return map[b.x, b.y].MovementCost();
        return 0;
    }

    Node FindInCloseList(Node n) {
        foreach (Node nn in closeList) {
            if ((nn.x == n.x) && (nn.y == n.y))
                return nn;
        }
        return null;
    }

    Node FindInOpenList(Node n) {
        foreach (Node nn in openList) {
            if ((nn.x == n.x) && (nn.y == n.y))
                return nn;
        }
        return null;
    }
}
