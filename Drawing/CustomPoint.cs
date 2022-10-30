using System.Numerics;

namespace CgaLabs.Drawing;

public struct CustomPoint
{
    public Vector3 Normal;
    public Vector3ForDrawing View;
    public Vector3 World;
}

public struct Vector3ForDrawing
{
    public int X;
    public int Y;
    public float Z;
}