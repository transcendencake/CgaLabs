using System.Numerics;

namespace CgaLabs;

public class GraphicsModel
{
    public List<Vector4> Vertexes { get; set; } = new();

    public List<Vector3> Normals { get; set; } = new();

    public List<List<Vector3>> PolygonalIndexes { get; set; } = new();

    public float Scale { get; set; } = 1;

    public int MaxCoordinate { get; private set; } = 1;

    public void RecalculateMax()
    {
        var max = int.MinValue;
        foreach (var vertex in Vertexes)
        {
            var max1 = (int)Math.Max(vertex.X, vertex.Y);
            var max2 = (int)Math.Max(vertex.Z, vertex.W);
            max1 = Math.Max(max1, max2);
            if (max1 > max)
            {
                max = max1;
            }
        }

        this.MaxCoordinate = max;
    }
}