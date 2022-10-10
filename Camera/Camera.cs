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
}