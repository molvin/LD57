using System;

public enum NodeType
{
    Start = 0,
    End = 1,
    Challenge = 2,
    Item = 3,
    Blocker = 4,
}

[Serializable]
public struct Node
{
    public float X;
    public float Y;
    public NodeType Type;
}

[Serializable]
public struct Level
{
    public int Seed;
    public Node[] Nodes;
    public Edge[] Edges;
}

[Serializable]
public struct Edge
{
    public Node First;
    public Node Second;
}

public static class TestLevels
{
    public static Level Level0()
    {
        Node start = new() { X = 0, Y = 0, Type = NodeType.Start };
        Node end = new() { X = 100, Y = -25, Type = NodeType.End };
        return new Level()
        {
            Nodes = new Node[] { start, end },
            Edges = new Edge[] { new() { First = start, Second = end} } 
        };
    }

    public static Level Level1()
    {
        Node start = new() { X = 0, Y = 0, Type = NodeType.Start };
        Node mid0 = new() { X = 30, Y = -15, Type = NodeType.Challenge };
        Node mid1 = new() { X = 65, Y = -25, Type = NodeType.Challenge };
        Node mid2 = new() { X = 120, Y = -35, Type = NodeType.Challenge };
        Node end = new() { X = 150, Y = -50, Type = NodeType.End };

        return new Level()
        {
            Nodes = new Node[] { start, mid0, mid1, mid2, end },
            Edges = new Edge[] { 
                new() { First = start, Second = mid0 },
                new() { First = mid0, Second = mid1 },
                new() { First = mid1, Second = mid2 },
                new() { First = mid2, Second = end }
            }
        };
    }
}
