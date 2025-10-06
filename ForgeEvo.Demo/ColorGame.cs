using ForgeEvo.Core.Engine;
using ForgeEvo.Core.Graphics;
using ForgeEvo.Core.Math;

namespace ForgeEvo.Demo;

internal class ColorGame() : Game(title: "Color & Mouse Demo")
{
    private byte _blueComponent = byte.MinValue;
    private byte _greenComponent = byte.MinValue;
    private byte _redComponent = byte.MinValue;

    private Color ClearColor => new(_redComponent, _greenComponent, _blueComponent);

    protected override void Initialize()
    {
    }

    protected override void Update(float deltaTime)
    {
        if (InputHandler.IsKeyPressed(Key.Escape))
        {
            Console.WriteLine("Escape pressed, quitting...");
            Environment.Exit(0);
        }

        if (InputHandler.IsKeyPressed(Key.Space))
            _blueComponent = _blueComponent == byte.MaxValue ? byte.MinValue : byte.MaxValue;

        if (InputHandler.DidMouseMove)
        {
            Vector2D mousePosition = InputHandler.MousePosition.Normal() * 255;

            _redComponent = (byte)mousePosition.X;
            _greenComponent = (byte)mousePosition.Y;
        }
    }

    protected override void Render()
    {
        Display.Render(ClearColor);
    }
}