using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CgaLabs.Drawing;

public class BitmapDrawer : IDisposable
{
    private List<Vector3> vertexes;
    private int width;
    private int height;
    private readonly int currentColor = Color.Black.ToArgb();
    private GCHandle bitsHandle;
    private int[] bitsMatrix;
    private Bitmap bitmap;
    private GraphicsModel model;
    private Vector3 lightPosition;

    public Bitmap GetBitmap(List<Vector3> points, GraphicsModel model, int width, int height, Vector3 light)
    {
        this.vertexes = points;
        this.width = width;
        this.height = height;
        bitsMatrix = Enumerable.Repeat(Color.White.ToArgb(), width * height).ToArray();
        bitsHandle = GCHandle.Alloc(bitsMatrix, GCHandleType.Pinned);
        bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, bitsHandle.AddrOfPinnedObject());
        this.model = model;
        this.lightPosition = light;

        DrawSun(Color.Brown.ToArgb());
        model.PolygonalIndexes.ForEach(DrawPolygon);

        return bitmap;
    }

    private void DrawPolygon(List<Vector3> vertexIndexes)
    {
        var edgePoints = new List<CustomPoint>();
        for (var i = 0; i < vertexIndexes.Count - 1; i++)
        {
            edgePoints.AddRange(GetLinePoints(i, i + 1, vertexIndexes));
        }

        edgePoints.AddRange(GetLinePoints(0, vertexIndexes.Count - 1, vertexIndexes));

        RasterizeAndDrawPolygonByEdgePoints(edgePoints);
    }

    private void RasterizeAndDrawPolygonByEdgePoints(List<CustomPoint> edgePoints)
    {
        if (edgePoints.Count == 0)
        {
            return;
        }

        var orderedByYs = edgePoints
            .Select(p => p.View.Y)
            .OrderBy(p => p)
            .ToList();
        var minBoundaryY = orderedByYs.First();
        var maxBoundaryY = orderedByYs.Last();

        for (int y = minBoundaryY + 1; y < maxBoundaryY; y++)
        {
            var linePoints = edgePoints
                .Where(p => p.View.Y == y)
                .OrderBy(p => p.View.X)
                .ToList();
            var pointFrom = linePoints.First();
            var pointTo = linePoints.Last();

            var points = GetLinePoints(pointFrom, pointTo);

            foreach (var point in points)
            {
                DrawPoint(point);
            }
        }
    }

    private List<CustomPoint> GetLinePoints(int from, int to, List<Vector3> indexes, bool draw = true)
    {
        var vertexIndexFrom = (int)indexes[from].X - 1;
        var vertexIndexTo = (int)indexes[to].X - 1;
        var normalIndexFrom = (int)indexes[from].Z - 1;
        var normalIndexTo = (int)indexes[to].Z - 1;

        var pointFrom = GetCustomPoint(vertexes[vertexIndexFrom], model.Normals[normalIndexFrom], model.Vertexes[vertexIndexFrom]);
        var pointTo = GetCustomPoint(vertexes[vertexIndexTo], model.Normals[normalIndexTo], model.Vertexes[vertexIndexTo]);

        var points = GetLinePoints(pointFrom, pointTo);

        if (draw)
        {
            foreach (var point in points)
            {
                DrawPoint(point);
            }
        }

        return points;
    }

    private List<CustomPoint> GetLinePoints(CustomPoint point1, CustomPoint point2)
    {
        var point1X = point1.View.X;
        var point2X = point2.View.X;
        var point1Y = point1.View.Y;
        var point2Y = point2.View.Y;
        var point1Z = point1.View.Z;
        var normal = point1.Normal;
        var world = point1.World;

        var deltaX = Math.Abs(point2X - point1X);
        var deltaY = Math.Abs(point2Y - point1Y);
        var iterationsAmount = Math.Max(deltaX, deltaY);
        if (iterationsAmount == 0)
        {
            return Enumerable.Empty<CustomPoint>().ToList();
        }

        var deltaWorld = (point2.World - point1.World) / iterationsAmount;
        var deltaNormal = (point2.Normal - point1.Normal) / iterationsAmount;
        var deltaZ = (point2.View.Z - point1.View.Z) / iterationsAmount;

        var result = new List<CustomPoint>(iterationsAmount);

        var incX = point1X > point2X ? -1 : 1;
        var incY = point1Y > point2Y ? -1 : 1;

        var error = deltaX - deltaY;

        while (point1X != point2X || point1Y != point2Y)
        {
            var newPoint = new CustomPoint
            {
                View = new Vector3Int
                {
                    X = point1X,
                    Y = point1Y,
                    Z = point1.View.Z
                },
                Normal = normal + Vector3.Zero,
                World = world + Vector4.Zero
            };
            result.Add(newPoint);

            var errorMultipliedBy2 = error * 2;

            if (errorMultipliedBy2 > -deltaY)
            {
                error -= deltaY;
                point1X += incX;
            }

            if (errorMultipliedBy2 < deltaX)
            {
                error += deltaX;
                point1Y += incY;
            }

            point1Z += deltaZ;
            normal += deltaNormal;
            world += deltaWorld;
        }

        var newPoint2 = new CustomPoint
        {
            View = new Vector3Int
            {
                X = point1X,
                Y = point1Y,
                Z = point1.View.Z
            },
            Normal = normal + Vector3.Zero,
            World = world + Vector4.Zero
        };
        result.Add(newPoint2);

        return result;
    }

    private CustomPoint GetCustomPoint(Vector3 vector, Vector3 normal, Vector4 world)
    {
        return new CustomPoint
        {
            View = vector.ToVector3Int(),
            Normal = normal,
            World = world
        };
    }

    private void DrawSun(int color)
    {
        var radius = 5;
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    DrawPoint((int)lightPosition.X + x, (int)lightPosition.Y + y, color);
                }
            }
        }
    }

    private void DrawPoint(CustomPoint point)
    {
        DrawPoint(point, currentColor);
    }

    private void DrawPoint(CustomPoint point, int color)
    {
        DrawPoint(point.View.X, point.View.Y, color);
    }

    private void DrawPoint(int x, int y, int color)
    {
        if (x > 0 && x < bitmap.Width && y > 0 && y < bitmap.Height)
            bitsMatrix[x + (y * width)] = color;
    }

    public void Dispose()
    {
        bitsHandle.Free();
        bitmap.Dispose();
    }
}