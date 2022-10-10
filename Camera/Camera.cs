using System.Numerics;

namespace CgaLabs.Camera;

public class Camera
{
    // camera position
    public Vector3 Eye { get; set; }

    // where we look at
    public Vector3 Target { get; set; }

    // direction which is up for camera
    public Vector3 Up { get; set; }

    public float FieldOfViewRadians { get; set; }

    public void RotateY(float angleRadians)
    {
        Eye = Vector3.Transform(Eye, Matrix4x4.CreateRotationY(angleRadians));
        Up = Vector3.Transform(Up, Matrix4x4.CreateRotationY(angleRadians));
    }

    public void RotateX(float angleRadians)
    {
        Eye = Vector3.Transform(Eye, Matrix4x4.CreateRotationX(angleRadians));
        Up = Vector3.Transform(Up, Matrix4x4.CreateRotationX(angleRadians));
    }

    public void RotateZ(float angleRadians)
    {
        Eye = Vector3.Transform(Eye, Matrix4x4.CreateRotationZ(angleRadians));
        Up = Vector3.Transform(Up, Matrix4x4.CreateRotationZ(angleRadians));
    }
}