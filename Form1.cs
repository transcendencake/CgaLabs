using System.Diagnostics;
using System.Numerics;
using CgaLabs.Drawing;
using CgaLabs.Utils;
using Timer = System.Windows.Forms.Timer;

namespace CgaLabs;

public partial class Form1 : Form
{
    private Point mousePosition = new(0, 0);
    private bool isMouseDown;
    private GraphicsModel? graphicsModel;
    private readonly Camera.Camera camera;
    private readonly Timer timer;
    private float scaleModifier = 300;
    private float xRotation = 0;
    private float yRotation = 0;
    private BitmapDrawer? drawer;
    private Vector3 lightPosition = new Vector3(600, 100, 200);

    public Form1()
    {
        InitializeComponent();
        camera = new Camera.Camera
        {
            Eye = new Vector3(0, 0, -300),
            Target = new Vector3(0, 0, 0),
            Up = new Vector3(0, 1, 0),
            FieldOfViewRadians = (float) Math.PI / 3
        };

        timer = new Timer
        {
            Interval = 6,
            Enabled = false,
        };
        timer.Tick += Redraw;
    }

    private void Redraw(object? sender, EventArgs e)
    {
        timer.Stop();

        graphicsModel.Scale = scaleModifier / (graphicsModel.MaxCoordinate * 5);
        VertexTransformatingUtils.Transform(camera, graphicsModel, Width, Height, xRotation, yRotation);

        if (drawer != null)
        {
            drawer.Dispose();
        }
        drawer = new BitmapDrawer();
        this.BackgroundImage = drawer.GetBitmap(graphicsModel, Width, Height, lightPosition, camera.Eye);

        timer.Start();
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
    }

    private void Form1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        var delta = e.Delta > 0 ? 1.5f : 0.66f;

        scaleModifier *= delta;
    }

    private void Form1_MouseDown(object sender, MouseEventArgs e)
    {
        isMouseDown = true;
        SaveMousePosition(e);
    }

    private void Form1_MouseUp(object sender, MouseEventArgs e)
    {
        isMouseDown = false;
    }

    private void Form1_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isMouseDown) return;
        var xOffset = e.X - mousePosition.X;
        var yOffset = mousePosition.Y - e.Y;
        SaveMousePosition(e);

        xRotation += PixelOffsetToRotationRadians(yOffset);
        yRotation += PixelOffsetToRotationRadians(xOffset);
    }

    private void SaveMousePosition(MouseEventArgs e)
    {
        mousePosition.X = e.X;
        mousePosition.Y = e.Y;
    }

    private void loadToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            DefaultExt = ".obj",
            Filter = "Object files|*.obj",
        };

        if (openFileDialog.ShowDialog() == DialogResult.Abort)
        {
            return;
        }

        var filePath = openFileDialog.FileName;
        Debug.WriteLine(filePath);

        graphicsModel = ObjParser.GetModel(filePath);
        timer.Enabled = true;
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Up:
                this.lightPosition.Y -= 30;
                break;
            case Keys.Down:
                this.lightPosition.Y += 30;
                break;
            case Keys.Left:
                this.lightPosition.X -= 30;
                break;
            case Keys.Right:
                this.lightPosition.X += 30;
                break;
        }
    }

    private float PixelOffsetToRotationRadians(float pixelOffset)
    {
        return 0.005f * pixelOffset;
    }
}