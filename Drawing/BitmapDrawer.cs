using System.Buffers;
using System.Collections.Concurrent;
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
    private ArrayPool<CustomPoint> arrayPool;

    public Bitmap GetBitmap(List<Vector3> points, GraphicsModel model, int width, int height)
    {
        this.vertexes = points;
        this.width = width;
        this.model = model;
        this.height = height;
        bitsMatrix = Enumerable.Repeat(Color.White.ToArgb(), width * height).ToArray();
        bitsHandle = GCHandle.Alloc(bitsMatrix, GCHandleType.Pinned);
        bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, bitsHandle.AddrOfPinnedObject());

        arrayPool = ArrayPool<CustomPoint>.Create(1000000, 100);
        Parallel.ForEach(Partitioner.Create(0, model.PolygonalIndexes.Count), range =>
        {
            for (var i = range.Item1; i < range.Item2; i++)
            {
                var polygon = model.PolygonalIndexes[i];
                DrawPolygon(polygon);
            }
        });
        // foreach (var polygon in model.PolygonalIndexes)
        // {
        //     DrawPolygon(polygon);
        // }

        return bitmap;
    }

    private void DrawPolygon(List<Vector3> vertexIndexes)
    {
        for (var i = 0; i < vertexIndexes.Count - 1; i++)
        {
            DrawLine(i, i + 1, vertexIndexes);
        }

        DrawLine(0, vertexIndexes.Count - 1, vertexIndexes);
    }

    private void DrawLine(int from, int to, List<Vector3> indexes)
    {
        var indexFrom = (int)indexes[from].X - 1;
        var indexTo = (int)indexes[to].X - 1;

        var vertexFrom = vertexes[indexFrom];
        var vertexTo = vertexes[indexTo];

        // var points = GetLinePoints(vertexFrom, vertexTo);
        // foreach (var point in points)
        // {
        //     DrawPoint(point);
        // }
        //
        // arrayPool.Return(points);
    }

    private CustomPoint[] GetLinePoints(Vector3 point1, Vector3 point2)
    {
        var incX = 1;
        var incY = 1;

        var deltaX = Math.Abs(point2.X - point1.X);
        var deltaY = Math.Abs(point2.Y - point1.Y);

        // var result = arrayPool.Rent(1000000);
        var i = 0;

        if (point1.X > point2.X)
        {
            incX = -1;
        }

        if (point1.Y > point2.Y)
        {
            incY = -1;
        }

        var error = deltaX - deltaY;

        while (point1.X != point2.X || point1.Y != point2.Y)
        {
            // result[i] = new CustomPoint
            // {
            //     View = new Vector3(point1.X, point1.Y, point1.Z)
            // };
            DrawPoint(new CustomPoint
            {
                View = new Vector3(point1.X, point1.Y, point1.Z)
            });

            var errorMultipliedBy2 = error * 2;

            if (errorMultipliedBy2 > -deltaY)
            {
                error -= deltaY;
                point1.X += incX;
            }

            if (errorMultipliedBy2 < deltaX)
            {
                error += deltaX;
                point1.Y += incY;
            }

            i++;
        }

        // result[i] = new CustomPoint
        // {
        //     View = new Vector3(point1.X, point1.Y, point1.Z)
        // };
        return new CustomPoint[1];
    }

    private void DrawPoint(CustomPoint point, int color)
    {
        var roundedX = (int) point.View.X;
        var roundedY = (int) point.View.Y;
        if (roundedX > 0 && roundedX < width && roundedY > 0 && roundedY < height)
            bitsMatrix[roundedX + (roundedY * width)] = color;
    }

    private void DrawPoint(CustomPoint point)
    {
        DrawPoint(point, currentColor);
    }

    public void Dispose()
    {
        bitsHandle.Free();
        bitmap.Dispose();
    }
}