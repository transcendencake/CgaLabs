using System.Collections.Concurrent;
using System.Numerics;

namespace CgaLabs.Drawing;

public static class VertexTransformatingUtils
{
    public static List<Vector3> Transform(Camera.Camera camera, GraphicsModel model, int width, int height)
    {
        // Мировые координаты
        var worldMatrix = GetWorldSpace(model.Scale);

        // Координаты наблюдателя
        var viewMatrix = GetViewSpace(camera);

        // Координаты перспективной проекции
        var projectionMatrix = GetPerspectiveSpace(camera.FieldOfViewRadians, width, height);

        // Матрица трансформации
        var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

        // Координаты окна
        return GetWindowSpace(transformMatrix, model.Vertexes, width, height);
    }

    private static Matrix4x4 GetWorldSpace(float scale)
    {
        return Matrix4x4.CreateScale(scale);
    }

    private static Matrix4x4 GetPerspectiveSpace(float fov, float width, float height)
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(fov, width / height, 0.1f, 200.0f);
    }

    private static Matrix4x4 GetViewSpace(Camera.Camera camera)
    {
        return Matrix4x4.CreateLookAt(camera.Eye, camera.Target, camera.Up);
    }

    private static Matrix4x4 GetViewPortSpace(float width, float height, float xMin = 0, float yMin = 0)
    {
        return new Matrix4x4(
            width / 2, 0, 0, 0,
            0, -height / 2, 0, 0,
            0, 0, 1, 0,
            width / 2, height / 2, 0, 1
        );
    }

    private static List<Vector3> GetWindowSpace(Matrix4x4 transformMatrix, List<Vector4> vertexes, int width, int height)
    {
        var windowPoints = new Vector3[vertexes.Count];

        // Координаты в соответствии с шириной и высотой экрана
        var viewPortMatrix = GetViewPortSpace(width, height);

        Parallel.ForEach(Partitioner.Create(0, vertexes.Count), range =>
        {
            for (var i = range.Item1; i < range.Item2; i++)
            {
                var transformedPoint = Vector4.Transform(vertexes[i], transformMatrix);
                transformedPoint /= transformedPoint.W;
                var displayedPoint = Vector4.Transform(transformedPoint, viewPortMatrix);
                windowPoints[i] = new Vector3(
                    displayedPoint.X,
                    displayedPoint.Y,
                    displayedPoint.Z
                );
            }
        });

        return windowPoints.ToList();
    }
}