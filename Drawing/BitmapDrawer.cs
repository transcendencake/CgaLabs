using System.Diagnostics;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using Xamarin.Essentials;

namespace CgaLabs.Drawing;

public class BitmapDrawer : IDisposable
{
    private int width;
    private int height;
    private readonly Color currentColor = Color.DarkRed;
    private GCHandle bitsHandle;
    private int[] bitsMatrix;
    private float[] zBuffer;
    private Bitmap bitmap;
    private GraphicsModel model;
    private Vector3 lightPosition;
    private Vector3 cameraPosition;

    public Bitmap GetBitmap(GraphicsModel model, int width, int height, Vector3 light)
    {
        this.width = width;
        this.height = height;
        bitsMatrix = Enumerable.Repeat(Color.White.ToArgb(), width * height).ToArray();
        zBuffer = Enumerable.Repeat(float.MaxValue, width * height).ToArray();
        bitsHandle = GCHandle.Alloc(bitsMatrix, GCHandleType.Pinned);
        bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, bitsHandle.AddrOfPinnedObject());
        this.model = model;
        this.lightPosition = light;
        this.cameraPosition = model.TransformedCamera;

        DrawSun(Color.Brown.ToArgb());
        model.PolygonalIndexes.ForEach(DrawPolygon);

        return bitmap;
    }

    private void DrawPolygon(List<Vector3> vertexIndexes)
    {
        // var vertexColors = vertexIndexes
        //     .Select(p => CalculateVertexShadowedColor(model.TransformedVertexes[(int)p.X - 1], model.TransformedNormals[(int)p.Z - 1], currentColor))
        //     .ToList();
        // var polygonColor = currentColor.WithLuminosity(CalculateAverageColor(vertexColors) * 100);

        var edgePoints = new List<CustomPoint>();
        for (var i = 0; i < vertexIndexes.Count - 1; i++)
        {
            edgePoints.AddRange(GetLinePoints(i, i + 1, vertexIndexes));
        }

        edgePoints.AddRange(GetLinePoints(0, vertexIndexes.Count - 1, vertexIndexes));

        // RasterizeAndDrawPolygonByEdgePoints(edgePoints, polygonColor.ToArgb());
        RasterizeAndDrawPolygonByEdgePoints(edgePoints, 0);
    }

    private float CalculateVertexShadowedColor(Vector3 vertex, Vector3 normal, Color color)
    {
        var cos = Vector3.Dot(Vector3.Normalize(normal), Vector3.Normalize(vertex - lightPosition));
        var k = cos > 0 ? cos : 0;
        return color.GetBrightness() * k;
    }

    private float CalculateAverageColor(List<float> colors)
    {
        return colors.Sum() / colors.Count;
    }

    private void RasterizeAndDrawPolygonByEdgePoints(List<CustomPoint> edgePoints, int color)
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
                DrawPoint(point, color);
            }

            foreach (var point in edgePoints)
            {
                DrawPoint(point, color);
            }
        }
    }

    private List<CustomPoint> GetLinePoints(int from, int to, List<Vector3> indexes, bool draw = false)
    {
        var vertexIndexFrom = (int)indexes[from].X - 1;
        var vertexIndexTo = (int)indexes[to].X - 1;
        var normalIndexFrom = (int)indexes[from].Z - 1;
        var normalIndexTo = (int)indexes[to].Z - 1;

        var pointFrom = GetCustomPoint(model.TransformedVertexes[vertexIndexFrom], model.TransformedNormals[normalIndexFrom]);
        var pointTo = GetCustomPoint(model.TransformedVertexes[vertexIndexTo], model.TransformedNormals[normalIndexTo]);

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

        var deltaX = Math.Abs(point2X - point1X);
        var deltaY = Math.Abs(point2Y - point1Y);
        var iterationsAmount = Math.Max(deltaX, deltaY);
        if (iterationsAmount == 0)
        {
            return Enumerable.Empty<CustomPoint>().ToList();
        }

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
                View = new Vector3ForDrawing
                {
                    X = point1X,
                    Y = point1Y,
                    Z = point1Z
                },
                Normal = normal + Vector3.Zero,
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
        }

        var newPoint2 = new CustomPoint
        {
            View = new Vector3ForDrawing
            {
                X = point1X,
                Y = point1Y,
                Z = point1Z
            },
            Normal = normal + Vector3.Zero,
        };
        result.Add(newPoint2);

        return result;
    }

    private CustomPoint GetCustomPoint(Vector3 vector, Vector3 normal)
    {
        return new CustomPoint
        {
            View = vector.ToVector3Int(),
            Normal = normal
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
                    DrawPoint((int)lightPosition.X + x, (int)lightPosition.Y + y, (int)lightPosition.Z, color);
                }
            }
        }
    }

    private void DrawPoint(CustomPoint point)
    {
        DrawPoint(point, currentColor.ToArgb());
    }

    private void DrawPoint(CustomPoint point, int color)
    {
        DrawPoint(point.View.X, point.View.Y, point.View.Z, GetPointColor(point).ToArgb());
    }

    private void DrawPoint(int x, int y, float z, int color)
    {
        if (
            x > 0
            && x < bitmap.Width
            && y > 0
            && y < bitmap.Height
            && z < zBuffer[x + (y * width)])
        {
            zBuffer[x + (y * width)] = z;
            bitsMatrix[x + (y * width)] = color;
        }
    }

    private Color GetPointColor(CustomPoint point)
    {
        var vertex = point.View.ToVector3();
        var normal = Vector3.Normalize(point.Normal);
        var light = Vector3.Normalize(vertex - lightPosition);
        var view = Vector3.Normalize(cameraPosition - vertex);
        var reflect = Vector3.Normalize(Vector3.Reflect(-light, normal));

        var reflectionColor = 0.3f
            * (float) Math.Pow(Math.Max(0, Vector3.Dot(reflect, view)), 20)
            * Color.White.ToVector3();
        var diffuseColor = 0.7f * Math.Max(Vector3.Dot(normal, light), 0) * currentColor.ToVector3();
        var backgroundColor = 0.3f * currentColor.ToVector3();

        var colorVector = reflectionColor + diffuseColor + backgroundColor;
        var color = Color.FromArgb(
            255,
            Math.Min((int)colorVector.X, 255),
            Math.Min((int)colorVector.Y, 255),
            Math.Min((int)colorVector.Z, 255)
        );

        return color;
    }

    public void Dispose()
    {
        bitsHandle.Free();
        bitmap.Dispose();
    }
}