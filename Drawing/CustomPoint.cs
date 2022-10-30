using System.Numerics;

namespace CgaLabs.Drawing;

public struct CustomPoint
{
    public Vector3 Normal;
    public Vector3Int View;
    public Vector4 World;
}

public struct Vector3Int
{
    public int X;
    public int Y;
    public int Z;
}