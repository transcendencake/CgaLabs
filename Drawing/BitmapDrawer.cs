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

    public Bitmap GetBitmap(List<Vector3> points, GraphicsModel model, int width, int height)
    {
        this.vertexes = points;
        this.width = width;
        this.height = height;
        bitsMatrix = Enumerable.Repeat(Color.White.ToArgb(), width * height).ToArray();
        bitsHandle = GCHandle.Alloc(bitsMatrix, GCHandleType.Pinned);
        bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, bitsHandle.AddrOfPinnedObject());

        model.PolygonalIndexes.ForEach(DrawLines);

        return bitmap;
    }

    private void DrawLines(List<Vector3> vertexIndexes)
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

        var pointFrom = GetPoint(vertexFrom);
        var pointTo = GetPoint(vertexTo);

        DrawLinePoints(pointFrom, pointTo);
    }

    private void DrawLinePoints(Point point1, Point point2)
    {
        var incX = 1;
        var incY = 1;

        var deltaX = Math.Abs(point2.X - point1.X);
        var deltaY = Math.Abs(point2.Y - point1.Y);

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
            DrawPoint(point1, bitmap);

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
        }

        DrawPoint(point2, bitmap);
    }

    private Point GetPoint(Vector3 vector)
    {
        return new Point((int)vector.X, (int)vector.Y);
    }

    private void DrawPoint(Point point, Bitmap bitmap)
    {
        if (point.X > 0 && point.X < bitmap.Width && point.Y > 0 && point.Y < bitmap.Height)
            bitsMatrix[point.X + (point.Y * width)] = currentColor;
    }

    public void Dispose()
    {
        bitsHandle.Free();
        bitmap.Dispose();
    }
}