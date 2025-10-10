using ForgeEvo.Core.Engine;
using ForgeEvo.Core.Graphics;

namespace ForgeEvo.Demo;

public class ShapeGame() : Game(title: "Shape Demo")
{
    private const int Radius = 200;
    private const int Padding = 25;

    private readonly Color _background = new(100, 149, 237);
    private Circle[] _circles = [];

    protected override void Initialize()
    {
        _circles =
        [
            new(
                new(Padding + Radius, Display.Size.Height / 2F), Radius, Color.White.WithAlpha(100), Circle.MinSegments
            ),
            new(
                new((Padding + Radius) * 2 + Radius, Display.Size.Height / 2F), Radius, Color.PureRed.WithAlpha(100), 8
            ),
            new(
                new((Padding + Radius) * 3 + Radius * 2, Display.Size.Height / 2F), Radius,
                Color.PureGreen.WithAlpha(100)
            ),
            new(
                new((Padding + Radius) * 4 + Radius * 3, Display.Size.Height / 2F), Radius,
                Color.PureBlue.WithAlpha(100), Circle.MaxSegments
            )
        ];
    }

    protected override void Update(float deltaTime)
    {
        if (InputHandler.IsKeyPressed(Key.Escape))
        {
            Console.WriteLine("Escape pressed, quitting...");
            Environment.Exit(0);
        }

        if (InputHandler.IsKeyDown(Key.Up))
            for (var i = 0; i < _circles.Length; i++)
                _circles[i].Radius += 200 * deltaTime;

        else if (InputHandler.IsKeyDown(Key.Down))
            for (var i = 0; i < _circles.Length; i++)
                _circles[i].Radius -= 200 * deltaTime;
    }

    protected override void Render()
    {
        foreach (Circle circle in _circles)
            circle.Enqueue();

        Display.Render(_background);
    }
}