using System.Collections.Concurrent;
using System.Numerics;

namespace CgaLabs.Drawing;

public static class VertexTransformatingUtils
{
    public static void Transform(Camera.Camera camera, GraphicsModel model, int width, int height, float xRotation, float yRotation)
    {
        var viewPortMatrix = GetViewportSpace(width, height);
        var worldMatrix = GetWorldSpace(model.Scale, xRotation, yRotation);
        var viewMatrix = GetViewSpace(camera);
        var projectionMatrix = GetPerspectiveSpace(camera.FieldOfViewRadians, width, height);
        var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;
        model.TransformedVertexes = GetWindowSpace(transformMatrix, model.Vertexes, viewPortMatrix);
        model.TransformedNormals = GetTransformedNormals(model.Normals, xRotation, yRotation);
    }

    private static Matrix4x4 GetWorldSpace(float scale, float xRotation, float yRotation)
    {
        return Matrix4x4.CreateScale(scale) * Matrix4x4.CreateRotationX(xRotation) * Matrix4x4.CreateRotationY(yRotation);
    }

    private static List<Vector3> GetTransformedNormals(List<Vector4> normals, float xRotation, float yRotation)
    {
        var result = new List<Vector3>(normals.Count);
        var transformationMatrix = Matrix4x4.CreateRotationX(xRotation) * Matrix4x4.CreateRotationY(yRotation);
        foreach (var normal in normals)
        {
            var point = Vector4.Transform(normal, transformationMatrix);
            result.Add(new Vector3(
                point.X,
                point.Y,
                point.Z
            ));
        }
        return result;
    }

    private static Matrix4x4 GetPerspectiveSpace(float fov, float width, float height)
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(fov, width / height, 0.1f, 1900.0f);
    }

    private static Matrix4x4 GetViewSpace(Camera.Camera camera)
    {
        var result = Matrix4x4.CreateLookAt(camera.Eye, camera.Target, camera.Up);
        return result;
    }

    private static Matrix4x4 GetViewportSpace(float width, float height, float xMin = 0, float yMin = 0)
    {
        return new Matrix4x4(
            width / 2, 0, 0, 0,
            0, -height / 2, 0, 0,
            0, 0, 1, 0,
            xMin + width / 2, yMin + height / 2, 0, 1
        );
    }

    private static List<Vector3> GetWindowSpace(Matrix4x4 transformMatrix, List<Vector4> vertexes, Matrix4x4 viewPortMatrix)
    {
        var windowPoints = new Vector3[vertexes.Count];

        Parallel.ForEach(Partitioner.Create(0, vertexes.Count), range =>
        {
            for (var i = range.Item1; i < range.Item2; i++)
            {
                var transformedPoint = Vector4.Transform(vertexes[i], transformMatrix);
                transformedPoint /= transformedPoint.W;
                var displayPoint = Vector4.Transform(transformedPoint, viewPortMatrix);
                windowPoints[i] = new Vector3(
                    displayPoint.X,
                    displayPoint.Y,
                    displayPoint.Z
                );
            }
        });

        return windowPoints.ToList();
    }
}