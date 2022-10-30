using System.Numerics;

namespace CgaLabs.Drawing;

public static class Extensions
{
    public static Vector3ForDrawing ToVector3Int(this Vector3 value)
    {
        return new Vector3ForDrawing
        {
            X = (int)value.X,
            Y = (int)value.Y,
            Z = value.Z
        };
    }

    public static Vector3 ToVector3(this Vector3ForDrawing value)
    {
        return new Vector3
        {
            X = value.X,
            Y = value.Y,
            Z = value.Z
        };
    }

    public static Vector3 ToVector3(this Color color)
    {
        return new Vector3(color.R, color.G, color.B);
    }
}