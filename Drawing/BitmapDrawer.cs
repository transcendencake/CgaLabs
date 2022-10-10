using System.Numerics;

namespace CgaLabs.Drawing;

public class BitmapDrawer
{
    private List<Vector3> vertexes;
    private readonly Color activeColor = Color.Black;
    private Bitmap bitmap;

    public Bitmap GetBitmap(List<Vector3> points, GraphicsModel model, int width, int height)
    {
        this.vertexes = points;
        this.bitmap = new Bitmap(width, height);

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
        var signX = 1;
        var signY = 1;

        var deltaX = Math.Abs(point2.X - point1.X);
        var deltaY = Math.Abs(point2.Y - point1.Y);

        if (point1.X > point2.X)
        {
            signX = -1;
        }

        if (point1.Y > point2.Y)
        {
            signY = -1;
        }

        var error = deltaX - deltaY;

        while (point1.X != point2.X || point1.Y != point2.Y)
        {
            DrawPoint(point1, bitmap);

            var error2 = error * 2;

            if (error2 > -deltaY)
            {
                error -= deltaY;
                point1.X += signX;
            }

            if (error2 >= deltaX) continue;
            error += deltaX;
            point1.Y += signY;
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
            bitmap.SetPixel(point.X, point.Y, activeColor);
    }
}