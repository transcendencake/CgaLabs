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
}