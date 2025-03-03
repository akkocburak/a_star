using System;
using System.Collections.Generic;
using System.Linq;

class Node
{
    public int[,] State;
    public int G; // Gerçek maliyet
    public int H; // Heuristik maliyet
    public int F => G + H; // Toplam maliyet
    public Node Parent;

    public Node(int[,] state, int g, int h, Node parent)
    {
        State = (int[,])state.Clone();
        G = g;
        H = h;
        Parent = parent;
    }
}

class AStar8Puzzle
{
    private static int[,] GoalState;
    private static List<Node> OpenList = new List<Node>();
    private static HashSet<string> ClosedList = new HashSet<string>();

    public static void Solve(int[,] startState, int[,] goalState)
    {
        GoalState = goalState;
        Node startNode = new Node(startState, 0, CalculateHeuristic(startState), null);
        OpenList.Add(startNode);

        while (OpenList.Count > 0)
        {
            OpenList = OpenList.OrderBy(n => n.F).ToList();
            Node current = OpenList[0];
            OpenList.RemoveAt(0);

            if (IsGoal(current.State))
            {
                PrintSolution(current);
                return;
            }

            ClosedList.Add(GetStateKey(current.State));

            foreach (var neighbor in GetNeighbors(current))
            {
                if (ClosedList.Contains(GetStateKey(neighbor.State)))
                    continue;
                OpenList.Add(neighbor);
            }
        }
        Console.WriteLine("Çözüm bulunamadı.");
    }

    private static bool IsGoal(int[,] state) => GetStateKey(state) == GetStateKey(GoalState);

    private static string GetStateKey(int[,] state)
    {
        return string.Join(",", state.Cast<int>());
    }

    private static int CalculateHeuristic(int[,] state)
    {
        int h = 0;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if (state[i, j] == 0) continue;
                for (int x = 0; x < 3; x++)
                    for (int y = 0; y < 3; y++)
                        if (GoalState[x, y] == state[i, j])
                            h += Math.Abs(i - x) + Math.Abs(j - y);
            }
        return h;
    }

    private static List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        (int x, int y) = FindZero(node.State);
        int[][] directions = { new int[] { 0, -1 }, new int[] { 0, 1 }, new int[] { -1, 0 }, new int[] { 1, 0 } };


        foreach (var dir in directions)
        {
            int newX = x + dir[0], newY = y + dir[1];
            if (newX >= 0 && newX < 3 && newY >= 0 && newY < 3)
            {
                int[,] newState = (int[,])node.State.Clone();
                (newState[x, y], newState[newX, newY]) = (newState[newX, newY], newState[x, y]);
                neighbors.Add(new Node(newState, node.G + 1, CalculateHeuristic(newState), node));
            }
        }
        return neighbors;
    }

    private static (int, int) FindZero(int[,] state)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (state[i, j] == 0)
                    return (i, j);
        return (-1, -1);
    }

    private static void PrintSolution(Node node)
    {
        Stack<Node> path = new Stack<Node>();
        while (node != null)
        {
            path.Push(node);
            node = node.Parent;
        }
        while (path.Count > 0)
        {
            PrintState(path.Pop().State);
            Console.WriteLine();
        }
    }

    private static void PrintState(int[,] state)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write(state[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    static void Main()
    {
        int[,] startState =
        {
            { 1, 2, 3 },
            { 4, 0, 5 },
            { 6, 7, 8 }
        };

        int[,] goalState =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 0 }
        };

        Solve(startState, goalState);
        Console.WriteLine("Çıkmak için bir tuşa basın...");
        Console.ReadKey();

    }
}
