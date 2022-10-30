using System.Numerics;

namespace CgaLabs.Drawing;

public static class Extensions
{
    public static Vector3Int ToVector3Int(this Vector3 value)
    {
        return new Vector3Int
        {
            X = (int)value.X,
            Y = (int)value.Y,
            Z = (int)value.Z
        };
    }
}