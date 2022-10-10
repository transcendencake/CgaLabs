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
    private GraphicsModel graphicsModel;
    private readonly Camera.Camera camera;
    private readonly Timer timer;

    public Form1()
    {
        InitializeComponent();
        camera = new Camera.Camera
        {
            Eye = new Vector3(0, 0, 500),
            Target = new Vector3(0, 0, 0),
            Up = new Vector3(0, 1, 0),
            FieldOfViewRadians = (float) Math.PI / 3
        };

        timer = new Timer
        {
            Interval = 33,
            Enabled = false,
        };
        timer.Tick += Redraw;
    }

    private void Redraw(object? sender, EventArgs e)
    {
        timer.Stop();

        graphicsModel.Scale = 500 / (graphicsModel.MaxCoordinate * 4);
        var points = VertexTransformatingUtils.Transform(camera, graphicsModel, Width, Height);
        var drawer = new BitmapDrawer();
        this.BackgroundImage = drawer.GetBitmap(points, graphicsModel, Width, Height);

        timer.Start();
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
        //update bitmap drawing here
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

        camera.RotateX(PixelOffsetToRotationRadians(yOffset));
        camera.RotateY(PixelOffsetToRotationRadians(xOffset));
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

    private float PixelOffsetToRotationRadians(float pixelOffset)
    {
        return 0.01f * pixelOffset;
    }
}